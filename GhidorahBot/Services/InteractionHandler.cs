using Discord.WebSocket;
using Discord.Interactions;
using GhidorahBot.Common;
using GhidorahBot.Init;
using System.Reflection;
using Discord;
using GhidorahBot.Modules;
using GhidorahBot.Database;
using GhidorahBot.Models;
using GhidorahBot.Validation;
using GhidorahBot.Extensions;

namespace GhidorahBot.Services
{
    public class InteractionHandler : IInteractionHandler
    {       
        private List<TeamModel> _teamList = new List<TeamModel>();
        private List<PlayerModel> _playerList = new List<PlayerModel>();
        private List<RosterModel> _rosterList = new List<RosterModel>();

        private SocketInteractionContext _ctx;
        private string _channelName = string.Empty;
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private DataValidation _validation { get; set; }
        private PlayerQueueService _pq { get; set; }

        public InteractionHandler(DiscordSocketClient client, InteractionService interactionService)
        {
            _client = client;
            _commands = interactionService;
        }

        public async Task InitializeAsync(DataValidation validation, PlayerQueueService pq)
        {
            _validation = validation;
            _pq = pq;
            await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), Bootstrapper.ServiceProvider);

            _client.InteractionCreated += HandleInteraction;
            _client.ModalSubmitted += HandleModelAsync;

            // Process the command execution results
            _commands.SlashCommandExecuted += SlashCommandExecuted;
            _commands.ContextCommandExecuted += ContextCommandExecuted;
            _commands.ComponentCommandExecuted += ComponentCommandExecuted;

            _commands.SlashCommandExecuted += async (optional, context, result) =>
            {
                if (!result.IsSuccess && result.Error != InteractionCommandError.UnknownCommand)
                {
                    // the command failed, let's notify the user that something happened.
                    await context.Channel.SendMessageAsync($"error: {result}");
                }
            };

            foreach (var module in _commands.Modules)
            {
                await Logger.Log(LogSeverity.Info, $"{nameof(InteractionHandler)} | Commands", $"Module '{module.Name}' initialized.");
            }
        }

        private Task ComponentCommandExecuted(ComponentCommandInfo arg1, IInteractionContext arg2, IResult arg3)
        {
            return Task.CompletedTask;
        }

        private Task ContextCommandExecuted(ContextCommandInfo arg1, IInteractionContext arg2, IResult arg3)
        {
            return Task.CompletedTask;
        }

        private Task SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, IResult arg3)
        {
            return Task.CompletedTask;
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
                _ctx = new SocketInteractionContext(_client, arg);
                await _commands.ExecuteCommandAsync(_ctx, Bootstrapper.ServiceProvider); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (arg.Type == InteractionType.ApplicationCommand)
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }

        private async Task HandleModelAsync(SocketModal modal)
        {
            _validation.ValidateLeagueCreated(_ctx, modal);
            if(!string.IsNullOrWhiteSpace(_validation.RespondMessage))
            {
                await modal.RespondAsync(_validation.RespondMessage);
                return;
            }

            _validation.SetGoogleSheetValues(_ctx);
            if(_validation.IsLeagueCreated)
            {
                switch (modal.Data.CustomId)
                {
                    case "modal_addnewteam":
                        _validation.ValidateNewTeam(modal, _ctx);
                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                    case "modal_updateteam":
                        _validation.ValidateUpdateTeam(modal, _ctx);
                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                    case "modal_addnewplayer":
                        _validation.ValidateNewPlayer(modal, _ctx);
                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                    case "modal_updateplayer":
                        _validation.ValidateUpdatePlayer(modal, _ctx);
                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                    case "modal_rosteraddplayers":
                        _validation.ValidateAddPlayers(modal, _ctx);
                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                    case "modal_rosterremoveplayers":
                        _validation.ValidateRemovePlayers(modal, _ctx);
                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                    case "modal_newmatchresult":
                        _validation.ValidateNewMatchResult(modal, _ctx);
                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                    case "modal_updatematchresult":
                        _validation.ValidateUpdateMatchResult(modal, _ctx);
                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                    case "modal_newplayerstats":
                        _validation.ValidateNewPlayerStats(modal, _ctx);
                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                    case "modal_updateplayerstats":
                        _validation.ValidateUpdatePlayerStats(modal, _ctx);
                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                    case "modal_searchplayer":
                        _validation.ValidatePlayerSearch(modal);
                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                    case "modal_searchteam":
                        _validation.ValidateTeamSearch(modal);
                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                    case "modal_searchmatchresult":
                        _validation.ValidateMatchResultSearch(modal);
                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                    case "modal_feedback":
                        _validation.ValidateFeedback(modal);
                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                    case "modal_freeagent":
                        _validation.ValidateFreeAgent(modal);
                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                    case "modal_requestadmin":
                        var leagueStaffRole = _ctx.Guild.Roles.FirstOrDefault(x => x.Name == "League Staff");
                        _validation.ValidateAdminRequest(_ctx, modal);

                        if (_validation.IsAdminRequestValid)
                        {
                            var modalName = modal.Data.CustomId;
                            var components = modal.Data.Components.ToList();

                            string issue = components
                                    .First(x => x.CustomId == "admin_issue").Value;

                            var chnl = _client.GetChannel(_validation.ChannelId) as IMessageChannel;
                            await chnl.SendMessageAsync($"{leagueStaffRole.Mention} New Admin request!\r" +
                                $"Submitted by: {modal.User.Username}#{modal.User.Discriminator}\r" +
                                $"DateTime: {DateTime.Now}\r" +
                                $"Channel Message was requested from: {modal.Channel.Name}\r" +
                                $"Message: {issue}");

                            await modal.RespondAsync(_validation.RespondMessage);
                        }
                        else
                        {
                            //Error
                            await modal.RespondAsync(_validation.RespondMessage);
                        }
                        break;
                    case "modal_requestcaster":
                        var casterRole = _ctx.Guild.Roles.FirstOrDefault(x => x.Name == "Caster");
                        _validation.ValidateCasterRequest(_ctx, modal);
                        var casterModal = modal.Data.CustomId;
                        var modalComponents = modal.Data.Components.ToList();

                        string casterRequest = modalComponents
                            .First(x => x.CustomId == "request_caster").Value;

                        var casterChnl = _client.GetChannel(_validation.ChannelId) as IMessageChannel;
                        await casterChnl.SendMessageAsync($"{casterRole.Mention} New Caster request!\r" +
                            $"Submitted by: {modal.User.Username}#{modal.User.Discriminator}\r" +
                            $"Request: {casterRequest}");

                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                    case "modal_newscrim":
                        var scrimModalName = modal.Data.CustomId;
                        var scrimComponents = modal.Data.Components.ToList();

                        string activisionId = scrimComponents
                            .First(x => x.CustomId == "scrim_activisionId").Value;

                        string scrimTime = scrimComponents
                            .First(x => x.CustomId == "scrim_time").Value;

                        string notes = scrimComponents
                            .First(x => x.CustomId == "scrim_notes").Value;

                        _pq.JoinScrimQueue(modal.User, activisionId, scrimTime, notes, _ctx.Guild.Id, _ctx.Channel.Id);
                        await modal.RespondAsync(_pq.LocalNotification);
                        break;
                    case "modal_registerdiscordchnl":
                        _validation.ValidateRegisterAdminChnl(_ctx, modal);
                        await modal.RespondAsync(_validation.RespondMessage);
                        break;
                }
            }
            else
            {
                await modal.RespondAsync(_validation.RespondMessage);
            }
        }
    }
}
