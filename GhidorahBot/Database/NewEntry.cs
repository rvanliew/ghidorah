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
        public int Id = 0;
        public Guid Guid = Guid.Empty;
        public string TeamName;
        public string ActivisionId;

        private SheetsService _service;
        private IConfiguration _config;
        private static readonly string _vLookUpTeamOne = "=IFERROR(VLOOKUP(C:C, Team!A:B, 2, false), \"\")";
        private static readonly string _vLookUpTeamTwo = "=IFERROR(VLOOKUP(G:G, Team!A:B, 2, false), \"\")";
        private static readonly string _vLoopUpPlayer = "=IFERROR(VLOOKUP(C:C, Player!A:B, 2, false), \"\")";
        private static readonly string _playerSheet = "Player";
        private static readonly string _teamSheet = "Team";
        private static readonly string _matchResultSheet = "MatchResult";

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

        public void AddNewTeam(SocketModal modal)
        {
            IncrementId(_teamSheet);

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

            var range = $"{_teamSheet}!A:G";
            var valueRange = new ValueRange();

            var objectList = new List<object>() { Id, TeamName.ToUpper(), twitter, group.ToUpper(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", active.ToUpper() };

            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
        }

        public void AddNewPlayer(SocketModal modal)
        {
            IncrementId(_playerSheet);

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

            var range = $"{_playerSheet}!A:G";
            var valueRange = new ValueRange();

            var objectList = new List<object>() { Id, ActivisionId, discordName, twitter, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", active.ToUpper() };

            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
        }

        public void AddNewMatchResult(TeamModel teamOne, TeamModel teamTwo, string winner, string loser, int teamOneMapsWon, int teamTwoMapsWon, string group)
        {
            int _rowId = 0;
            string _winner = string.Empty;
            string _loser = string.Empty;
            IncrementId(_matchResultSheet);  
            var range = $"{_matchResultSheet}!A:N";
            var valueRange = new ValueRange();

            Guid = Guid.NewGuid();
            _rowId = Id + 1;

            if (teamOne.TeamName.ToUpper() == winner.ToUpper())
            {
                _winner = $"=IFERROR(D{_rowId}, \"\")";
                _loser = $"=IFERROR(H{_rowId}, \"\")";
            }
            else if (teamTwo.TeamName.ToUpper() == winner.ToUpper())
            {
                _winner = $"=IFERROR(H{_rowId}, \"\")";
                _loser = $"=IFERROR(D{_rowId}, \"\")";
            }

            var objectList = new List<object>()
            {
                Id,
                Guid,
                teamOne.Id,
                _vLookUpTeamOne,
                teamOneMapsWon,
                teamTwoMapsWon,
                teamTwo.Id,
                _vLookUpTeamTwo,
                teamTwoMapsWon,
                teamOneMapsWon,
                group.ToUpper(),
                _winner,
                _loser,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.ExecuteAsync();
        }

        public void AddNewPlayerStats(string sheetName, string guid, PlayerModel player, StatsModel stats, MapModeModel mapMode)
        {
            IncrementId(sheetName);
            var range = $"{sheetName}!A:L";
            var valueRange = new ValueRange();

            var objectList = new List<object>() {
                Id,
                guid,
                player.Id,
                _vLoopUpPlayer,
                mapMode.Map,
                mapMode.Mode,
                stats.Kills,
                stats.Deaths,
                stats.HillTime,
                stats.BombsPlanted,
                stats.ObjKills,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };

            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.ExecuteAsync();
        }
    }
}
