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
        public string UpdateResponseMessage = string.Empty;

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

        /// <summary>
        /// Update an Existing Team
        /// </summary>
        /// <param name="updateList"></param>
        /// <param name="id"></param>
        /// <param name="teamSheet"></param>
        public void UpdateTeam(List<UpdateTeamModel> updateList, string id, string teamSheet)
        {
            try
            {
                _rowId = Convert.ToInt32(id);
                _rowId++;

                foreach (var team in updateList)
                {
                    if (!string.IsNullOrWhiteSpace(team.TeamName))
                    {
                        UpdateEntry("B", team.TeamName, teamSheet);
                    }

                    if (!string.IsNullOrWhiteSpace(team.Twitter))
                    {
                        UpdateEntry("C", team.Twitter, teamSheet);
                    }

                    if (!string.IsNullOrWhiteSpace(team.Group))
                    {
                        UpdateEntry("D", team.Group, teamSheet);
                    }

                    if (!string.IsNullOrWhiteSpace(team.Captain))
                    {
                        UpdateEntry("E", team.Captain, teamSheet);
                    }

                    if (!string.IsNullOrWhiteSpace(team.Manager))
                    {
                        UpdateEntry("F", team.Manager, teamSheet);
                    }
                }

                //Update LastUpdated Date
                UpdateEntry("G", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), teamSheet);
            }
            catch (Exception ex)
            {
                UpdateResponseMessage = $"UPDATE TEAM EXCEPTION:\r{ex}";
            }        
        }

        /// <summary>
        /// Update and Exisiting Player
        /// </summary>
        /// <param name="updateList"></param>
        /// <param name="id"></param>
        /// <param name="playerSheet"></param>
        public void UpdatePlayer(List<UpdatePlayerModel> updateList, string id, string playerSheet)
        {
            try
            {
                _rowId = Convert.ToInt32(id);
                _rowId++;

                foreach (var player in updateList)
                {
                    if (!string.IsNullOrWhiteSpace(player.ActivisionId))
                    {
                        UpdateEntry("B", player.ActivisionId, playerSheet);
                    }

                    if (!string.IsNullOrWhiteSpace(player.DiscordName))
                    {
                        UpdateEntry("C", player.DiscordName, playerSheet);
                    }

                    if (!string.IsNullOrWhiteSpace(player.Twitter))
                    {
                        UpdateEntry("D", player.Twitter, playerSheet);
                    }

                    if (!string.IsNullOrWhiteSpace(player.Active))
                    {
                        UpdateEntry("F", player.Active, playerSheet);
                    }
                }

                //Update LastUpdated Date
                UpdateEntry("G", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), playerSheet);
            }
            catch (Exception ex)
            {
                UpdateResponseMessage = $"UPDATE TEAM EXCEPTION:\r{ex}";
            }     
        }

        public void UpdateRoster(List<PlayerModel> removePlayerList, List<PlayerModel> addPlayerList, RosterModel roster, string rosterSheet)
        {
            _updatedRosterList.Clear();
            _rowId = Convert.ToInt32(roster.Id);
            _rowId++;

            if(removePlayerList.Any())
            {
                RemovePlayerFromRoster(removePlayerList, roster, rosterSheet);
                var updatedRoster = _search.SearchRoster(roster);
                AddPlayerToRoster(addPlayerList, updatedRoster, rosterSheet);
                return;
            }

            if (addPlayerList.Any())
            {
                AddPlayerToRoster(addPlayerList, roster, rosterSheet);
            }

            UpdateEntry("P", $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", rosterSheet);
        }

        public void UpdateMatchResult(TeamModel teamOne, TeamModel teamTwo, int teamOneMapsWon, int teamTwoMapsWon,
            string matchResultSheet, string winner, string loser, int id)
        {
            _rowId = Convert.ToInt32(id);
            _rowId++;

            UpdateEntry("C", teamOne.Id, matchResultSheet);
            UpdateEntry("E", teamOneMapsWon.ToString(), matchResultSheet);
            UpdateEntry("F", teamTwoMapsWon.ToString(), matchResultSheet);
            UpdateEntry("G", teamTwo.Id, matchResultSheet);
            UpdateEntry("I", teamTwoMapsWon.ToString(), matchResultSheet);
            UpdateEntry("J", teamOneMapsWon.ToString(), matchResultSheet);
            UpdateEntry("L", winner, matchResultSheet);
            UpdateEntry("M", loser, matchResultSheet);
            UpdateEntry("N", $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", matchResultSheet);
        }

        private void RemovePlayerFromRoster(List<PlayerModel> removePlayerList, RosterModel roster, string rosterSheet)
        {
            foreach (var rPlayer in removePlayerList)
            {
                if (rPlayer.Id == roster.PlayerOne)
                {
                    UpdateEntry("C", "", rosterSheet);
                }

                if (rPlayer.Id == roster.PlayerTwo)
                {
                    UpdateEntry("E", "", rosterSheet);
                }

                if (rPlayer.Id == roster.PlayerThree)
                {
                    UpdateEntry("G", "", rosterSheet);
                }

                if (rPlayer.Id == roster.PlayerFour)
                {
                    UpdateEntry("I", "", rosterSheet);
                }

                if (rPlayer.Id == roster.PlayerFive)
                {
                    UpdateEntry("K", "", rosterSheet);
                }

                if (rPlayer.Id == roster.PlayerSix)
                {
                    UpdateEntry("M", "", rosterSheet);
                }
            }
        }

        private void AddPlayerToRoster(List<PlayerModel> addPlayerList, RosterModel roster, string rosterSheet)
        {
            try
            {
                foreach (var player in addPlayerList)
                {
                    if (string.IsNullOrWhiteSpace(roster.PlayerOne))
                    {
                        UpdateEntry("C", addPlayerList[0].Id, rosterSheet);
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(roster.PlayerTwo))
                    {
                        UpdateEntry("E", addPlayerList[1].Id, rosterSheet);
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(roster.PlayerThree))
                    {
                        UpdateEntry("G", addPlayerList[2].Id, rosterSheet);
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(roster.PlayerFour))
                    {
                        UpdateEntry("I", addPlayerList[3].Id, rosterSheet);
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(roster.PlayerFive))
                    {
                        UpdateEntry("K", addPlayerList[4].Id, rosterSheet);
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(roster.PlayerSix))
                    {
                        UpdateEntry("M", addPlayerList[5].Id, rosterSheet);
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
            try
            {
                var range = $"{sheetName}!{rowLetter}{_rowId}";
                var valueRange = new ValueRange();

                var objectList = new List<object>() { $"{rowData}" };
                valueRange.Values = new List<IList<object>> { objectList };

                var updateRequest = _service.Spreadsheets.Values.Update(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                var updateResponse = updateRequest.ExecuteAsync();
            }
            catch(Exception ex)
            {
                UpdateResponseMessage = $"{ex}";
            }
        }
    }
}
