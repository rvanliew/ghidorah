using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using GhidorahBot.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;

namespace GhidorahBot.Database
{
    public class Update
    {
        public string TeamName;
        public string ActivisionId;

        private readonly string _teamSheet = "Team";
        private readonly string _playerSheet = "Player";
        private int _rowId;

        private IConfiguration _config;
        private SheetsService _service;

        public Update(IConfiguration config, SheetsService service)
        {
            _config = config;
            _service = service;
        }

        public void UpdateTeam(SocketModal modal, string id)
        {
            _rowId = Convert.ToInt32(id);
            _rowId++;

            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            TeamName = components
            .First(x => x.CustomId == "edit_team_Name").Value;

            string twitter = components
            .First(x => x.CustomId == "edit_team_twitter").Value;

            string group = components
            .First(x => x.CustomId == "edit_team_group").Value;

            string active = components
            .First(x => x.CustomId == "edit_team_active").Value;

            List<string> componentList = new List<string>();
            componentList.Add(TeamName);
            componentList.Add(twitter);
            componentList.Add(group.ToUpper());
            componentList.Add(active.ToUpper());

            for (int index = 0; index < componentList.Count; index++)
            {
                if (!string.IsNullOrWhiteSpace(componentList[index]))
                {
                    switch (index)
                    {
                        case 0:
                            UpdateEntry("B", TeamName, _teamSheet);
                            break;
                        case 1:
                            UpdateEntry("C", twitter, _teamSheet);
                            break;
                        case 2:
                            UpdateEntry("D", group.ToUpper(), _teamSheet);
                            break;
                        case 3:
                            UpdateEntry("G", active.ToUpper(), _teamSheet);
                            break;
                    }
                }
            }

            UpdateLastUpdatedDate(_teamSheet);
        }

        public void UpdatePlayer(SocketModal modal, string id)
        {
            _rowId = Convert.ToInt32(id);
            _rowId++;

            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            ActivisionId = components
            .First(x => x.CustomId == "edit_player_activ_id").Value;

            string discordName = components
            .First(x => x.CustomId == "edit_player_discord_name").Value;

            string twitter = components
            .First(x => x.CustomId == "edit_player_twitter").Value;

            string active = components
            .First(x => x.CustomId == "edit_player_active").Value;

            List<string> componentList = new List<string>();
            componentList.Add(ActivisionId);
            componentList.Add(discordName);
            componentList.Add(twitter);
            componentList.Add(active);

            for (int index = 0; index < componentList.Count; index++)
            {
                if (!string.IsNullOrWhiteSpace(componentList[index]))
                {
                    switch (index)
                    {
                        case 0:
                            UpdateEntry("B", ActivisionId, _playerSheet);
                            break;
                        case 1:
                            UpdateEntry("C", discordName, _playerSheet);
                            break;
                        case 2:
                            UpdateEntry("D", twitter, _playerSheet);
                            break;
                        case 3:
                            UpdateEntry("G", active.ToUpper(), _playerSheet);
                            break;
                    }
                }
            }

            UpdateLastUpdatedDate(_playerSheet);
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
