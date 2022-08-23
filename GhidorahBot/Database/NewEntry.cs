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
                var appendResponse = appendRequest.ExecuteAsync();

                appendResponse.Wait();
            }
            catch(Exception ex)
            {
                NewEntryResponseMessage = $"ERROR: {ex}";
            }
        }
        public void AddNewTeamTotalStatsRow(List<object> objectList, string teamTotalsSheet)
        {
            try
            {
                var range = $"{teamTotalsSheet}!A:N";
                var valueRange = new ValueRange();

                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.ExecuteAsync();

                appendResponse.Wait();
            }
            catch (Exception ex)
            {
                NewEntryResponseMessage = $"ERROR: {ex}";
            }
        }
        public void AddNewRosterRow(List<object> objectList, string rosterSheet)
        {
            try
            {
                var range = $"{rosterSheet}!A:P";
                var valueRange = new ValueRange();

                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.ExecuteAsync();

                appendResponse.Wait();
            }
            catch (Exception ex)
            {
                NewEntryResponseMessage = $"ERROR: {ex}";
            }
        }
        public void AddNewPlayerTotalStatsRow(List<object> objectList, string playerTotalsSheets)
        {
            try
            {
                var range = $"{playerTotalsSheets}!A:G";
                var valueRange = new ValueRange();

                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.ExecuteAsync();

                appendResponse.Wait();
            }
            catch (Exception ex)
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
                var range = $"{playerSheet}!A:H";
                var valueRange = new ValueRange();

                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.ExecuteAsync();

                appendResponse.Wait();
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
                var range = $"{matchResultSheet}!A:P";
                var valueRange = new ValueRange();

                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.ExecuteAsync();

                appendResponse.Wait();
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
                var range = $"{playerStatsSheet}!A:N";
                var valueRange = new ValueRange();

                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.ExecuteAsync();

                appendResponse.Wait();
            }
            catch(Exception ex)
            {
                NewEntryResponseMessage = $"ERROR: {ex}";
            }
        }
        public void AddNewLeagueCredentials(List<object> objectList, string leagueCredentialsSheet)
        {
            try
            {
                var range = $"{leagueCredentialsSheet}!A:E";
                var valueRange = new ValueRange();

                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.ExecuteAsync();

                appendResponse.Wait();
            }
            catch (Exception ex)
            {
                NewEntryResponseMessage = $"ERROR: {ex}";
            }
        }
        public void CreateSpreadSheetHeaderRow(List<object> headerList, string sheetName, string columnOne, string columnTwo)
        {
            try
            {
                var range = $"{sheetName}!{columnOne}:{columnTwo}";
                var valueRange = new ValueRange();

                valueRange.Values = new List<IList<object>> { headerList };

                var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.ExecuteAsync();

                appendResponse.Wait();
            }
            catch (Exception ex)
            {
                NewEntryResponseMessage = $"ERROR: {ex}";
            }
        }
        public void RegisterNewAdminChnl(List<object> objectList, string sheetName)
        {
            try
            {
                var range = $"{sheetName}!A:D";
                var valueRange = new ValueRange();

                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.ExecuteAsync();

                appendResponse.Wait();
            }
            catch(Exception ex)
            {
                NewEntryResponseMessage = $"ERROR: {ex}";
            }
        }
    }
}
