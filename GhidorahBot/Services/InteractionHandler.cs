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

namespace GhidorahBot.Services
{
    public class InteractionHandler : IInteractionHandler
    {       
        private bool _authorized = false;

        private List<TeamModel> _teamList = new List<TeamModel>();
        private List<PlayerModel> _playerList = new List<PlayerModel>();
        private List<RosterModel> _rosterList = new List<RosterModel>();

        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private Search _search { get; set; }
        private Update _update { get; set; }
        private NewEntry _newentry { get; set; }
        private Feedback _feedback { get; set; }
        private DataValidation _validation { get; set; }

        public InteractionHandler(DiscordSocketClient client, InteractionService interactionService)
        {
            _client = client;
            _commands = interactionService;
        }

        public async Task InitializeAsync(Search search, Update update, NewEntry newentry, Feedback feedback, DataValidation validation)
        {
            _search = search;
            _update = update;
            _newentry = newentry;
            _feedback = feedback;
            _validation = validation;

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
                    else
                    {
                        _authorized = false;
                    }
                }

                if(!_authorized)
                {
                    await ctx.Channel.SendMessageAsync($"{ctx.User.Mention} You do not have permission to use this command.");
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
            var discordUser = modal.User as SocketGuildUser;
            var leagueStaff = (discordUser as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "League Staff");

            var orgOwner = (discordUser as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Organization Owner");

            //var teamCaptain = (discordUser as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Team Captain");

            //foreach(var role in discordUser.Roles)
            //{
            //    if(role == orgOwner)
            //    {
            //        await modal.Channel.SendMessageAsync($"{modal.User.Mention} You have the org owner role.");
            //    }
            //    else if(role == teamCaptain)
            //    {
            //        await modal.Channel.SendMessageAsync($"{modal.User.Mention} You have the team captain role.");
            //    }
            //}

            switch(modal.Data.CustomId)
            {
                case "modal_addnewteam":
                    _validation.ValidateNewTeam(modal);
                    if(_validation.IsRequestValid)
                    {
                        _newentry.AddNewTeam(modal);
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"New Team has been added successfully\r\r" +
                            $"Team Name: {_newentry.TeamName.ToUpper()}\r" +
                            $"Team Id: {_newentry.Id}");
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                        $"Error message:\r" +
                        $"{_validation.ValidationErrorMsg}");
                    }
                    break;
                case "modal_updateteam":
                    _validation.ValidateUpdateTeam(modal);
                    if(_validation.IsRequestValid)
                    {
                        _update.UpdateTeam(modal, _validation.TeamId);
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"Team updated successfully.\r" +
                            $"Updated Information (blank items were not updated):\r" +
                            $"Team Name: {_update.UserTeamInputList[0].TeamName.ToUpper()}\r" +
                            $"Twitter: {_update.UserTeamInputList[0].Twitter}\r" +
                            $"Group: {_update.UserTeamInputList[0].Group.ToUpper()}\r" +
                            $"Active: {_update.UserTeamInputList[0].Active.ToUpper()}");
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                        $"Error message:\r" +
                        $"{_validation.ValidationErrorMsg}");
                    }                   
                    break;
                case "modal_addnewplayer":
                    _validation.ValidateNewPlayer(modal);
                    if(_validation.IsRequestValid)
                    {
                        _newentry.AddNewPlayer(modal);
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"New Player has been added successfully\r\r" +
                            $"Player Id: {_newentry.Id}\r" +
                            $"Activision Id: {_newentry.ActivisionId.ToUpper()}");
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                        $"Error message:\r" +
                        $"{_validation.ValidationErrorMsg}");
                    }
                    break;
                case "modal_updateplayer":
                    _validation.ValidateUpdatePlayer(modal);
                    if(_validation.IsRequestValid)
                    {
                        _update.UpdatePlayer(modal, _validation.PlayerId);
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"Player updated successfully.\r" +
                            $"Updated Information (blank items were not updated):\r" +
                            $"Activision Id: {_update.UserPlayerInputList[0].ActivisionId.ToUpper()}\r" +
                            $"Discord: {_update.UserPlayerInputList[0].DiscordName}\r" +
                            $"Twitter: {_update.UserPlayerInputList[0].Twitter.ToUpper()}\r" +
                            $"Active: {_update.UserPlayerInputList[0].Active.ToUpper()}");
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                        $"Error message:\r" +
                        $"{_validation.ValidationErrorMsg}");
                    }
                    break;
                case "modal_updateRoster":
                    _validation.ValidateUpdateRoster(modal);
                    if(_validation.IsRequestValid)
                    {
                        _update.UpdateRoster(_validation.RemovePlayerList, _validation.AddPlayerList, _validation.RosterMatch);
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"Roster updated successfully.");
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                        $"Error message:\r" +
                        $"{_validation.ValidationErrorMsg}");
                    }
                    break;
                case "modal_newmatchresult":
                    _validation.ValidateMatchResult(modal);
                    if(_validation.IsRequestValid)
                    {
                        _newentry.AddNewMatchResult(_validation.TeamOne, _validation.TeamTwo, _validation.WinningTeam, _validation.LosingTeam,
                            _validation.TeamOneMapsWon, _validation.TeamTwoMapsWon, _validation.MatchGroup);
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"Match Recorded:\r\r" +
                            $"TEAM 1: {_validation.TeamOne.TeamName}\r" +
                            $"TEAM 2: {_validation.TeamTwo.TeamName}\r" +
                            $"MATCH GUID: {_newentry.Guid}\r\r" +
                            $"Note: Please use the MATCH GUID when creating new Player Stats.");
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                        $"Error message:\r" +
                        $"{_validation.ValidationErrorMsg}");
                    }
                    break;
                case "modal_updatematchresult":
                    _validation.ValidateUpdateMatchResult(modal);
                    if(_validation.IsRequestValid)
                    {
                        _update.UpdateMatchResult(_validation.TeamOne,
                            _validation.TeamTwo,
                            _validation.MatchGUID,
                            _validation.TeamOneMapsWon,
                            _validation.TeamTwoMapsWon,
                            _validation.MatchResult.Id);
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"Match Result Updated.\r" +
                            $"Match GUID: {_validation.MatchGUID}");
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                        $"Error message:\r" +
                        $"{_validation.ValidationErrorMsg}");
                    }
                    break;
                case "modal_newplayerstats":
                    _validation.ValidateNewPlayerStats(modal);
                    if(_validation.IsRequestValid)
                    {
                        _newentry.AddNewPlayerStats("PlayerStats", _validation.GUID, _validation.Player, _validation.Stats, _validation.MapMode);
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                        $"Player stats have been recorded.\r\r" +
                        $"Match GUID: {_validation.GUID}\r" +
                        $"Player Name: {_validation.Player.ActivsionId}\r" +
                        $"Map & Mode: {_validation.MapMode.Map}, {_validation.MapMode.Mode}\r\r" +
                        $"Kills: {_validation.Stats.Kills}\r" +
                        $"Deaths: {_validation.Stats.Deaths}\r" +
                        $"HillTime: {_validation.Stats.HillTime}\r" +
                        $"BombsPlanted: {_validation.Stats.BombsPlanted}\r" +
                        $"ObjectiveKills: {_validation.Stats.ObjKills}");
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                        $"Error message:\r" +
                        $"{_validation.ValidationErrorMsg}");
                    }
                    break;
                case "modal_searchplayer":
                    var searchPlayerModal = modal.Data.CustomId;
                    var searchPlayerComponents = modal.Data.Components.ToList();

                    string activisionId = searchPlayerComponents
                        .First(x => x.CustomId == "search_player").Value;

                    PlayerModel player = _search.SearchPlayer(activisionId);
                    PlayerStatTotalsModel playerStats = _search.SearchPlayerStatTotals(activisionId);

                    if(player == null)
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"Error: Could not find player by name: {activisionId}");
                    }
                    else if (playerStats == null)
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"Error retrieving stats for player {activisionId}");
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r\r" +
                            $"Player Information:\r" +
                            $"Id: {player.Id}\r" +
                            $"Activision Id: {player.ActivsionId}\r" +
                            $"Discord Name: {player.DiscordName}\r" +
                            $"Twitter: {player.Twitter}\r" +
                            $"Date Created: {player.DateCreated}\r" +
                            $"Last Updated: {player.LastUpdated}\r" +
                            $"Active: {player.Active}\r\r" +
                            $"Player Stats:\r" +
                            $"Kills: {playerStats.Kills}\r" +
                            $"Deaths: {playerStats.Deaths}\r" +
                            $"Hardpoint Hill Time: {playerStats.HillTime}\r" +
                            $"Bombs Planted: {playerStats.BombsPlanted}\r" +
                            $"Objective Kills (Control): {playerStats.ObjKills}\r" +
                            $"K/D Ratio: {playerStats.KdRatio}");
                    }
                    break;
                case "modal_searchteam":
                    var searchTeamModel = modal.Data.CustomId;
                    var searchTeamComponents = modal.Data.Components.ToList();

                    string teamName = searchTeamComponents
                        .First(x => x.CustomId == "search_team").Value;

                    TeamModel team = _search.SearchTeam(teamName);
                    RosterModel roster = _search.SearchRosterSingle(teamName);

                    if (team == null)
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"Could not find team by name: {teamName}");
                    }
                    else if (roster == null)
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"Could not find roster by name: {teamName}");
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r\r" +
                            $"Team Information:\r" +
                            $"Id: {team.Id}\r" +
                            $"Team Name: {team.TeamName}\r" +
                            $"Twitter: {team.Twitter}\r" +
                            $"Group: {team.Group}\r" +
                            $"Date Created: {team.DateCreated}\r" +
                            $"Last Updated: {team.LastUpdated}\r" +
                            $"Active: {team.Active}\r\r" +
                            $"Roster Information:\r" +
                            $"Player 1: {roster.PlayerOne}\r" +
                            $"Player 2: {roster.PlayerTwo}\r" +
                            $"Player 3: {roster.PlayerThree}\r" +
                            $"Player 4: {roster.PlayerFour}\r" +
                            $"Player 5: {roster.PlayerFive}\r" +
                            $"Player 6: {roster.PlayerSix}");
                    }
                    break;
                case "modal_searchMatchResult":
                    var searchMatchResult = modal.Data.CustomId;
                    var searchMatchResultComponents = modal.Data.Components.ToList();

                    string userInputGuid = searchMatchResultComponents
                        .First(x => x.CustomId == "modal_searchMatchResult").Value;

                    MatchResultModel matchResult = _search.SearchMatchResult(userInputGuid);

                    if(matchResult == null)
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r" +
                            $"Could not find match by GUID: {userInputGuid}");
                    }
                    else
                    {
                        await modal.RespondAsync($"{modal.User.Mention}\r\r" +
                            $"Id: {matchResult.Id}\r" +
                            $"GUID: {matchResult.GUID}\r" +
                            $"TEAM 1 NAME: {matchResult.TeamOneName}\r" +
                            $"Maps Won: {matchResult.TeamOneMW}\r" +
                            $"TEAM 2 NAME: {matchResult.TeamTwoName}\r" +
                            $"Maps Won: {matchResult.TeamTwoMW}");
                    }

                    break;
                case "modal_feedback":
                    _feedback.AddFeedback(modal);
                    await modal.RespondAsync($"{modal.User.Mention} Feedback Submitted.\r" +
                        $"Thank you for your valuable feedback.");
                    break;
                case "modal_freeagent":
                    var fa_modal = modal.Data.CustomId;
                    var fa_components = modal.Data.Components.ToList();

                    string type = fa_components
                        .First(x => x.CustomId == "fa_type").Value;

                    string age = fa_components
                        .First(x => x.CustomId == "fa_age").Value;

                    string role = fa_components
                        .First(x => x.CustomId == "fa_role").Value;

                    string region = fa_components
                        .First(x => x.CustomId == "fa_regiontimezone").Value;

                    string other = fa_components
                        .First(x => x.CustomId == "fa_other").Value;

                    await modal.RespondAsync($"{modal.User.Mention}\rFree Agent Posting:\r\r" +
                        $"Type: {type.ToUpper()}\r" +
                        $"Age: {age}\r" +
                        $"Role: {role.ToUpper()}\r" +
                        $"Region & TimeZone: {region.ToUpper()}\r\r" +
                        $"Other Notes:\r" +
                        $"{other}");
                    break;
            }
        }
    }
}
