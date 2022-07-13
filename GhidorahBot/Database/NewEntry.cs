using Discord.WebSocket;
using GhidorahBot.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GhidorahBot.Database
{
    public class NewEntry
    {
        public int Id = 0;
        public Guid Guid = Guid.Empty;
        public string TeamName;
        public string ActivisionId;

        private SheetsService _service;
        private IConfiguration _config;

        public NewEntry(IConfiguration config, SheetsService service)
        {
            _config = config;
            _service = service;
        }

        private void IncrementId(string sheetName)
        {
            Id = 0;

            var range = $"{sheetName}!A2:A";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

            var reponse = request.ExecuteAsync();
            var requestResponse = reponse.Result;

            if (requestResponse != null && requestResponse.Values != null && requestResponse.Values.Count > 0)
            {
                var lastId = requestResponse.Values.Last();
                Id = Convert.ToInt32(lastId[0]);
                Id++;
            }
            else
            {
                Id++;
            }
        }

        public void AddNewTeam(SocketModal modal, string sheetName)
        {
            IncrementId(sheetName);

            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            TeamName = components
            .First(x => x.CustomId == "team_Name").Value;

            string twitter = components
            .First(x => x.CustomId == "team_twitter").Value;

            string group = components
            .First(x => x.CustomId == "team_group").Value;

            string active = components
            .First(x => x.CustomId == "team_active").Value;

            var range = $"{sheetName}!A:G";
            var valueRange = new ValueRange();

            var objectList = new List<object>() { Id, TeamName, twitter, group.ToUpper(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", active.ToUpper() };

            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
        }

        public void AddNewPlayer(SocketModal modal, string sheetName)
        {
            IncrementId(sheetName);

            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            ActivisionId = components
            .First(x => x.CustomId == "player_activ_id").Value;

            string discordName = components
            .First(x => x.CustomId == "player_discord_name").Value;

            string twitter = components
            .First(x => x.CustomId == "player_twitter").Value;

            string active = components
            .First(x => x.CustomId == "player_active").Value;

            var range = $"{sheetName}!A:G";
            var valueRange = new ValueRange();

            var objectList = new List<object>() { Id, ActivisionId, discordName, twitter, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", active.ToUpper() };

            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
        }

        public void AddNewMatchResult(SocketModal modal, string sheetName)
        {
            IncrementId(sheetName);

            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            Guid = Guid.NewGuid();

            string teamOneName = components
            .First(x => x.CustomId == "team_1_name").Value;

            string teamOneMapsWon = components
            .First(x => x.CustomId == "team_1_maps_won").Value;

            string teamTwoName = components
            .First(x => x.CustomId == "team_2_name").Value;

            string teamTwoMapsWon = components
            .First(x => x.CustomId == "team_2_maps_won").Value;

            string group = components
            .First(x => x.CustomId == "group_id").Value;

            var range = $"{sheetName}!A:I";
            var valueRange = new ValueRange();

            var objectList = new List<object>() { Id, Guid, teamOneName, teamOneMapsWon, teamTwoName, teamTwoMapsWon, group.ToUpper(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };

            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
        }

        public void AddNewPlayerStats(SocketModal modal, string sheetName, string guid, string playerName)
        {
            IncrementId(sheetName);

            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            string mapMode = components
            .First(x => x.CustomId == "newstats_mapmode").Value;

            string stats = components
            .First(x => x.CustomId == "newstats_stats").Value;

            var range = $"{sheetName}!A:K";
            var valueRange = new ValueRange();

            string[] splitStats = stats.Split(",");

            var objectList = new List<object>() { 
                Id,
                guid,
                playerName,
                mapMode, 
                /*kills*/splitStats[0],
                /*deaths*/splitStats[1],
                /*hilltime*/splitStats[2],
                /*bombsplanted*/splitStats[3],
                /*objkills*/splitStats[4],
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };

            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
        }
    }
}
