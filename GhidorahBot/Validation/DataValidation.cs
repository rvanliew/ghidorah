using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GhidorahBot.Common;
using GhidorahBot.Database;
using GhidorahBot.Extensions;
using GhidorahBot.Models;
using GhidorahBot.Global;
using Google.Apis.Sheets.v4;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Discord.Commands;

namespace GhidorahBot.Validation
{
    public class DataValidation
    {
        private Search _search { get; set; }
        private Update _update { get; set; }
        private NewEntry _newEntry { get; set; }
        private Feedback _feedback { get; set; }
        private CreateCodLeague _createCodLeague { get; set; }
        private DiscordSocketClient _client { get; set; }
        private SheetsService _service;
        private IConfiguration _config;

        public string RespondMessage = "";
        public ulong ChannelId;
        public bool IsAdminRequestValid;
        public bool IsLeagueCreated;

        private int _id = 0;
        private bool _isCaptain, _isManager, _isLeagueStaff;

        //Roster Variables
        private bool _slotsAvailable;
        private List<string> _invalidPlayerList = new List<string>();

        private string _guildRosterSheet;
        private string _guildTeamSheet;
        private string _guildMatchResultSheet;
        private string _guildTeamStatsTotalsSheet;
        private string _guildPlayerSheet;
        private string _guildPlayerStatsSheet;
        private string _guildPlayerStatsTotalsSheet;

        public DataValidation(Search search,
            Update update,
            NewEntry newentry,
            Feedback feedback,
            IConfiguration config,
            SheetsService service,
            DiscordSocketClient client,
            CreateCodLeague createCodLeague)
        {
            _config = config;
            _service = service;
            _search = search;
            _update = update;
            _newEntry = newentry;
            _feedback = feedback;
            _client = client;
            _createCodLeague = createCodLeague;
        }

        public void SetGoogleSheetValues(SocketInteractionContext ctx)
        {
            string guildNameNoSpace = Regex.Replace(ctx.Guild.Name, @"\s+", "");

            _guildRosterSheet = $"{guildNameNoSpace}_{Values.GoogleSheets.roster}";
            _guildTeamSheet = $"{guildNameNoSpace}_{Values.GoogleSheets.team}";
            _guildMatchResultSheet = $"{guildNameNoSpace}_{Values.GoogleSheets.matchResult}";
            _guildTeamStatsTotalsSheet = $"{guildNameNoSpace}_{Values.GoogleSheets.teamStatTotal}";
            _guildPlayerSheet = $"{guildNameNoSpace}_{Values.GoogleSheets.player}";
            _guildPlayerStatsSheet = $"{guildNameNoSpace}_{Values.GoogleSheets.playerStats}";
            _guildPlayerStatsTotalsSheet = $"{guildNameNoSpace}_{Values.GoogleSheets.playerStatsTotal}";
        }

        #region Team
        public void ValidateNewTeam(SocketModal modal, SocketInteractionContext ctx)
        {
            ValidateDiscordUser(ctx);
            if(_isLeagueStaff)
            {
                List<SocketGuildUser> discordUserList = new List<SocketGuildUser>();
                DiscordUsers discordUser = new DiscordUsers();
                discordUserList = discordUser.DownloadAllUsers(ctx);

                string createdBy = $"{modal.User.Username}#{modal.User.Discriminator}";

                List<object> newTeamList;
                bool response;
                IncrementId($"{_guildTeamSheet}");

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
                    RespondMessage = $"{modal.User.Mention} Group must be a letter A-Z";
                    return;
                }

                string captain = components
                    .First(x => x.CustomId == "team_captain").Value;
                var captainRole = ctx.Guild.Roles.FirstOrDefault(x => x.Name == "Team Captain");
                if (!string.IsNullOrWhiteSpace(captain))
                {
                    if (captainRole != null)
                    {
                        AddRole(captain, captainRole, discordUserList);
                    }
                    else
                    {
                        RespondMessage = $"{modal.User.Mention}\r" +
                            $"'Team Captain' discord role could not be found.";
                        return;
                    }
                }

                string manager = components
                    .First(x => x.CustomId == "team_manager").Value;
                var managerRole = ctx.Guild.Roles.FirstOrDefault(x => x.Name == "Team Manager");
                if (!string.IsNullOrWhiteSpace(manager))
                {
                    if (managerRole != null)
                    {
                        AddRole(manager, managerRole, discordUserList);
                    }
                    else
                    {
                        RespondMessage = $"{modal.User.Mention}\r" +
                            $"'Team Manager' discord role could not be found.";
                        return;
                    }
                }

                var teamSearch = _search.SearchTeam(teamName, _guildTeamSheet);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    newTeamList = new List<object>()
                    {
                        _id,
                        teamName.ToUpper(),
                        twitter,
                        group.ToUpper(),
                        captain.ToUpper(),
                        manager.ToUpper(),
                        "",
                        "",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        createdBy
                    };

                    _newEntry.AddNewTeam(newTeamList, $"{_guildTeamSheet}");
                    if (string.IsNullOrWhiteSpace(_newEntry.NewEntryResponseMessage))
                    {
                        int gsRowId = _id + 1;
                        List<object> newTotalRow = new List<object>()
                        {
                            _id,
                            $"={_guildTeamSheet}!B{gsRowId}",
                            $"=VLOOKUP(A:A,{_guildTeamSheet}!A:G,4,false)",
                            $"=IFERROR(query({_guildMatchResultSheet}!A:O, \"select sum(E) where D='\"&$B{gsRowId}&\"' label sum(E) ''\", 0), 0)",
                            $"=IFERROR(query({_guildMatchResultSheet}!A:O, \"select sum(I) where H='\"&$B{gsRowId}&\"' label sum(I) ''\", 0),0)",
                            $"=IFERROR(query({_guildMatchResultSheet}!A:O, \"select sum(F) where D='\"&$B{gsRowId}&\"' label sum(F) ''\", 0), 0)",
                            $"=IFERROR(query({_guildMatchResultSheet}!A:O, \"select sum(J) where H='\"&$B{gsRowId}&\"' label sum(J) ''\", 0),0)",
                            $"=sum(D{gsRowId}:E{gsRowId})",
                            $"=sum(F{gsRowId}:G{gsRowId})",
                            $"=IFERROR(H{gsRowId}/(H{gsRowId}+I{gsRowId}),0)",
                            $"=countif(query({_guildMatchResultSheet}!L:L, \"select L where L='\"&$B{gsRowId}&\"'\", 1), B{gsRowId})",
                            $"=IFERROR(query({_guildMatchResultSheet}!M:M, \"select count(M) where M='\"&$B{gsRowId}&\"' or M='NA' label count(M) ''\"), 0)",
                            $"=K{gsRowId}+L{gsRowId}",
                            $"=IFERROR((K{gsRowId}/M{gsRowId}), 0)"
                        };

                        _newEntry.AddNewTeamTotalStatsRow(newTotalRow, $"{_guildTeamStatsTotalsSheet}");

                        List<object> newRosterRow = new List<object>()
                        {
                            _id,
                            $"={_guildTeamSheet}!B{gsRowId}",
                            "",
                            $"=IFERROR(VLOOKUP(C:C, {_guildPlayerSheet}!A:B, 2, false), \"\")",
                            "",
                            $"=IFERROR(VLOOKUP(E:E, {_guildPlayerSheet}!A:B, 2, false), \"\")",
                            "",
                            $"=IFERROR(VLOOKUP(G:G, {_guildPlayerSheet}!A:B, 2, false), \"\")",
                            "",
                            $"=IFERROR(VLOOKUP(I:I, {_guildPlayerSheet}!A:B, 2, false), \"\")",
                            "",
                            $"=IFERROR(VLOOKUP(K:K, {_guildPlayerSheet}!A:B, 2, false), \"\")",
                            "",
                            $"=IFERROR(VLOOKUP(M:M, {_guildPlayerSheet}!A:B, 2, false), \"\")",
                            "",
                            "",
                        };

                        _newEntry.AddNewRosterRow(newRosterRow, $"{_guildRosterSheet}");

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
                List<SocketGuildUser> discordUserList = new List<SocketGuildUser>();
                DiscordUsers discordUser = new DiscordUsers();
                discordUserList = discordUser.DownloadAllUsers(ctx);

                string createdBy = $"{modal.User.Username}#{modal.User.Discriminator}";

                string captainEdit = string.Empty;
                string managerEdit = string.Empty;
                var updateTeamEntry = new List<UpdateTeamModel>();

                var modalName = modal.Data.CustomId;
                var components = modal.Data.Components.ToList();

                string searchTeamName = components
                .First(x => x.CustomId == "search_team_Name").Value;
                var teamSearch = _search.SearchTeam(searchTeamName, _guildTeamSheet);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\rSearch Team Name: {searchTeamName} is incorrect or does not exist.";
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
                        var captainRole = ctx.Guild.Roles.FirstOrDefault(x => x.Name == "Team Captain");
                        if (captainRole != null)
                        {
                            RemoveRole(teamSearch.Captain, captainRole, discordUserList);
                            AddRole(captainEdit, captainRole, discordUserList);
                        }
                        else
                        {
                            RespondMessage = $"{modal.User.Mention}\r" +
                                $"'Team Captain' discord role could not be found.";
                            return;
                        }
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
                        var managerRole = ctx.Guild.Roles.FirstOrDefault(x => x.Name == "Team Manager");
                        if (managerRole != null)
                        {
                            RemoveRole(teamSearch.Manager, managerRole, discordUserList);
                            AddRole(managerEdit, managerRole, discordUserList);
                        }
                        else
                        {
                            RespondMessage = $"{modal.User.Mention}\r" +
                                $"'Team Manager' discord role could not be found.";
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //
                }

                if (teamSearch != null)
                {
                    updateTeamEntry.Add(new UpdateTeamModel(teamName.ToUpper(), twitter.ToUpper(), group.ToUpper(), captainEdit.ToUpper(), managerEdit.ToUpper()));
                    _update.UpdateTeam(updateTeamEntry, teamSearch.Id, $"{_guildTeamSheet}", createdBy);

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

            TeamModel team = _search.SearchTeam(teamName, _guildTeamSheet);
            if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
            {
                RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                return;
            }

            TeamStatTotalsModel teamStats = _search.SearchTeamStatTotals(team.TeamName, _guildTeamStatsTotalsSheet);
            if(!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
            {
                RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                return;
            }
            
            RosterModel roster = _search.SearchRosterSingle(teamName, _guildRosterSheet);
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
                IncrementId($"{_guildPlayerSheet}");
                var modalName = modal.Data.CustomId;
                var components = modal.Data.Components.ToList();

                string activisionId = components
                .First(x => x.CustomId == "player_activ_id").Value;

                string discordName = components
                .First(x => x.CustomId == "player_discord_name").Value;

                string twitter = components
                .First(x => x.CustomId == "player_twitter").Value;

                var playerSearch = _search.SearchPlayer(activisionId, _guildPlayerSheet);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    var newPlayerList = new List<object>()
                    {
                        _id,
                        activisionId.ToUpper(),
                        discordName.ToUpper(),
                        twitter,
                        "",
                        "",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        createdBy
                    };

                    _newEntry.AddNewPlayer(newPlayerList, $"{_guildPlayerSheet}");
                    if (string.IsNullOrWhiteSpace(_newEntry.NewEntryResponseMessage))
                    {
                        int gsRowId = _id + 1;
                        List<object> newTotalRow = new List<object>()
                        {
                            $"=Player!B{gsRowId}",
                            $"=IFERROR(query({_guildPlayerStatsSheet}!D:K, \"select sum(G) where D='\"&$A{gsRowId}&\"' label sum(G) ''\", 0),0)",
                            $"=IFERROR(query({_guildPlayerStatsSheet}!D:K, \"select sum(H) where D='\"&$A{gsRowId}&\"' label sum(H) ''\", 0),0)",
                            $"=ARRAYFORMULA(TEXT(SUM(IFERROR(QUERY({_guildPlayerStatsSheet}!D:K, \"select I where D='\"&$A{gsRowId}&\"' label I ''\"))), \"[h]:mm:ss\"))",
                            $"=IFERROR(query({_guildPlayerStatsSheet}!D:K, \"select sum(J) where D='\"&$A{gsRowId}&\"' label sum(J) ''\", 0),0)",
                            $"=IFERROR(query({_guildPlayerStatsSheet}!D:K, \"select sum(K) where D='\"&$A{gsRowId}&\"' label sum(K) ''\", 0),0)",
                            $"=IFERROR(DIVIDE(B{gsRowId},C{gsRowId}), 0)"
                        };

                        _newEntry.AddNewPlayerTotalStatsRow(newTotalRow, $"{_guildPlayerStatsTotalsSheet}");
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
                else
                {
                    RespondMessage = $"{modal.User.Mention} This player already exists";
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
                string updatedBy = $"{modal.User.Username}#{modal.User.Discriminator}";
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

                var playerSearch = _search.SearchPlayer(activisionId, _guildPlayerSheet);
                if (string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    if (playerSearch != null)
                    {
                        updatePlayerEntry.Add(new UpdatePlayerModel(activisionId.ToUpper(), discordName.ToUpper(), twitter.ToUpper(), active.ToUpper()));
                        _update.UpdatePlayer(updatePlayerEntry, playerSearch.Id, $"{_guildPlayerSheet}", updatedBy);
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
            string playerTeamName = string.Empty;

            var searchPlayerModal = modal.Data.CustomId;
            var searchPlayerComponents = modal.Data.Components.ToList();

            string activisionId = searchPlayerComponents
                .First(x => x.CustomId == "search_player").Value;

            PlayerModel player = _search.SearchPlayer(activisionId, _guildPlayerSheet);
            if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
            {
                RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                return;
            }
            
            PlayerStatTotalsModel playerStats = _search.SearchPlayerStatTotals(player.ActivsionId, _guildPlayerStatsSheet);
            if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
            {
                RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                return;
            }

            RosterModel roster = _search.SearchRosterForPlayer(player.ActivsionId, _guildRosterSheet);
            if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
            {
                RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                return;
            }
            else if(roster == null)
            {
                playerTeamName = "N/A";
            }
            else
            {
                playerTeamName = roster.TeamName;
            }

            RespondMessage = $"{modal.User.Mention}\r" +
                $"Player Information:\r" +
                $"Current Team: {playerTeamName}\r" +
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
        private void UpdateRoster(string teamName, string userInputPlayers, SocketModal modal, string rosterUpdateType,
            SocketInteractionContext ctx)
        {
            RespondMessage = string.Empty;
            _invalidPlayerList.Clear();
            List<string> playerList = new List<string>();
            RosterModel rosterIds = null;

            var teamMatch = _search.SearchTeam(teamName, _guildTeamSheet);
            var rosterTeam = _search.SearchRosterSingle(teamName, _guildRosterSheet);
            var fullRosterList = _search.GetRosterIdsList(_guildRosterSheet);
            var fullPlayerList = _search.GetPlayerList(_guildPlayerSheet);

            if (rosterTeam != null)
            {
                rosterIds = _search.SearchRoster(rosterTeam, _guildRosterSheet);
            }
            else
            {
                RespondMessage = $"{modal.User.Mention} Could not find Roster for team: {teamName}";
                return;
            }

            if (string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
            {
                if (rosterTeam != null && teamMatch != null && fullRosterList != null && fullPlayerList != null)
                {
                    string discordUser = $"{modal.User.Username}#{modal.User.Discriminator}";
                    if (discordUser.ToUpper().Equals(teamMatch.Captain.ToUpper()) ||
                        discordUser.ToUpper().Equals(teamMatch.Manager.ToUpper()) ||
                        _isLeagueStaff)
                    {
                        if (!string.IsNullOrWhiteSpace(userInputPlayers))
                        {
                            playerList = userInputPlayers.Split(",").ToList();
                            playerList = playerList.ConvertAll(x => x.ToUpper());
                        }

                        if (playerList.Count > 6)
                        {
                            RespondMessage = "You can only have a maximum of 6 players per roster.";
                            return;
                        }

                        var playerModelList = CreatePlayerList(playerList, fullPlayerList);

                        if (_invalidPlayerList.Any())
                        {
                            RespondMessage = $"{modal.User.Mention}\rThe players listed below could not be found.\r" +
                                    $"Please ensure these players exist inside the Player database before you assign them to a Roster.\r\r";

                            foreach (var player in _invalidPlayerList)
                            {
                                RespondMessage += $"{player}\r";
                            }

                            _invalidPlayerList.Clear();
                            return;
                        }


                        var playerOnRosterList = IsPlayerOnRoster(fullRosterList, playerModelList, rosterTeam);
                        if (playerOnRosterList.Any())
                        {
                            RespondMessage = $"{modal.User.Mention}\rThe players listed below are already assigned to a Roster.\r";

                            foreach (var player in playerOnRosterList)
                            {
                                RespondMessage += $"{player.ActivsionId}\r";
                            }

                            _invalidPlayerList.Clear();
                            return;
                        }

                        if(!rosterUpdateType.Equals("REMOVE"))
                        {
                            RosterSlotsAvailable(rosterTeam, playerModelList);
                        }
                        else
                        {
                            _slotsAvailable = true;
                        }                     

                        if (_slotsAvailable)
                        {
                            _update.UpdateRoster(playerModelList, rosterIds, $"{_guildRosterSheet}", rosterUpdateType, discordUser);
                            RespondMessage = $"{modal.User.Mention}\r";

                            switch(rosterUpdateType)
                            {
                                case "ADD":
                                    RespondMessage += $"Players added to Roster {rosterTeam.TeamName}:\r";
                                    break;
                                case "REMOVE":
                                    RespondMessage += $"Players removed from Roster {rosterTeam.TeamName}:\r";
                                    break;
                            }

                            foreach (var player in playerModelList)
                            {
                                RespondMessage += $"{player.ActivsionId}\r";
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        RespondMessage = $"{modal.User.Mention}\rYou are not listed as Captain or Manager for this team. You cannot edit this roster.";
                        return;
                    }
                }
                else
                {
                    RespondMessage = $"{modal.User.Mention}\rError retrieving required Player, Roster, or Team information from database.";
                    return;
                }
            }
            else
            {
                RespondMessage = $"{modal.User.Mention}\r" +
                    $"{_search.SearchExceptionMsg}";
                return;
            }
        }     
        public void ValidateAddPlayers(SocketModal modal, SocketInteractionContext ctx)
        {
            RespondMessage = string.Empty;
            string rosterUpdateType = "ADD";
            ValidateDiscordUser(ctx);
            if (_isLeagueStaff || _isCaptain || _isManager)
            {
                var modalName = modal.Data.CustomId;
                var components = modal.Data.Components.ToList();

                string teamName = components
                .First(x => x.CustomId == "rosteradd_teamsearch").Value;

                string addPlayer = components
                .First(x => x.CustomId == "rosteradd_addplayers").Value;

                UpdateRoster(teamName, addPlayer, modal, rosterUpdateType, ctx);
            }
            else
            {
                RespondMessage = "You do not have permission you use this command.";
            }
        }
        public void ValidateRemovePlayers(SocketModal modal, SocketInteractionContext ctx)
        {
            RespondMessage = string.Empty;
            string rosterUpdateType = "REMOVE";
            ValidateDiscordUser(ctx);
            if (_isLeagueStaff || _isCaptain || _isManager)
            {
                var modalName = modal.Data.CustomId;
                var components = modal.Data.Components.ToList();

                string teamName = components
                .First(x => x.CustomId == "rosterremove_teamsearch").Value;

                string removePlayer = components
                .First(x => x.CustomId == "rosterremove_addplayers").Value;

                UpdateRoster(teamName, removePlayer, modal, rosterUpdateType, ctx);
            }
            else
            {
                RespondMessage = "You do not have permission you use this command.";
            }
        }
        private void RosterSlotsAvailable(RosterModel roster, List<PlayerModel> addPlayerList)
        {
            int availableSlotCount = 0;

            if(string.IsNullOrWhiteSpace(roster.PlayerOne))
            {
                availableSlotCount++;
            }

            if (string.IsNullOrWhiteSpace(roster.PlayerTwo))
            {
                availableSlotCount++;
            }

            if (string.IsNullOrWhiteSpace(roster.PlayerThree))
            {
                availableSlotCount++;
            }

            if (string.IsNullOrWhiteSpace(roster.PlayerFour))
            {
                availableSlotCount++;
            }

            if (string.IsNullOrWhiteSpace(roster.PlayerFive))
            {
                availableSlotCount++;
            }

            if (string.IsNullOrWhiteSpace(roster.PlayerSix))
            {
                availableSlotCount++;
            }

            if (availableSlotCount >= addPlayerList.Count)
            {
                _slotsAvailable = true;
            }
            else
            {
                _slotsAvailable = false;
                RespondMessage = $"Not enough available roster slots.\r" +
                    $"Open Roster Slots: {availableSlotCount}\r" +
                    $"Number of players to add: {addPlayerList.Count}";
            }
        }
        private List<PlayerModel> IsPlayerOnRoster(List<RosterModel> rosterList, List<PlayerModel> playerList, RosterModel rosterTeamMatch)
        {
            List<PlayerModel> playerOnRosterList = new List<PlayerModel>();

            foreach(RosterModel roster in rosterList)
            {
                if(roster.TeamName != rosterTeamMatch.TeamName)
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
                            playerOnRosterList.Add(player);
                        }
                    }
                }
            }

            return playerOnRosterList;
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

                var teamOne = _search.SearchTeam(teamOneName, _guildTeamSheet);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\rSearch Team Name: {teamOne} is incorrect or does not exist.";
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

                var teamTwo = _search.SearchTeam(teamTwoName, _guildTeamSheet);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\rSearch Team Name: {teamTwo} is incorrect or does not exist.";
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

                IncrementId($"{_guildMatchResultSheet}");
                Guid guid = Guid.NewGuid();

                if(teamOne_MapsWonInt == 0 && teamTwo_MapsWonInt == 0)
                {
                    newMatchResultList = new List<object>()
                    {
                        _id,
                        guid,
                        teamOne.Id,
                        $"=IFERROR(VLOOKUP(C:C, {_guildTeamSheet}!A:B, 2, false), \"\")",
                        0,
                        3,
                        teamTwo.Id,
                        $"=IFERROR(VLOOKUP(G:G, {_guildTeamSheet}!A:B, 2, false), \"\")",
                        0,
                        3,
                        group.ToUpper(),
                        $"NA",
                        $"NA",
                        "",
                        "",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                }
                else if (teamOne_MapsWonInt >= 3)
                {
                    newMatchResultList = new List<object>()
                    {
                        _id,
                        guid,
                        teamOne.Id,
                        $"=IFERROR(VLOOKUP(C:C, {_guildTeamSheet}!A:B, 2, false), \"\")",
                        teamOne_MapsWonInt,
                        teamTwo_MapsWonInt,
                        teamTwo.Id,
                        $"=IFERROR(VLOOKUP(G:G, {_guildTeamSheet}!A:B, 2, false), \"\")",
                        teamTwo_MapsWonInt,
                        teamOne_MapsWonInt,
                        group.ToUpper(),
                        $"=IFERROR(D{_id}, \"\")",
                        $"=IFERROR(H{_id}, \"\")",
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
                        $"=IFERROR(VLOOKUP(C:C, {_guildTeamSheet}!A:B, 2, false), \"\")",
                        teamOne_MapsWonInt,
                        teamTwo_MapsWonInt,
                        teamTwo.Id,
                        $"=IFERROR(VLOOKUP(G:G, {_guildTeamSheet}!A:B, 2, false), \"\")",
                        teamTwo_MapsWonInt,
                        teamOne_MapsWonInt,
                        group.ToUpper(),
                        $"=IFERROR(H{_id}, \"\")",
                        $"=IFERROR(D{_id}, \"\")",
                        "",
                        "",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                }

                _newEntry.AddNewMatchResult(newMatchResultList, $"{_guildMatchResultSheet}");
                if (string.IsNullOrWhiteSpace(_newEntry.NewEntryResponseMessage))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"Match Recorded:\r\r" +
                        $"TEAM 1: {teamOneName.ToUpper()}\r" +
                        $"TEAM 2: {teamTwoName.ToUpper()}\r" +
                        $"MATCH GUID: {guid}\r\r" +
                        $"NOTE: If you have player stats that need to be submitted for this match.\r" +
                        $"Please use the match guid above to link the player stats to this match.";
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
                string updatedBy = $"{modal.User.Username}#{modal.User.Discriminator}";
                int teamOne_MapsWonInt = 0;
                int teamTwo_MapsWonInt = 0;
                string winner = string.Empty;
                string loser = string.Empty;
                bool response = false;

                var modalName = modal.Data.CustomId;
                var components = modal.Data.Components.ToList();

                string guid = components
                .First(x => x.CustomId == "updatematch_searchmatchGUID").Value;

                var match = _search.SearchMatchResult(guid, _guildMatchResultSheet);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"{guid} - {_search.SearchExceptionMsg}";
                }

                string teamOneName = components
                .First(x => x.CustomId == "updatematch_team_1_name").Value;

                var teamOne = _search.SearchTeam(teamOneName, _guildTeamSheet);
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

                var teamTwo = _search.SearchTeam(teamTwoName, _guildTeamSheet);
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

                IncrementId($"{_guildMatchResultSheet}");

                if (teamOne_MapsWonInt >= 3)
                {
                    winner = $"=IFERROR(D{_id}, \"\")";
                    loser = $"=IFERROR(H{_id}, \"\")";
                }
                else if (teamTwo_MapsWonInt >= 3)
                {
                    winner = $"=IFERROR(H{_id}, \"\")";
                    loser = $"=IFERROR(D{_id}, \"\")";
                }

                _update.UpdateMatchResult(teamOne, teamTwo, teamOne_MapsWonInt, teamTwo_MapsWonInt, $"{_guildMatchResultSheet}", winner, loser, _id, updatedBy);
                if (string.IsNullOrWhiteSpace(_update.UpdateResponseMessage))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"Match GUID: {guid}\rhas been updated\r\r" +
                        $"Team 1: {teamOne.TeamName}\r" +
                        $"Maps Won: {teamOne_MapsWonInt}\r" +
                        $"Team 2: {teamTwo.TeamName}\r" +
                        $"Maps Won: {teamTwo_MapsWonInt}";
                }
                else
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
                .First(x => x.CustomId == "search_matchresultguid").Value;

            MatchResultModel matchResult = _search.SearchMatchResult(userInputGuid, _guildMatchResultSheet);
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
            IncrementId(Values.GoogleSheets.feedback);
            _feedback.AddFeedback(modal, Values.GoogleSheets.feedback, _id);
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

            RespondMessage = $"{modal.User.Mention}\rFree Agent:\r\r" +
                $"Type: {type.ToUpper()}\r" +
                $"Age: {age}\r" +
                $"Role: {role.ToUpper()}\r" +
                $"Region & TimeZone: {region.ToUpper()}\r\r" +
                $"Other Notes:\r" +
                $"{other}";
        }

        #region PlayerStats
        public void ValidateNewPlayerStats(SocketModal modal, SocketInteractionContext ctx)
        {
            ValidateDiscordUser(ctx);
            if (_isLeagueStaff)
            {
                string guildNameNoSpace = Regex.Replace(ctx.Guild.Name, @"\s+", "");
                RespondMessage = string.Empty;
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

                var guidMatch = _search.SingleColumnSearch($"{guildNameNoSpace}_{Values.GoogleSheets.matchResult}", "B2:B", guid);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                    _search.SearchExceptionMsg = string.Empty;
                    return;
                }

                string playerName = components
                    .First(x => x.CustomId == "newstats_playername").Value;

                var player = _search.SearchPlayer(playerName, _guildPlayerSheet);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"Player: {playerName} - {_search.SearchExceptionMsg}";
                    _search.SearchExceptionMsg = string.Empty;
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
                        _search.SearchExceptionMsg = string.Empty;
                        return;
                    }

                    gamemodeMatch = _search.SearchGamemode(gamemode);
                    if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                    {
                        RespondMessage = $"{modal.User.Mention}\r" +
                        $"{gamemode.ToUpper()} - {_search.SearchExceptionMsg}";
                        _search.SearchExceptionMsg = string.Empty;
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

                IncrementId($"{guildNameNoSpace}_{Values.GoogleSheets.playerStats}");
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
                    "",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                _newEntry.AddNewPlayerStats(playerStatsList, $"{guildNameNoSpace}_{Values.GoogleSheets.playerStats}");
                if (string.IsNullOrWhiteSpace(_newEntry.NewEntryResponseMessage))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"Player stats have been recorded.\r\r" +
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
        public void ValidateUpdatePlayerStats(SocketModal modal, SocketInteractionContext ctx)
        {
            ValidateDiscordUser(ctx);
            if (_isLeagueStaff)
            {
                string guildNameNoSpace = Regex.Replace(ctx.Guild.Name, @"\s+", "");
                RespondMessage = string.Empty;
                List<object> playerStatsList = new List<object>();
                StatsModel statsList;
                MapModel mapMatch;
                GamemodeModel gamemodeMatch;

                string map = string.Empty;
                string gamemode = string.Empty;
                string existingGuid = string.Empty;
                string newGuid = string.Empty;

                var modalName = modal.Data.CustomId;
                var components = modal.Data.Components.ToList();
                string guid = components
                    .First(x => x.CustomId == $"updatestats_guid").Value;

                guid = Regex.Replace(guid, @"\s+", ",");
                string[] guids = guid.Split(",");
                
                if(guids.Length > 1)
                {
                    existingGuid = guids[0];
                    newGuid = guids[1];
                }
                else
                {
                    existingGuid = guids[0];
                }

                var guidMatch = _search.SingleColumnSearch($"{guildNameNoSpace}_{Values.GoogleSheets.playerStats}", "B2:B", existingGuid);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"{_search.SearchExceptionMsg}";
                    _search.SearchExceptionMsg = string.Empty;
                    return;
                }

                string playerName = components
                    .First(x => x.CustomId == "updatestats_playername").Value;

                var player = _search.SearchPlayer(playerName, _guildPlayerSheet);
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"Player: {playerName} - {_search.SearchExceptionMsg}";
                    _search.SearchExceptionMsg = string.Empty;
                    return;
                }

                IncrementId($"{guildNameNoSpace}_{Values.GoogleSheets.playerStats}");
                _id--;
                var playerStatMatch = _search.SearchPlayerStatsDataTable(guidMatch, _id.ToString(), _guildPlayerStatsSheet);
                if(!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"Player Stat Search - {_search.SearchExceptionMsg}";
                    _search.SearchExceptionMsg = string.Empty;
                    return;
                }

                string mapMode = components
                .First(x => x.CustomId == "updatestats_mapmode").Value;

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
                        _search.SearchExceptionMsg = string.Empty;
                        return;
                    }

                    gamemodeMatch = _search.SearchGamemode(gamemode);
                    if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                    {
                        RespondMessage = $"{modal.User.Mention}\r" +
                        $"{gamemode.ToUpper()} - {_search.SearchExceptionMsg}";
                        _search.SearchExceptionMsg = string.Empty;
                        return;
                    }
                }

                string stats = components
                .First(x => x.CustomId == "updatestats_stats").Value;
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

                var fullDiscordName = $"{modal.User.Username}#{modal.User.Discriminator}";
                _id++;

                if(!string.IsNullOrWhiteSpace(newGuid))
                {
                    playerStatsList = new List<object>()
                    {
                        newGuid,
                        playerStatMatch.PlayerId,
                        "=IFERROR(VLOOKUP(C:C, Player!A:B, 2, false), \"\")",
                        mapMatch.MapName,
                        gamemodeMatch.ModeName,
                        statsList.Kills,
                        statsList.Deaths,
                        statsList.HillTime,
                        statsList.BombsPlanted,
                        statsList.ObjKills,
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        fullDiscordName,
                    };
                }
                else
                {
                    playerStatsList = new List<object>()
                    {
                        existingGuid,
                        playerStatMatch.PlayerId,
                        "=IFERROR(VLOOKUP(C:C, Player!A:B, 2, false), \"\")",
                        mapMatch.MapName,
                        gamemodeMatch.ModeName,
                        statsList.Kills,
                        statsList.Deaths,
                        statsList.HillTime,
                        statsList.BombsPlanted,
                        statsList.ObjKills,
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        fullDiscordName,
                    };
                }

                _update.UpdatePlayerStats(playerStatsList, $"{guildNameNoSpace}_{Values.GoogleSheets.playerStats}", _id);
                if (string.IsNullOrWhiteSpace(_update.UpdateResponseMessage))
                {
                    RespondMessage = $"{modal.User.Mention}\r" +
                        $"Player stats have been updated.\r\r" +
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
                        $"Update Exception:\r{_update.UpdateResponseMessage}";
                }
            }
            else
            {
                RespondMessage = "You do not have permission you use this command.";
            }
        }
        #endregion

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

                string chnlId = components
                    .First(x => x.CustomId == "discord_chnl").Value;

                try
                {
                    var channel = ctx.Guild.GetChannel(Convert.ToUInt64(chnlId));

                    itemList = new List<object>()
                {
                    guildId.ToString(),
                    guildName,
                    chnlId.ToString(),
                    channel.Name
                };

                    _newEntry.RegisterNewAdminChnl(itemList, Values.GoogleSheets.registerChannel);
                    RespondMessage = $"{modal.User.Mention} {channel.Name} has been registered.\r" +
                        "You will now receive messages in that channel whenever a user submits a new request.";
                }
                catch (Exception ex)
                {
                    RespondMessage = "Channel ID is incorrect or does not exist.";
                    return;
                }             
            }
            else
            {
                RespondMessage = "You do not have permission you use this command.";
            }         
        }
        public void ValidateAdminRequest(SocketInteractionContext ctx, SocketModal modal)
        {
            ulong guildId = ctx.Guild.Id;

            var guildIdMatchList = _search.SearchChannelTable(guildId);
            if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
            {
                IsAdminRequestValid = false;
                RespondMessage = $"{modal.User.Mention}\r" +
                    $"{_search.SearchExceptionMsg}";
                return;
            }
            else
            {
                foreach(ChannelModel channel in guildIdMatchList)
                {
                    if(channel.ChannelName.Contains("admin"))
                    {
                        IsAdminRequestValid = true;
                        ChannelId = Convert.ToUInt64(channel.ChannelId);
                        RespondMessage = $"{modal.User.Mention}\r" +
                            $"Your request has been submitted!\r" +
                            $"An admin will contact you as soon as possible. Thank you for your patience.";
                        return;
                    }
                }
            }
        }
        public void ValidateCasterRequest(SocketInteractionContext ctx, SocketModal modal)
        {
            var guildMatchList = _search.SearchChannelTable(ctx.Guild.Id);
            if (_search.SearchExceptionMsg == null)
            {
                foreach(ChannelModel channel in guildMatchList)
                {
                    if(channel.ChannelName.Contains("caster"))
                    {
                        ChannelId = Convert.ToUInt64(channel.ChannelId);
                        RespondMessage = $"{modal.User.Mention}\r" +
                        $"Your caster request has been submitted!\r" +
                        $"A Caster will reach out to you if your match has been chosen to be casted.\r" +
                        $"With our limited number of casters, not all caster requests can be filled. We apologize for any inconvenience.\r\r" +
                        $"XP League will also send out a notification letting everyone know which match is being casted and where they can watch.";
                        return;
                    }
                }
            }
            else
            {
                RespondMessage = "Channel ID is incorrect or does not exist.";
                return;
            }
        }
        public async Task ValidateLeagueCredentialsAsync(SocketCommandContext ctx, string clientPassword)
        {
            var leagueStaffRole = ctx.Guild.Roles.FirstOrDefault(x => x.Name == "League Staff");
            SocketGuildUser discordUser = ctx.User as SocketGuildUser;
            if (discordUser != null)
            {
                foreach (var role in discordUser.Roles)
                {
                    if (role == leagueStaffRole)
                    {
                        _isLeagueStaff = true;
                        break;
                    }
                }
            }

            if(_isLeagueStaff)
            {
                if (clientPassword.Length < 8)
                {
                    RespondMessage = "League password must be at least 8 characters long";
                    return;
                }

                _search.SingleColumnSearch(Values.GoogleSheets.leagueCredentials, "A2:A", Convert.ToString(ctx.Guild.Id));
                if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
                {
                    string guildNameNoSpace = Regex.Replace(ctx.Guild.Name, @"\s+", "");

                    List<object> credentialList = new List<object>()
                    {
                        $"{ctx.Guild.Id}",
                        $"{guildNameNoSpace}",
                        $"{clientPassword}",
                        $"{ctx.User.Username}#{ctx.User.Discriminator}",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    _newEntry.AddNewLeagueCredentials(credentialList, Values.GoogleSheets.leagueCredentials);
                    await _createCodLeague.CreateNewCodLeague(guildNameNoSpace);
                    RespondMessage = "League created!\rYou now have access to all slash commands";
                }
                else
                {
                    RespondMessage = $"{ctx.User.Mention}\r" +
                        $"ERROR: This League already exists. Cannot create a new one.";
                    return;
                }
            }
            else
            {
                RespondMessage = $"{ctx.User.Mention}\rERROR: You do not have permission you use this command.";
            }
        }
        private void IncrementId(string sheetName)
        {
            try
            {
                _id = 0;

                var range = $"{sheetName}!A2:A";
                var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

                var response = request.ExecuteAsync();
                var requestResponse = response.Result;

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
            catch(Exception ex)
            {
                RespondMessage = $"DataValidation | IncrementId | Error: {ex}";
            }
        }

        private async void AddRole(string discordName, SocketRole role, List<SocketGuildUser> discordUserList)
        {
            SocketGuildUser discordUser;

            try
            {
                foreach (SocketGuildUser user in discordUserList)
                {
                    var userFullName = $"{user.Username.ToUpper()}#{user.Discriminator}";
                    if (userFullName.Equals(discordName.ToUpper()))
                    {
                        discordUser = user;
                        await (user as IGuildUser).AddRoleAsync(role);
                    }
                }
            }
            catch(Exception ex)
            {
                RespondMessage = $"Error adding role: {ex}";
            }
        }
        private async void RemoveRole(string discordName, SocketRole role, List<SocketGuildUser> discordUserList)
        {
            SocketGuildUser discordUser;

            try
            {
                foreach (var user in discordUserList)
                {
                    var userFullName = $"{user.Username.ToUpper()}#{user.Discriminator}";
                    if (userFullName.Equals(discordName.ToUpper()))
                    {
                        discordUser = user;
                        await (user as IGuildUser).RemoveRoleAsync(role);
                    }
                }
            }
            catch(Exception ex)
            {
                RespondMessage = $"Error removing role: {ex}";
            }
        }
        private List<PlayerModel> CreatePlayerList(List<string> playerList, List<PlayerModel> fullPlayerList)
        {
            List<PlayerModel> playerModelList = new List<PlayerModel>();
            PlayerModel searchResult = null;
            int index = 0;

            foreach(var player in playerList)
            {
                try
                {
                    searchResult = fullPlayerList.Single(p => p.ActivsionId.ToUpper() == playerList[index].ToUpper());
                    playerModelList.Add(searchResult);
                    index++;
                    continue;
                }
                catch
                {
                    _invalidPlayerList.Add(player);
                    index++;
                }
            }

            return playerModelList;
        }
        public void ValidateLeagueCreated(SocketInteractionContext ctx, SocketModal modal)
        {
            _search.SingleColumnSearch(Values.GoogleSheets.leagueCredentials, "A2:A", Convert.ToString(ctx.Guild.Id));
            if (!string.IsNullOrWhiteSpace(_search.SearchExceptionMsg))
            {
                RespondMessage = $"{modal.User.Mention} No League exists for this discord. If you would like to create one please use !createleague\r" +
                    $"**NOTE:** All slash commands will be unavilable until a League is created.";
                IsLeagueCreated = false;
            }
            else
            {
                IsLeagueCreated = true;
            }
        }
    }
}
