using Discord.WebSocket;
using Discord.Interactions;
using GhidorahBot.Common;
using GhidorahBot.Init;
using System.Reflection;
using Discord;
using GhidorahBot.Modules;
using GhidorahBot.Database;
using Microsoft.Extensions.Configuration;

namespace GhidorahBot.Services
{
    public class InteractionHandler : IInteractionHandler
    {
        private readonly string _teamSheet = "Team";
        private readonly string _playerSheet = "Player";
        private readonly string _matchResultSheet = "MatchResult";
        private readonly string _playerStatsSheet = "PlayerStats";
        private readonly string _rosterSheet = "Roster";
        private bool _authorized = false;

        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private Roster _roster { get; set; }
        private Search _search { get; set; }
        private Update _update { get; set; }
        private NewEntry _newentry { get; set; }
        private Feedback _feedback { get; set; }

        public InteractionHandler(DiscordSocketClient client, InteractionService interactionService)
        {
            _client = client;
            _commands = interactionService;
        }

        public async Task InitializeAsync(Roster roster, Search search, Update update, NewEntry newentry, Feedback feedback)
        {
            _roster = roster;
            _search = search;
            _update = update;
            _newentry = newentry;
            _feedback = feedback;

            var interactionModule = new InteractiveModule(_client);
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
                var ctx = new SocketInteractionContext(_client, arg);

                //Validate User
                var discordUser = ctx.User as SocketGuildUser;
                var leagueStaff = (discordUser as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "League Staff");

                foreach(var role in discordUser.Roles)
                {
                    if (role == leagueStaff)
                    {
                        await _commands.ExecuteCommandAsync(ctx, Bootstrapper.ServiceProvider);
                        _authorized = true;
                        break;
                    }
                }

                if(!_authorized)
                {
                    await ctx.Channel.SendMessageAsync("You do not have permission to use this command.");
                    _authorized = false;
                }
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
            (bool, string, string) _searchReturn;

            switch(modal.Data.CustomId)
            {
                case "modal_addnewteam":
                    _searchReturn = _search.SearchItem(modal, "Team", "team_Name", "B2:B");

                    if(!_searchReturn.Item1)
                    {
                        _newentry.AddNewTeam(modal, _teamSheet);
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"New Team has been added successfully\r\r" +
                            $"Team Name: {_newentry.TeamName}\r" +
                            $"Team Id: {_newentry.Id}");
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention} Team already exists");
                    }                 
                    break;
                case "modal_editteam":
                    _searchReturn = _search.SearchItem(modal, _teamSheet, "search_team_Name", "A2:B");

                    if(_searchReturn.Item1)
                    {
                        _update.UpdateTeam(modal, _searchReturn.Item2);
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"Team update successful\r\r" +
                            $"Team Id: {_searchReturn.Item2}");
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention} Something went wrong updating Team {_searchReturn.Item2}");
                    }                
                    break;
                case "modal_addnewplayer":
                    _searchReturn = _search.SearchItem(modal, _playerSheet, "player_activ_id", "B2:B");

                    if(!_searchReturn.Item1)
                    {
                        _newentry.AddNewPlayer(modal, _playerSheet);
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"New Player has been added successfully\r\r" +
                            $"Player Id: {_newentry.Id}\r" +
                            $"Activision Id: {_newentry.ActivisionId}");
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention} Player already exists");
                    }
                    break;
                case "modal_editplayer":
                    _searchReturn = _search.SearchItem(modal, _playerSheet, "search_player_id", "A2:B");

                    if (_searchReturn.Item1)
                    {
                        _update.UpdatePlayer(modal, _searchReturn.Item2);
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"Player updated successfully\r\r" +
                            $"Player Id: {_searchReturn.Item2}\r" +
                            $"Activision Id: {_update.ActivisionId}");
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention} Something went wrong updating Player {_searchReturn.Item2}");
                    }
                    break;
                case "modal_newteamroster":
                    var matchFound = _roster.SearchForTeam(modal);

                    if(matchFound.Item1)
                    {
                        var searchReturn = _roster.SearchForPlayer(modal);

                        if (searchReturn.Item2 > 6)
                        {
                            await modal.RespondAsync($"{modal.User.Mention}\r" +
                                $"Team has {searchReturn.Item2} player(s). A team roster can only have 6 total players");
                        }
                        else if (searchReturn.Item2 < 4)
                        {
                            await modal.RespondAsync($"{modal.User.Mention}\r" +
                                $"Team has {searchReturn.Item2} player(s). A team roster must have at least 4 players");
                        }
                        else
                        {
                            _roster.NewRoster(modal, _rosterSheet, searchReturn.Item4);
                            await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"Team Roster has been created successfully");
                        }
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                                $"{matchFound.Item2} does not exist or is currently listed as NOT active.\r\r" +
                                $"Please create a new team or activate this team before adding players to the roster.");
                    }
                    break;
                case "modal_updateRoster":
                    _searchReturn = _search.SearchItem(modal, _teamSheet, "updateroster_search_team_name", "A2:B");

                    if (_searchReturn.Item1)
                    {
                        //TODO:
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention} Something went wrong updating Team {_searchReturn.Item2}");
                    }
                    break;
                case "modal_newmatchresult":
                    var searchTeamOne = _search.SearchItem(modal, "Team", "team_1_name", "A2:B");
                    var searchTeamTwo = _search.SearchItem(modal, "Team", "team_2_name", "A2:B");

                    if(searchTeamOne.Item1 == true && searchTeamTwo.Item1 == true)
                    {
                        _newentry.AddNewMatchResult(modal, _matchResultSheet);
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"Match recorded!\r\r" +
                            $"Team 1: {searchTeamOne.Item3}\r" +
                            $"Team 2: {searchTeamTwo.Item3}\r" +
                            $"Match Guid: {_newentry.Guid}\r\r" +
                            $"Please use the Match Guid when entering player stats.");
                    }
                    else
                    {
                        if(searchTeamOne.Item1 == false)
                        {
                            await modal.RespondAsync($"{modal.User.Mention}\r\r" +
                                $"Could not find team name: {searchTeamOne.Item3}\r" +
                                $"Id: {searchTeamOne.Item3}");
                        }
                        else if (searchTeamTwo.Item1 == false)
                        {
                            await modal.RespondAsync($"{modal.User.Mention}\r\r" +
                                $"Could not find team name: {searchTeamTwo.Item3}\r" +
                                $"Id: {searchTeamTwo.Item3}");
                        }
                    }

                    break;
                case "modal_updatematchresult":
                    //update match result
                    break;
                case "modal_newplayerstats":
                    _searchReturn = _search.SearchItem(modal, _matchResultSheet, "newstats_search_matchguid", "B2:B");

                    if (_searchReturn.Item1)
                    {
                        var matchGuid = _searchReturn.Item2;

                        _searchReturn = _search.SearchItem(modal, _playerSheet, "newstats_playername", "A2:B");

                        var playerName = _searchReturn.Item3;

                        if(_searchReturn.Item1)
                        {
                            _newentry.AddNewPlayerStats(modal, _playerStatsSheet, matchGuid, playerName);
                            await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"Player stats have been recorded.");
                        }
                        else
                        {
                            await modal.RespondAsync($"{modal.User.Mention} Could not find player.");
                        }
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention} Could not find a match for the guid provided.");
                    }

                    break;
                case "modal_feedback":
                    _feedback.AddFeedback(modal);
                    await modal.RespondAsync($"{modal.User.Mention} Feedback Submitted.\r" +
                        $"Thank you for your valuable feedback.");
                    break;
            }
        }
    }
}
