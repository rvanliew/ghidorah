using Discord.WebSocket;
using GhidorahBot.Models;
using GhidorahBot.Extensions;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;

namespace GhidorahBot.Database
{
    public class Update
    {
        public string TeamName;
        public string ActivisionId;
        public List<UpdateTeamUserInput> UserTeamInputList = new List<UpdateTeamUserInput>();
        public List<UpdatePlayerUserInput> UserPlayerInputList = new List<UpdatePlayerUserInput>();
        private List<UpdateRosterUserInput> UserRosterInputList = new List<UpdateRosterUserInput>();

        private static readonly string _teamSheet = "Team";
        private static readonly string _playerSheet = "Player";
        private static readonly string _rosterSheet = "Roster";
        private static readonly string _matchResult = "MatchResult";
        private int _rowId;
        private List<RosterModel> _updatedRosterList = new List<RosterModel>();

        private IConfiguration _config;
        private SheetsService _service;
        private Search _search { get; set; }

        public Update(IConfiguration config, SheetsService service, Search search)
        {
            _config = config;
            _service = service;
            _search = search;
        }

        public void UpdateTeam(SocketModal modal, string id)
        {
            UserTeamInputList.Clear();

            _rowId = Convert.ToInt32(id);
            _rowId++;

            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            string teamName = components
            .First(x => x.CustomId == "edit_team_Name").Value;

            string twitter = components
            .First(x => x.CustomId == "edit_team_twitter").Value;

            string group = components
            .First(x => x.CustomId == "edit_team_group").Value;

            string active = components
            .First(x => x.CustomId == "edit_team_active").Value;

            UserTeamInputList.Add(new UpdateTeamUserInput(teamName, twitter, group, active));

            foreach (var team in UserTeamInputList)
            {
                if (!string.IsNullOrWhiteSpace(team.TeamName))
                {
                    UpdateEntry("B", team.TeamName.ToUpper(), _teamSheet);
                }

                if (!string.IsNullOrWhiteSpace(team.Twitter))
                {
                    UpdateEntry("C", twitter, _teamSheet);
                }

                if (!string.IsNullOrWhiteSpace(team.Group))
                {
                    UpdateEntry("D", group.ToUpper(), _teamSheet);
                }

                if (!string.IsNullOrWhiteSpace(team.Active))
                {
                    UpdateEntry("G", active.ToUpper(), _teamSheet);
                }
            }

            UpdateLastUpdatedDate(_teamSheet);
        }

        public void UpdatePlayer(SocketModal modal, string id)
        {
            UserPlayerInputList.Clear();
            _rowId = Convert.ToInt32(id);
            _rowId++;

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

            UserPlayerInputList.Add(new UpdatePlayerUserInput(activisionId, discordName, twitter, active));

            foreach(var player in UserPlayerInputList)
            {
                if(!string.IsNullOrWhiteSpace(player.ActivisionId))
                {
                    UpdateEntry("B", activisionId, _playerSheet);
                }

                if (!string.IsNullOrWhiteSpace(player.DiscordName))
                {
                    UpdateEntry("C", discordName, _playerSheet);
                }

                if (!string.IsNullOrWhiteSpace(player.Twitter))
                {
                    UpdateEntry("D", twitter, _playerSheet);
                }

                if (!string.IsNullOrWhiteSpace(player.Active))
                {
                    UpdateEntry("G", active.ToUpper(), _playerSheet);
                }
            }

            UpdateLastUpdatedDate(_playerSheet);
        }

        public void UpdateRoster(List<PlayerModel> removePlayerList, List<PlayerModel> addPlayerList, RosterModel roster)
        {
            UserRosterInputList.Clear();
            _updatedRosterList.Clear();
            _rowId = Convert.ToInt32(roster.Id);
            _rowId++;

            if(removePlayerList.Any())
            {
                RemovePlayerFromRoster(removePlayerList, roster);
                var updatedRoster = _search.SearchRoster(roster);
                AddPlayerToRoster(addPlayerList, updatedRoster);
                return;
            }

            if (addPlayerList.Any())
            {
                AddPlayerToRoster(addPlayerList, roster);
            }

            UpdateEntry("P", $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", _rosterSheet);
        }

        public void UpdateMatchResult(TeamModel teamOne, TeamModel teamTwo, string guid, int teamOneMapsWon, int teamTwoMapsWon, string id)
        {
            _rowId = Convert.ToInt32(id);
            _rowId++;

            string _winner = string.Empty;
            string _loser = string.Empty;

            if(teamOneMapsWon >= 3)
            {
                _winner = $"=IFERROR(D{_rowId}, \"\")";
                _loser = $"=IFERROR(H{_rowId}, \"\")";
            }
            else if (teamTwoMapsWon >= 3)
            {
                _winner = $"=IFERROR(H{_rowId}, \"\")";
                _loser = $"=IFERROR(D{_rowId}, \"\")";
            }

            UpdateEntry("C", teamOne.Id, _matchResult);
            UpdateEntry("E", teamOneMapsWon.ToString(), _matchResult);
            UpdateEntry("F", teamTwoMapsWon.ToString(), _matchResult);
            UpdateEntry("G", teamTwo.Id, _matchResult);
            UpdateEntry("I", teamTwoMapsWon.ToString(), _matchResult);
            UpdateEntry("J", teamOneMapsWon.ToString(), _matchResult);
            UpdateEntry("L", _winner, _matchResult);
            UpdateEntry("M", _loser, _matchResult);
            UpdateEntry("O", $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", _matchResult);
        }

        private void RemovePlayerFromRoster(List<PlayerModel> removePlayerList, RosterModel roster)
        {
            foreach (var rPlayer in removePlayerList)
            {
                if (rPlayer.Id == roster.PlayerOne)
                {
                    UpdateEntry("C", "", _rosterSheet);
                }

                if (rPlayer.Id == roster.PlayerTwo)
                {
                    UpdateEntry("E", "", _rosterSheet);
                }

                if (rPlayer.Id == roster.PlayerThree)
                {
                    UpdateEntry("G", "", _rosterSheet);
                }

                if (rPlayer.Id == roster.PlayerFour)
                {
                    UpdateEntry("I", "", _rosterSheet);
                }

                if (rPlayer.Id == roster.PlayerFive)
                {
                    UpdateEntry("K", "", _rosterSheet);
                }

                if (rPlayer.Id == roster.PlayerSix)
                {
                    UpdateEntry("M", "", _rosterSheet);
                }
            }
        }

        private void AddPlayerToRoster(List<PlayerModel> addPlayerList, RosterModel roster)
        {
            try
            {
                foreach (var player in addPlayerList)
                {
                    if (string.IsNullOrWhiteSpace(roster.PlayerOne))
                    {
                        UpdateEntry("C", addPlayerList[0].Id, _rosterSheet);
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(roster.PlayerTwo))
                    {
                        UpdateEntry("E", addPlayerList[1].Id, _rosterSheet);
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(roster.PlayerThree))
                    {
                        UpdateEntry("G", addPlayerList[2].Id, _rosterSheet);
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(roster.PlayerFour))
                    {
                        UpdateEntry("I", addPlayerList[3].Id, _rosterSheet);
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(roster.PlayerFive))
                    {
                        UpdateEntry("K", addPlayerList[4].Id, _rosterSheet);
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(roster.PlayerSix))
                    {
                        UpdateEntry("M", addPlayerList[5].Id, _rosterSheet);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                //
            }
        }

        private void UpdateEntry(string rowLetter, string rowData, string sheetName)
        {
            var range = $"{sheetName}!{rowLetter}{_rowId}";
            var valueRange = new ValueRange();

            var objectList = new List<object>() { $"{rowData}" };
            valueRange.Values = new List<IList<object>> { objectList };

            var updateRequest = _service.Spreadsheets.Values.Update(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var updateResponse = updateRequest.ExecuteAsync();
        }

        private void UpdateLastUpdatedDate(string sheetName)
        {
            var range = $"{sheetName}!F{_rowId}";
            var valueRange = new ValueRange();

            var objectList = new List<object>() { $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}" };
            valueRange.Values = new List<IList<object>> { objectList };

            var updateRequest = _service.Spreadsheets.Values.Update(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var updateResponse = updateRequest.ExecuteAsync();
        }
    }
}
