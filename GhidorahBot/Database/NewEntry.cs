using Discord.WebSocket;
using GhidorahBot.Models;
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
        public string NewEntryResponseMessage = string.Empty;

        private SheetsService _service;
        private IConfiguration _config;

        public NewEntry(IConfiguration config, SheetsService service)
        {
            _config = config;
            _service = service;
        }

        /// <summary>
        /// Create new Team
        /// </summary>
        /// <param name="objectList"></param>
        /// <param name="teamSheet"></param>
        public void AddNewTeam(List<object> objectList, string teamSheet)
        {
            try
            {
                var range = $"{teamSheet}!A:I";
                var valueRange = new ValueRange();

                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.Execute();
            }
            catch(Exception ex)
            {
                NewEntryResponseMessage = $"ERROR: {ex}";
            }

        }

        /// <summary>
        /// Create new Player
        /// </summary>
        /// <param name="objectList"></param>
        /// <param name="playerSheet"></param>
        public void AddNewPlayer(List<object> objectList,string playerSheet)
        {
            try
            {
                var range = $"{playerSheet}!A:G";
                var valueRange = new ValueRange();

                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.Execute();
            }
            catch(Exception ex)
            {
                NewEntryResponseMessage = $"ERROR: {ex}";
            }
        }

        /// <summary>
        /// Create new Match Result
        /// </summary>
        /// <param name="objectList"></param>
        /// <param name="matchResultSheet"></param>
        public void AddNewMatchResult(List<object> objectList, string matchResultSheet)
        {
            try
            {
                var range = $"{matchResultSheet}!A:O";
                var valueRange = new ValueRange();

                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.ExecuteAsync();
            }
            catch(Exception ex)
            {
                NewEntryResponseMessage = $"ERROR: {ex}";
            }
        }

        public void AddNewPlayerStats(List<object> objectList, string playerStatsSheet)
        {
            try
            {
                var range = $"{playerStatsSheet}!A:M";
                var valueRange = new ValueRange();

                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.ExecuteAsync();
            }
            catch(Exception ex)
            {
                NewEntryResponseMessage = $"ERROR: {ex}";
            }
        }
        public void RegisterNewAdminChnl(List<object> objectList, string sheetName)
        {
            try
            {
                var range = $"{sheetName}!A:C";
                var valueRange = new ValueRange();

                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.ExecuteAsync();
            }
            catch(Exception ex)
            {
                NewEntryResponseMessage = $"ERROR: {ex}";
            }
        }
    }
}
