using Discord.WebSocket;
using GhidorahBot.Models;
using GhidorahBot.Extensions;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;

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
        public void UpdateTeam(List<UpdateTeamModel> updateList, string id, string teamSheet, string discordUser)
        {
            UpdateResponseMessage = string.Empty;

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
                UpdateEntry("H", discordUser, teamSheet);
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
        public void UpdatePlayer(List<UpdatePlayerModel> updateList, string id, string playerSheet, string updatedBy)
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
                }

                //Update LastUpdated Date
                UpdateEntry("E", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), playerSheet);
                UpdateEntry("F", updatedBy, playerSheet);
            }
            catch (Exception ex)
            {
                UpdateResponseMessage = $"UPDATE PLAYER EXCEPTION:\r{ex}";
            }     
        }

        public void UpdateRoster(List<PlayerModel> playerList, RosterModel roster, string rosterSheet, string type, string discordUser)
        {
            _updatedRosterList.Clear();
            _rowId = Convert.ToInt32(roster.Id);
            _rowId++;

            switch(type)
            {
                case "ADD":
                    var addObjectList = AddPlayerToRoster(playerList, roster);
                    UpdateRosterEntry(rosterSheet, addObjectList);
                    break;
                case "REMOVE":
                    var removeObjectList = RemovePlayerFromRoster(playerList, roster);
                    UpdateRosterEntry(rosterSheet, removeObjectList);
                    break;
            }

            UpdateEntry("O", $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", rosterSheet);
            UpdateEntry("P", $"{discordUser}", rosterSheet);
        }

        public void UpdateMatchResult(TeamModel teamOne, TeamModel teamTwo, int teamOneMapsWon, int teamTwoMapsWon,
            string matchResultSheet, string winner, string loser, int id, string discordUsername)
        {
            _rowId = id;

            UpdateEntry("C", teamOne.Id, matchResultSheet);
            UpdateEntry("E", teamOneMapsWon.ToString(), matchResultSheet);
            UpdateEntry("F", teamTwoMapsWon.ToString(), matchResultSheet);
            UpdateEntry("G", teamTwo.Id, matchResultSheet);
            UpdateEntry("I", teamTwoMapsWon.ToString(), matchResultSheet);
            UpdateEntry("J", teamOneMapsWon.ToString(), matchResultSheet);
            UpdateEntry("L", winner, matchResultSheet);
            UpdateEntry("M", loser, matchResultSheet);
            UpdateEntry("N", $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", matchResultSheet);
            UpdateEntry("O", $"{discordUsername}", matchResultSheet);
        }

        public void UpdatePlayerStats(List<object> updateDataList, string playerStatSheet, int rowId)
        {
            UpdateResponseMessage = string.Empty;

            try
            {
                var range = $"{playerStatSheet}!B{rowId}:M{rowId}";
                var valueRange = new ValueRange();
                valueRange.Values = new List<IList<object>> { updateDataList };

                var updateRequest = _service.Spreadsheets.Values.Update(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                var updateResponse = updateRequest.ExecuteAsync();

                updateResponse.Wait();
            }
            catch (Exception ex)
            {
                UpdateResponseMessage = $"UPDATE PLAYER STATS EXCEPTION:\r{ex}";
            }
        }

        private void UpdateEntry(string rowLetter, string rowData, string sheetName)
        {
            UpdateResponseMessage = string.Empty;

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

        private void UpdateRosterEntry(string rosterSheetName, List<object> dataList)
        {
            UpdateResponseMessage = string.Empty;

            try
            {
                var range = $"{rosterSheetName}!C{_rowId}:N{_rowId}";
                var valueRange = new ValueRange();
                valueRange.Values = new List<IList<object>> { dataList };

                var updateRequest = _service.Spreadsheets.Values.Update(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                var updateResponse = updateRequest.ExecuteAsync();

                updateResponse.Wait();
            }
            catch (Exception ex)
            {
                UpdateResponseMessage = $"{ex}";
            }
        }

        private List<object> RemovePlayerFromRoster(List<PlayerModel> removePlayerList, RosterModel roster)
        {
            List<object> playerIdList = new List<object>()
            {
                "", //0
                "=IFERROR(VLOOKUP(C:C, Player!A:B, 2, false), \"\")", //1
                "", //2
                "=IFERROR(VLOOKUP(E:E, Player!A:B, 2, false), \"\")", //3
                "", //4
                "=IFERROR(VLOOKUP(G:G, Player!A:B, 2, false), \"\")", //5
                "", //6
                "=IFERROR(VLOOKUP(I:I, Player!A:B, 2, false), \"\")", //7
                "", //8
                "=IFERROR(VLOOKUP(K:K, Player!A:B, 2, false), \"\")", //9
                "", //10
                "=IFERROR(VLOOKUP(M:M, Player!A:B, 2, false), \"\")", //11
            };

            if (!string.IsNullOrWhiteSpace(roster.PlayerOne))
            {
                playerIdList[0] = roster.PlayerOne;
            }

            if (!string.IsNullOrWhiteSpace(roster.PlayerTwo))
            {
                playerIdList[2] = roster.PlayerTwo;
            }

            if (!string.IsNullOrWhiteSpace(roster.PlayerThree))
            {
                playerIdList[4] = roster.PlayerThree;
            }

            if (!string.IsNullOrWhiteSpace(roster.PlayerFour))
            {
                playerIdList[6] = roster.PlayerFour;
            }

            if (!string.IsNullOrWhiteSpace(roster.PlayerFive))
            {
                playerIdList[8] = roster.PlayerFive;
            }

            if (!string.IsNullOrWhiteSpace(roster.PlayerSix))
            {
                playerIdList[10] = roster.PlayerSix;
            }

            foreach (PlayerModel player in removePlayerList)
            {
                var index = playerIdList.IndexOf(player.Id);
                if (index != -1)
                {
                    playerIdList[index] = "";
                }
            }

            return playerIdList;
        }

        private List<object> AddPlayerToRoster(List<PlayerModel> addPlayerList, RosterModel roster)
        {
            List<object> playerIdList = new List<object>()
            {
                "", //0
                "=IFERROR(VLOOKUP(C:C, Player!A:B, 2, false), \"\")", //1
                "", //2
                "=IFERROR(VLOOKUP(E:E, Player!A:B, 2, false), \"\")", //3
                "", //4
                "=IFERROR(VLOOKUP(G:G, Player!A:B, 2, false), \"\")", //5
                "", //6
                "=IFERROR(VLOOKUP(I:I, Player!A:B, 2, false), \"\")", //7
                "", //8
                "=IFERROR(VLOOKUP(K:K, Player!A:B, 2, false), \"\")", //9
                "", //10
                "=IFERROR(VLOOKUP(M:M, Player!A:B, 2, false), \"\")", //11
            };

            if (!string.IsNullOrWhiteSpace(roster.PlayerOne))
            {
                playerIdList[0] = roster.PlayerOne;
            }

            if (!string.IsNullOrWhiteSpace(roster.PlayerTwo))
            {
                playerIdList[2] = roster.PlayerTwo;
            }

            if (!string.IsNullOrWhiteSpace(roster.PlayerThree))
            {
                playerIdList[4] = roster.PlayerThree;
            }

            if (!string.IsNullOrWhiteSpace(roster.PlayerFour))
            {
                playerIdList[6] = roster.PlayerFour;
            }

            if (!string.IsNullOrWhiteSpace(roster.PlayerFive))
            {
                playerIdList[8] = roster.PlayerFive;
            }

            if (!string.IsNullOrWhiteSpace(roster.PlayerSix))
            {
                playerIdList[10] = roster.PlayerSix;
            }

            int playerListIndex = 0;
            for (int index = 0; index < 12; index += 2)
            {
                if (string.IsNullOrWhiteSpace(playerIdList[index].ToString()))
                {
                    if (playerListIndex < addPlayerList.Count)
                    {
                        playerIdList[index] = addPlayerList[playerListIndex].Id;
                        playerListIndex++;
                    }
                }
            }

            return playerIdList;
        }
    }
}
