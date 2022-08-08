using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GhidorahBot.Database;
using GhidorahBot.Extensions;
using GhidorahBot.Models;
using Google.Apis.Sheets.v4;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace GhidorahBot.Validation
{
    public class DataValidation
    {
        private Search _search { get; set; }
        private Update _update { get; set; }
        private NewEntry _newEntry { get; set; }
        private Feedback _feedback { get; set; }
        private DiscordSocketClient _client { get; set; }
        private SheetsService _service;
        private IConfiguration _config;

        public string RespondMessage = "";
        public ulong ChannelId;
        public bool IsAdminRequestValid;

        private int _id = 0;
        private static int _maxSlots = 6;
        private int _unavailableSlots = 0;
        private bool _isCaptain, _isManager, _isLeagueStaff;

        //Roster Variables
        private bool _slotsAvailable;
        private List<string> _userInputList = new List<string>();
        private List<string> _invalidPlayerList = new List<string>();
        private List<PlayerModel> _playerOnRosterList = new List<PlayerModel>();
        private List<PlayerModel> _removePlayerList = new List<PlayerModel>();
        private List<PlayerModel> _addPlayerList = new List<PlayerModel>();

        //SheetNames
        private static readonly string _rosterSheet = "Roster";
        private static readonly string _teamSheet = "Team";
        private static readonly string _matchResultSheet = "MatchResult";
        private static readonly string _playerSheet = "Player";
        private static readonly string _playerStatsSheet = "PlayerStats";
        private static readonly string _feedbackSheet = "Feedback";
        private static readonly string _registerAdminChnl = "AdminChannel";

        public DataValidation(Search search, Update update, NewEntry newentry, Feedback feedback, IConfiguration config, SheetsService service, DiscordSocketClient client)
        {
            _config = config;
            _service = service;
            _search = search;
            _update = update;
            _newEntry = newentry;
            _feedback = feedback;
            _client = client;
        }

        #region Team
        public void ValidateNewTeam(SocketModal modal, SocketInteractionContext ctx)
        {
            ValidateDiscordUser(ctx);
            if(_isLeagueStaff)
            {
                string createdBy = $"{modal.User.Username}#{modal.User.Discriminator}";

                List<object> newTeamList;
                bool response;
                IncrementId(_teamSheet);

                var modalName = modal.Data.CustomId;
                var components = modal.Data.Components.ToList();

                string teamName = components
                    .First(x => x.CustomId == "team_Name").Value;

                string twitter = components
                    .First(x => x.CustomId == "team_twitter").Value;

                string group = components
                    .First(x => x.CustomId == "team_group").Value;

                response = group.Any(x => !char.IsLetter(x));
                if (response)
                {
                    RespondMessage = "Group must be a letter A-Z";
                    return;
                }

                string captain = components
                    .First(x => x.CustomId == "team_captain").Value;
                //var captainRole = ctx.Guild.Roles.FirstOrDefault(x => x.Name == "Team Captain");
                //if (!string.IsNullOrWhiteSpace(captain))
                //{
                //    if (captainRole != null)
                //    {
                //        AddRole(captain, ctx, captainRole);
                //    }
                //    else
                //    {
                //        RespondMessage = $"{modal.User.Mention}\r" +
                //            $"'Team Captain' discord role could not be found.";
                //        return;
                //    }
                //}

                string manager = components
                    .First(x => x.CustomId == "team_manager").Value;
                //var managerRole = ctx.Guild.Roles.FirstOrDefault(x => x.Name == "Team Manager");
                //if (!string.IsNullOrWhiteSpace(manager))
                //{
                //    if (managerRole != null)
                //    {
                //        AddRole(manager, ctx, managerRole);
                //    }
                //    else
                //    {
                //        RespondMessage = $"{modal.User.Mention}\r" +
                //            $"'Team Manager' discord role could not be found.";
                //        return;
                //    }
                //}

                var teamSearch = _search.SearchTeam(teamName);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    newTeamList = new List<object>()
                {
                    _id,
                    teamName.ToUpper(),
                    twitter,
                    group.ToUpper(),
                    captain,
                    manager,
                    "",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    createdBy
                };

                    _newEntry.AddNewTeam(newTeamList, _teamSheet);
                    if (string.IsNullOrWhiteSpace(_newEntry.NewEntryResponseMessage))
                    {
                        RespondMessage = $"{modal.User.Mention}\r" +
                            $"New Team has been added successfully\r\r" +
                            $"Team Name: {teamName.ToUpper()}\r" +
                            $"Team Id: {_id}";
                    }
                    else
                    {
                        RespondMessage = $"{modal.User.Mention}\r" +
                            $"NewEntry Exception:\r{_newEntry.NewEntryResponseMessage}";
                    }
                }
            }
            else
            {
                RespondMessage = "You do not have permission you use this command.";
            }  
        }
        public void ValidateUpdateTeam(SocketModal modal, SocketInteractionContext ctx)
        {
            ValidateDiscordUser(ctx);
            if(_isLeagueStaff)
            {
                string captainEdit = string.Empty;
                string managerEdit = string.Empty;
                var updateTeamEntry = new List<UpdateTeamModel>();

                var modalName = modal.Data.CustomId;
                var components = modal.Data.Components.ToList();

                string searchTeamName = components
                .First(x => x.CustomId == "search_team_Name").Value;
                var teamSearch = _search.SearchTeam(searchTeamName);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\rTeam: {searchTeamName} does NOT exist. Please create a new team.";
                    return;
                }

                string teamName = components
                .First(x => x.CustomId == "edit_team_Name").Value;

                string twitter = components
                .First(x => x.CustomId == "edit_team_twitter").Value;

                string group = components
                .First(x => x.CustomId == "edit_team_group").Value;

                string teamLeads = components
                .First(x => x.CustomId == "edit_team_leads").Value;

                string[] teamLeadsArray = teamLeads.Split(",");
                try
                {
                    if (!string.IsNullOrWhiteSpace(teamLeadsArray[0]))
                    {
                        captainEdit = teamLeadsArray[0];
                        //var captainRole = ctx.Guild.Roles.FirstOrDefault(x => x.Name == "Team Captain");
                        //if (captainRole != null)
                        //{
                        //    RemoveRole(teamSearch.Captain, ctx, captainRole);
                        //    AddRole(captainEdit, ctx, captainRole);
                        //}
                        //else
                        //{
                        //    RespondMessage = $"{modal.User.Mention}\r" +
                        //        $"'Team Captain' discord role could not be found.";
                        //    return;
                        //}
                    }
                }
                catch (Exception ex)
                {
                    //
                }

                try
                {
                    if (!string.IsNullOrWhiteSpace(teamLeadsArray[1]))
                    {
                        managerEdit = teamLeadsArray[1];
                        //var managerRole = ctx.Guild.Roles.FirstOrDefault(x => x.Name == "Team Manager");
                        //if (managerRole != null)
                        //{
                        //    RemoveRole(teamSearch.Manager, ctx, managerRole);
                        //    AddRole(managerEdit, ctx, managerRole);
                        //}
                        //else
                        //{
                        //    RespondMessage = $"{modal.User.Mention}\r" +
                        //        $"'Team Manager' discord role could not be found.";
                        //    return;
                        //}
                    }
                }
                catch (Exception ex)
                {
                    //
                }

                if (teamSearch != null)
                {
                    updateTeamEntry.Add(new UpdateTeamModel(teamName.ToUpper(), twitter.ToUpper(), group.ToUpper(), captainEdit.ToUpper(), managerEdit.ToUpper()));
                    _update.UpdateTeam(updateTeamEntry, teamSearch.Id, _teamSheet);

                    if (string.IsNullOrWhiteSpace(_update.UpdateResponseMessage))
                    {
                        RespondMessage = $"{modal.User.Mention}\r" +
                            $"Team updated successfully.\r" +
                            $"Updated Information (blank items were not updated):\r" +
                            $"Team Name: {updateTeamEntry[0].TeamName}\r" +
                            $"Twitter: {updateTeamEntry[0].Twitter}\r" +
                            $"Group: {updateTeamEntry[0].Group}\r" +
                            $"Captain: {updateTeamEntry[0].Captain}\r" +
                            $"Manager: {updateTeamEntry[0].Manager}";
                    }
                    else
                    {
                        RespondMessage = $"{modal.User.Mention}\r" +
                            $"{_update.UpdateResponseMessage}";
                    }
                }
                else
                {
                    RespondMessage = $"{modal.User.Mention}\rTeam: {teamName} does NOT exist. Please create a new team.";
                    return;
                }
            }
            else
            {
                RespondMessage = "You do not have permission you use this command.";
            }         
        }

        public void ValidateTeamSearch(SocketModal modal)
        {
            var searchTeamModel = modal.Data.CustomId;
            var searchTeamComponents = modal.Data.Components.ToList();

            string teamName = searchTeamComponents
                .First(x => x.CustomId == "search_team").Value;

            TeamModel team = _search.SearchTeam(teamName);
            if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
            {
                RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                return;
            }

            TeamStatTotalsModel teamStats = _search.SearchTeamStatTotals(team.TeamName);
            if(!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
            {
                RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                return;
            }
            
            RosterModel roster = _search.SearchRosterSingle(teamName);
            if(!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
            {
                RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                return;
            }

            RespondMessage = $"{modal.User.Mention}\r\r" +
                $"Team Information:\r" +
                $"Id: {team.Id}\r" +
                $"Team Name: {team.TeamName}\r" +
                $"Twitter: {team.Twitter}\r" +
                $"Group: {team.Group}\r" +
                $"Date Created: {team.DateCreated}\r" +
                $"Last Updated: {team.LastUpdated}\r\r" +
                $"Roster Information:\r" +
                $"Player 1: {roster.PlayerOne}\r" +
                $"Player 2: {roster.PlayerTwo}\r" +
                $"Player 3: {roster.PlayerThree}\r" +
                $"Player 4: {roster.PlayerFour}\r" +
                $"Player 5: {roster.PlayerFive}\r" +
                $"Player 6: {roster.PlayerSix}\r\r" +
                $"Team Stats:\r" +
                $"Maps Won: {teamStats.TotalMapsWon}\r" +
                $"Maps Lost: {teamStats.TotalMapsLost}\r" +
                $"Map Win %: {teamStats.MapWinPercentage}\r" +
                $"Matches Won: {teamStats.MatchWins}\r" +
                $"Matches Lost: {teamStats.MatchLoses}\r" +
                $"Total Matches Played: {teamStats.TotalMatches}\r" +
                $"Match Win %: {teamStats.MatchWinPercentage}";
        }
        #endregion

        #region Player
        public void ValidateNewPlayer(SocketModal modal, SocketInteractionContext ctx)
        {
            ValidateDiscordUser(ctx);
            if(_isLeagueStaff)
            {
                string createdBy = $"{modal.User.Username}#{modal.User.Discriminator}";
                IncrementId(_playerSheet);
                var modalName = modal.Data.CustomId;
                var components = modal.Data.Components.ToList();

                string activisionId = components
                .First(x => x.CustomId == "player_activ_id").Value;

                string discordName = components
                .First(x => x.CustomId == "player_discord_name").Value;

                string twitter = components
                .First(x => x.CustomId == "player_twitter").Value;

                var playerSearch = _search.SearchPlayer(activisionId);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    var newPlayerList = new List<object>()
                    {
                        _id,
                        activisionId.ToUpper(),
                        discordName,
                        twitter,
                        "",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        createdBy
                    };

                    _newEntry.AddNewPlayer(newPlayerList, _playerSheet);
                    if (string.IsNullOrWhiteSpace(_newEntry.NewEntryResponseMessage))
                    {
                        RespondMessage = $"{modal.User.Mention}\r" +
                                $"New Player has been added successfully\r\r" +
                                $"Activision Id: {activisionId.ToUpper()}\r" +
                                $"Player Id: {_id}";
                    }
                    else
                    {
                        RespondMessage = $"{modal.User.Mention}\r" +
                            $"NewEntry Exception:\r{_newEntry.NewEntryResponseMessage}";
                    }
                }
            }
            else
            {
                RespondMessage = "You do not have permission you use this command.";
            }
            
        }
        public void ValidateUpdatePlayer(SocketModal modal, SocketInteractionContext ctx)
        {
            ValidateDiscordUser(ctx);
            if(_isLeagueStaff)
            {
                var updatePlayerEntry = new List<UpdatePlayerModel>();
                var modalName = modal.Data.CustomId;
                var components = modal.Data.Components.ToList();

                string activisionId = components
                .First(x => x.CustomId == "edit_player_activ_id").Value;

                string discordName = components
                .First(x => x.CustomId == "edit_player_discord_name").Value;

                string twitter = components
                .First(x => x.CustomId == "edit_player_twitter").Value;

                string active = components
                .First(x => x.CustomId == "edit_player_active").Value;

                var playerSearch = _search.SearchPlayer(activisionId);
                if (string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    if (playerSearch != null)
                    {
                        updatePlayerEntry.Add(new UpdatePlayerModel(activisionId.ToUpper(), discordName.ToUpper(), twitter.ToUpper(), active.ToUpper()));
                        _update.UpdatePlayer(updatePlayerEntry, playerSearch.Id, _playerSheet);
                        if (string.IsNullOrWhiteSpace(_update.UpdateResponseMessage))
                        {
                            RespondMessage = $"{modal.User.Mention}\r" +
                                $"Player updated successfully.\r" +
                                $"Updated Information (blank items were not updated):\r" +
                                $"Activision Id: {updatePlayerEntry[0].ActivisionId}\r" +
                                $"Discord: {updatePlayerEntry[0].DiscordName}\r" +
                                $"Twitter: {updatePlayerEntry[0].Twitter}\r" +
                                $"Active: {updatePlayerEntry[0].Active}";
                        }
                        else
                        {
                            RespondMessage = $"{modal.User.Mention}\r" +
                                $"{_update.UpdateResponseMessage}";
                        }
                    }
                    else
                    {
                        RespondMessage = $"{modal.User.Mention}\rPlayer: {activisionId} does NOT exist. Please create a new player.";
                        return;
                    }
                }
                else
                {
                    RespondMessage = $"{modal.User.Mention}\rPlayer: {activisionId} does NOT exist. Please create a new player.";
                }
            }
            else
            {
                RespondMessage = "You do not have permission you use this command.";
            }
        }
        public void ValidatePlayerSearch(SocketModal modal)
        {
            var searchPlayerModal = modal.Data.CustomId;
            var searchPlayerComponents = modal.Data.Components.ToList();

            string activisionId = searchPlayerComponents
                .First(x => x.CustomId == "search_player").Value;

            PlayerModel player = _search.SearchPlayer(activisionId);
            if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
            {
                RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                return;
            }
            
            PlayerStatTotalsModel playerStats = _search.SearchPlayerStatTotals(player.ActivsionId);
            if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
            {
                RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                return;
            }

            RosterModel roster = _search.SearchRosterForPlayer(player.ActivsionId);
            if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
            {
                RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                return;
            }

            RespondMessage = $"{modal.User.Mention}\r" +
                $"Player Information:\r" +
                $"Current Team: {roster.TeamName}\r" +
                $"Player Id: {player.Id}\r" +
                $"Activision Id: {player.ActivsionId}\r" +
                $"Discord: {player.DiscordName}\r" +
                $"Twitter: {player.Twitter}\r" +
                $"Last Updated: {player.LastUpdated}\r" +
                $"Date Created: {player.DateCreated}\r\r" +
                $"Player Stats Total:\r" +
                $"Kills: {playerStats.Kills}\r" +
                $"Deaths: {playerStats.Deaths}\r" +
                $"Hardpoint Hill Time: {playerStats.HillTime}\r" +
                $"Bombs Planted (SnD): {playerStats.BombsPlanted}\r" +
                $"Objective Kills (Control): {playerStats.ObjKills}\r" +
                $"K/D Ratio: {playerStats.KdRatio}";
        }
        #endregion

        #region Roster
        public void ValidateUpdateRoster(SocketModal modal, SocketInteractionContext ctx)
        {
            ValidateDiscordUser(ctx);
            if(_isLeagueStaff || _isCaptain || _isManager)
            {
                List<string> removePlayerList = new List<string>();
                List<string> addPlayerList = new List<string>();

                var modalName = modal.Data.CustomId;
                var components = modal.Data.Components.ToList();

                string teamName = components
                .First(x => x.CustomId == "updateroster_search_team_name").Value;

                string removePlayer = components
                .First(x => x.CustomId == "updateroster_removeplayer").Value;

                string addPlayer = components
                .First(x => x.CustomId == "updateroster_addplayer").Value;

                var rosterTeam = _search.SearchRosterSingle(teamName);
                if (string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    if (rosterTeam != null)
                    {
                        if (!string.IsNullOrWhiteSpace(removePlayer))
                        {
                            removePlayerList = removePlayer.Split(",").ToList();
                            removePlayerList = removePlayerList.ConvertAll(x => x.ToUpper());
                        }
                        else
                        {
                            RespondMessage = $"{modal.User.Mention}\rError creating RemovePlayerList.";
                            return;
                        }

                        if (_removePlayerList.Count > 6)
                        {
                            RespondMessage += "You can only remove up to 6 players from a roster.";
                            return;
                        }

                        if (!string.IsNullOrWhiteSpace(addPlayer))
                        {
                            addPlayerList = addPlayer.Split(",").ToList();
                            addPlayerList = addPlayerList.ConvertAll(x => x.ToUpper());
                        }
                        else
                        {
                            RespondMessage = $"{modal.User.Mention}\rError creating AddPlayerList.";
                            return;
                        }

                        if (_addPlayerList.Count > 6)
                        {
                            RespondMessage = "You can only have a maximum of 6 players per roster.";
                            return;
                        }

                        CreatePlayerList(removePlayerList, _removePlayerList);
                        CreatePlayerList(addPlayerList, _addPlayerList);

                        if (_invalidPlayerList.Any())
                        {
                            foreach (var player in _invalidPlayerList)
                            {
                                RespondMessage = $"{modal.User.Mention}\rThe players listed below could not be found.\r" +
                                    $"These players will need to be added as new players before you can assign them to a Roster.\r\r";
                            }

                            return;
                        }
                        else
                        {
                            var fullRosterList = _search.GetRosterIdsList();
                            IsPlayerOnRoster(fullRosterList, _removePlayerList);
                            IsPlayerOnRoster(fullRosterList, _addPlayerList);

                            if (_playerOnRosterList.Any())
                            {
                                RespondMessage = $"{modal.User.Mention}\rThe players listed below are already assigned to a Roster.\r";
                                foreach (var player in _playerOnRosterList)
                                {
                                    RespondMessage += $"{player.ActivsionId}";
                                }

                                return;
                            }
                            else
                            {
                                CalculateRosterSlots(_removePlayerList, _addPlayerList);

                                if (_slotsAvailable)
                                {
                                    _update.UpdateRoster(_removePlayerList, _addPlayerList, rosterTeam, _rosterSheet);
                                }
                            }
                        }
                    }
                    else
                    {
                        RespondMessage = $"{modal.User.Mention}\rRoster: {teamName} does NOT exist. Please ask an administrator for assistance.";
                        return;
                    }
                }
                else
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                }
            }
            else
            {
                RespondMessage = "You do not have permission you use this command.";
            }           
        }
        private void CalculateRosterSlots(List<PlayerModel> removePlayerList, List<PlayerModel> addPlayerList)
        {
            int openSlots = _maxSlots - _unavailableSlots;
            int slotsRequired = openSlots - (removePlayerList.Count + addPlayerList.Count);

            if (slotsRequired < 0)
            {
                RespondMessage = $"Open Roster spots {openSlots}.\r" +
                    $"Roster spots pending {Math.Abs(slotsRequired)}.\r" +
                    $"A Roster can have a maximum of 6 total players.";
                _slotsAvailable = false;
            }
            else
            {
                _slotsAvailable = true;
            }
        }
        private void IsPlayerOnRoster(List<RosterModel> rosterList, List<PlayerModel> playerList)
        {
            _playerOnRosterList.Clear();

            foreach(RosterModel roster in rosterList)
            {
                foreach (var player in playerList)
                {
                    if (player.Id.EqualsAnyOf(
                        roster.PlayerOne,
                        roster.PlayerTwo,
                        roster.PlayerThree,
                        roster.PlayerFour,
                        roster.PlayerFive,
                        roster.PlayerSix))
                    {
                        _playerOnRosterList.Add(player);
                    }
                }
            }
        }
        #endregion


        #region MatchResult
        public void ValidateNewMatchResult(SocketModal modal, SocketInteractionContext ctx)
        {
            ValidateDiscordUser(ctx);
            if(_isLeagueStaff || _isCaptain || _isManager)
            {
                List<object> newMatchResultList = new List<object>();
                int teamOne_MapsWonInt = 0;
                int teamTwo_MapsWonInt = 0;
                bool response = false;

                var modalName = modal.Data.CustomId;
                var components = modal.Data.Components.ToList();

                string teamOneName = components
                .First(x => x.CustomId == "team_1_name").Value;

                var teamOne = _search.SearchTeam(teamOneName);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                    return;
                }

                string teamOneMapsWon = components
                .First(x => x.CustomId == "team_1_maps_won").Value;

                response = int.TryParse(teamOneMapsWon, out teamOne_MapsWonInt);
                if (!response)
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"Please enter a number for Maps won.";
                    return;
                }

                string teamTwoName = components
                .First(x => x.CustomId == "team_2_name").Value;

                var teamTwo = _search.SearchTeam(teamTwoName);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                    return;
                }

                string teamTwoMapsWon = components
                .First(x => x.CustomId == "team_2_maps_won").Value;

                response = int.TryParse(teamTwoMapsWon, out teamTwo_MapsWonInt);
                if (!response)
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"Please enter a number for Maps won.";
                    return;
                }

                string group = components
                .First(x => x.CustomId == "group_id").Value;

                response = group.Any(x => !char.IsLetter(x));
                if (response)
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        "Group must be a letter A-Z";
                    return;
                }

                IncrementId(_matchResultSheet);
                Guid guid = Guid.NewGuid();

                if (teamOne_MapsWonInt >= 3)
                {
                    newMatchResultList = new List<object>()
                {
                    _id,
                    guid,
                    teamOne.Id,
                    "=IFERROR(VLOOKUP(C:C, Team!A:B, 2, false), \"\")",
                    teamOne_MapsWonInt,
                    teamTwo_MapsWonInt,
                    teamTwo.Id,
                    "=IFERROR(VLOOKUP(G:G, Team!A:B, 2, false), \"\")",
                    teamTwo_MapsWonInt,
                    teamOne_MapsWonInt,
                    group.ToUpper(),
                    $"=IFERROR(D{_id++}, \"\")",
                    $"=IFERROR(H{_id++}, \"\")",
                    "",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                }
                else if (teamTwo_MapsWonInt >= 3)
                {
                    newMatchResultList = new List<object>()
                {
                    _id,
                    guid,
                    teamOne.Id,
                    "=IFERROR(VLOOKUP(C:C, Team!A:B, 2, false), \"\")",
                    teamOne_MapsWonInt,
                    teamTwo_MapsWonInt,
                    teamTwo.Id,
                    "=IFERROR(VLOOKUP(G:G, Team!A:B, 2, false), \"\")",
                    teamTwo_MapsWonInt,
                    teamOne_MapsWonInt,
                    group.ToUpper(),
                    $"=IFERROR(H{_id++}, \"\")",
                    $"=IFERROR(D{_id++}, \"\")",
                    "",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                }

                _newEntry.AddNewMatchResult(newMatchResultList, _matchResultSheet);
                if (string.IsNullOrWhiteSpace(_newEntry.NewEntryResponseMessage))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"Match Recorded:\r\r" +
                        $"TEAM 1: {teamOneName.ToUpper()}\r" +
                        $"TEAM 2: {teamTwoName.ToUpper()}\r" +
                        $"MATCH GUID: {guid}\r\r" +
                        $"NOTE: If you have player stats that need to be submitted for this match.\r" +
                        $"Please use the match guid above to link the player stats to this match!";
                }
                else
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"NewEntry Exception:\r{_newEntry.NewEntryResponseMessage}";
                }
            }
            else
            {
                RespondMessage = "You do not have permission you use this command.";
            }          
        }
        public void ValidateUpdateMatchResult(SocketModal modal, SocketInteractionContext ctx)
        {
            ValidateDiscordUser(ctx);
            if(_isLeagueStaff)
            {
                int teamOne_MapsWonInt = 0;
                int teamTwo_MapsWonInt = 0;
                string winner = string.Empty;
                string loser = string.Empty;
                bool response = false;

                var modalName = modal.Data.CustomId;
                var components = modal.Data.Components.ToList();

                string guid = components
                .First(x => x.CustomId == "updatematch_searchmatchGUID").Value;

                var match = _search.SearchMatchResult(guid);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"{guid} - {_search.SearchExceptionMsg}";
                }

                string teamOneName = components
                .First(x => x.CustomId == "updatematch_team_1_name").Value;

                var teamOne = _search.SearchTeam(teamOneName);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"{teamOneName} - {_search.SearchExceptionMsg}";
                    return;
                }

                string teamOneMapsWon = components
                .First(x => x.CustomId == "updatematch_team_1_maps_won").Value;

                response = int.TryParse(teamOneMapsWon, out teamOne_MapsWonInt);
                if (!response)
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"Please enter a number for Maps won.";
                    return;
                }

                string teamTwoName = components
                .First(x => x.CustomId == "updatematch_team_2_name").Value;

                var teamTwo = _search.SearchTeam(teamTwoName);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"{teamTwoName} - {_search.SearchExceptionMsg}";
                    return;
                }

                string teamTwoMapsWon = components
                .First(x => x.CustomId == "updatematch_team_2_maps_won").Value;

                response = int.TryParse(teamTwoMapsWon, out teamTwo_MapsWonInt);
                if (!response)
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"Please enter a number for Maps won.";
                    return;
                }

                IncrementId(_matchResultSheet);

                if (teamOne_MapsWonInt >= 3)
                {
                    winner = $"=IFERROR(D{_id++}, \"\")";
                    loser = $"=IFERROR(H{_id++}, \"\")";
                }
                else if (teamTwo_MapsWonInt >= 3)
                {
                    winner = $"=IFERROR(H{_id++}, \"\")";
                    loser = $"=IFERROR(D{_id++}, \"\")";
                }

                _update.UpdateMatchResult(teamOne, teamTwo, teamOne_MapsWonInt, teamTwo_MapsWonInt, _matchResultSheet, winner, loser, _id);
                if (!string.IsNullOrWhiteSpace(_update.UpdateResponseMessage))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_update.UpdateResponseMessage}";
                }
            }
            else
            {
                RespondMessage = "You do not have permission you use this command.";
            }        
        }
        public void ValidateMatchResultSearch(SocketModal modal)
        {
            var searchMatchResult = modal.Data.CustomId;
            var searchMatchResultComponents = modal.Data.Components.ToList();

            string userInputGuid = searchMatchResultComponents
                .First(x => x.CustomId == "modal_searchMatchResult").Value;

            MatchResultModel matchResult = _search.SearchMatchResult(userInputGuid);
            if(!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
            {
                RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                return;
            }

            RespondMessage = $"{modal.User.Mention}\r" +
                $"GUID: {matchResult.GUID}\r" +
                $"TEAM 1 NAME: {matchResult.TeamOneName}\r" +
                $"Maps Won: {matchResult.TeamOneMW}\r" +
                $"TEAM 2 NAME: {matchResult.TeamTwoName}\r" +
                $"Maps Won: {matchResult.TeamTwoMW}";
        }
        #endregion

        public void ValidateFeedback(SocketModal modal)
        {
            IncrementId(_feedbackSheet);
            _feedback.AddFeedback(modal, _feedbackSheet, _id);
            if(!string.IsNullOrWhiteSpace(_feedback.RespondFeedbackMessage))
            {
                RespondMessage = _feedback.RespondFeedbackMessage;
                return;
            }
            else
            {
                RespondMessage = $"{modal.User.Mention}\r" +
                $"Feedback submitted.\r" +
                $"Thank you for your valuable feedback.";
            }
        }
        public void ValidateFreeAgent(SocketModal modal)
        {
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

            RespondMessage = $"{modal.User.Mention}\rFree Agent Posting:\r\r" +
                $"Type: {type.ToUpper()}\r" +
                $"Age: {age}\r" +
                $"Role: {role.ToUpper()}\r" +
                $"Region & TimeZone: {region.ToUpper()}\r\r" +
                $"Other Notes:\r" +
                $"{other}";
        }
        public void ValidateNewPlayerStats(SocketModal modal, SocketInteractionContext ctx)
        {
            ValidateDiscordUser(ctx);
            if(_isLeagueStaff)
            {
                List<object> playerStatsList = new List<object>();
                StatsModel statsList;
                MapModel mapMatch;
                GamemodeModel gamemodeMatch;
                string map = string.Empty;
                string gamemode = string.Empty;
                var modalName = modal.Data.CustomId;
                var components = modal.Data.Components.ToList();
                string guid = components
                    .First(x => x.CustomId == $"newstats_search_matchguid").Value;

                var guidMatch = _search.SingleColumnSearch(_matchResultSheet, "B2:B", guid);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                    return;
                }

                string playerName = components
                    .First(x => x.CustomId == "newstats_playername").Value;

                var player = _search.SearchPlayer(playerName);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"Player: {playerName} - {_search.SearchExceptionMsg}";
                    return;
                }

                string mapMode = components
                .First(x => x.CustomId == "newstats_mapmode").Value;

                mapMode = Regex.Replace(mapMode, @"\s+", ",");
                string[] splitMapMode = mapMode.Split(",");

                if (string.IsNullOrWhiteSpace(splitMapMode[0]))
                {
                    RespondMessage = "Cannot complete request. Map is missing.";
                    return;
                }
                else if (string.IsNullOrWhiteSpace(splitMapMode[1]))
                {
                    RespondMessage = "Cannot complete request. Mode is missing.";
                    return;
                }
                else
                {
                    map = splitMapMode[0];
                    gamemode = splitMapMode[1];

                    mapMatch = _search.SearchMap(map);
                    if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                    {
                        RespondMessage = $"{modal.User.Mention}\r" +
                        $"{map.ToUpper()} - {_search.SearchExceptionMsg}";
                        return;
                    }

                    gamemodeMatch = _search.SearchGamemode(gamemode);
                    if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                    {
                        RespondMessage = $"{modal.User.Mention}\r" +
                        $"{gamemode.ToUpper()} - {_search.SearchExceptionMsg}";
                        return;
                    }
                }

                string stats = components
                .First(x => x.CustomId == "newstats_stats").Value;
                string[] splitStats = stats.Split(",");

                if (splitStats.Count() != 5)
                {
                    RespondMessage = "Cannot complete request. Missing required stats.";
                    return;
                }
                else
                {
                    statsList = new StatsModel(splitStats[0], splitStats[1], splitStats[2], splitStats[3], splitStats[4]);
                }

                IncrementId(_playerStatsSheet);
                playerStatsList = new List<object>()
            {
                    _id,
                    guid,
                    player.Id,
                    "=IFERROR(VLOOKUP(C:C, Player!A:B, 2, false), \"\")",
                    mapMatch.MapName,
                    gamemodeMatch.ModeName,
                    statsList.Kills,
                    statsList.Deaths,
                    statsList.HillTime,
                    statsList.BombsPlanted,
                    statsList.ObjKills,
                    "",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

                _newEntry.AddNewPlayerStats(playerStatsList, _playerStatsSheet);
                if (string.IsNullOrWhiteSpace(_newEntry.NewEntryResponseMessage))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"Player stats have been recorded.\r\r" +
                        $"TableId: {_id}" +
                        $"Match GUID: {guid}\r" +
                        $"Player Name: {player.ActivsionId.ToUpper()}\r" +
                        $"Map & Mode: {mapMatch.MapName.ToUpper()}, {gamemodeMatch.ModeName.ToUpper()}\r\r" +
                        $"Kills: {statsList.Kills}\r" +
                        $"Deaths: {statsList.Deaths}\r" +
                        $"HillTime: {statsList.HillTime}\r" +
                        $"BombsPlanted: {statsList.BombsPlanted}\r" +
                        $"ObjectiveKills: {statsList.ObjKills}";
                }
                else
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"NewEntry Exception:\r{_newEntry.NewEntryResponseMessage}";
                }
            }
            else
            {
                RespondMessage = "You do not have permission you use this command.";
            }         
        }

        private void ValidateDiscordUser(SocketInteractionContext ctx)
        {
            var captainRole = ctx.Guild.Roles.FirstOrDefault(x => x.Name == "Team Captain");
            var managerRole = ctx.Guild.Roles.FirstOrDefault(x => x.Name == "Team Manager");
            var leagueStaffRole = ctx.Guild.Roles.FirstOrDefault(x => x.Name == "League Staff");

            SocketGuildUser discordUser = ctx.User as SocketGuildUser;

            _isCaptain = false;
            _isManager = false;
            _isLeagueStaff = false;

            if (discordUser != null)
            {
                foreach (var role in discordUser.Roles)
                {
                    if (role == captainRole)
                    {
                        _isCaptain = true;
                    }

                    if (role == managerRole)
                    {
                        _isManager = true;
                    }

                    if (role == leagueStaffRole)
                    {
                        _isLeagueStaff = true;
                    }
                }
            }
            else
            {
                RespondMessage = "Error: Discord user is null or empty.";
            }
        }
        public void ValidateRegisterAdminChnl(SocketInteractionContext ctx, SocketModal modal)
        {
            ValidateDiscordUser(ctx);
            if(_isLeagueStaff)
            {
                List<object> itemList = new List<object>();
                ulong guildId = ctx.Guild.Id;
                string guildName = ctx.Guild.Name;

                var modalName = modal.Data.CustomId;
                var components = modal.Data.Components.ToList();

                string adminChnlId = components
                    .First(x => x.CustomId == "admin_chnl").Value;

                itemList = new List<object>()
                {
                    guildId.ToString(),
                    guildName,
                    adminChnlId.ToString()
                };

                _newEntry.RegisterNewAdminChnl(itemList, _registerAdminChnl);
                RespondMessage = $"{modal.User.Mention} Admin channel has been registered.\r" +
                    "You will now receive messages in that channel whenever a user submits a new admin request.";
            }
            else
            {
                RespondMessage = "You do not have permission you use this command.";
            }         
        }
        public void ValidateAdminRequest(SocketInteractionContext ctx, SocketModal modal)
        {
            ulong guildId = ctx.Guild.Id;

            var guildIdMatch = _search.SearchChannelTable(guildId);
            if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
            {
                IsAdminRequestValid = false;
                RespondMessage = $"{modal.User.Mention}\r" +
                    $"{_search.SearchExceptionMsg}";
                return;
            }
            else
            {
                IsAdminRequestValid = true;
                ChannelId = Convert.ToUInt64(guildIdMatch.ChannelId);
                RespondMessage = $"{modal.User.Mention}\r" +
                    $"Your request has been submitted!\r" +
                    $"An admin will contact you as soon as possible. Thank you for your patience.";
            }
        }
        private void IncrementId(string sheetName)
        {
            _id = 0;

            var range = $"{sheetName}!A2:A";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

            var reponse = request.ExecuteAsync();
            var requestResponse = reponse.Result;

            if (requestResponse != null && requestResponse.Values != null && requestResponse.Values.Count > 0)
            {
                var lastId = requestResponse.Values.Last();
                _id = Convert.ToInt32(lastId[0]);
                _id++;
            }
            else
            {
                _id++;
            }
        }

        //private async void AddRole(string discordName, SocketInteractionContext ctx, SocketRole role)
        //{
        //        //var userList = _client.Guilds.ToList()[0].Users.ToList();
        //        var userList = ctx.Guild.Users;
        //    SocketGuildUser discordUser;

        //    foreach (var user in userList)
        //    {
        //        var userFullName = $"{user.DisplayName}#{user.Discriminator}";
        //        if (userFullName.Equals(discordName))
        //        {
        //            discordUser = user;
        //            await (user as IGuildUser).AddRoleAsync(role);
        //        }
        //    }
        //}
        //private async void RemoveRole(string discordName, SocketInteractionContext ctx, SocketRole role)
        //{
        //    var userList = ctx.Guild.Users;
        //    SocketGuildUser discordUser;

        //    foreach (var user in userList)
        //    {
        //        var userFullName = $"{user.DisplayName}#{user.Discriminator}";
        //        if (userFullName.Equals(discordName))
        //        {
        //            discordUser = user;
        //            await (user as IGuildUser).RemoveRoleAsync(role);
        //        }
        //    }
        //}

        private void CreatePlayerList(List<string> playerList, List<PlayerModel> listToAdd)
        {
            foreach(var player in playerList)
            {
                var matchedPlayer = _search.SearchPlayer(player);

                if(matchedPlayer != null)
                {
                    listToAdd.Add(matchedPlayer);
                }
                else
                {
                    _invalidPlayerList.Add(player);
                }
            }
        }
    }
}
