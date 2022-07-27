using Discord.WebSocket;
using GhidorahBot.Models;
using GhidorahBot.Extensions;
using Google.Apis.Sheets.v4;
using Microsoft.Extensions.Configuration;

namespace GhidorahBot.Database
{
    public class Search
    {
        public string SearchExceptionMsg;

        private IConfiguration _config;
        private SheetsService _service;

        //SheetNames
        private static readonly string _playerSheet = "Player";
        private static readonly string _teamSheet = "Team";
        private static readonly string _rosterSheet = "Roster";
        private static readonly string _playerStatTotalsSheet = "PlayerStatsTotals";
        private static readonly string _matchResultSheet = "MatchResult";

        public Search(IConfiguration config, SheetsService service)
        {
            _config = config;
            _service = service;
        }

        public List<PlayerModel> GetPlayerList()
        {
            List<PlayerModel> playerList = new List<PlayerModel>();

            var range = $"{_playerSheet}!A2:G";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

            var response = request.ExecuteAsync();
            var valueRangeResult = response.Result;

            try
            {
                if (valueRangeResult != null && valueRangeResult.Values.Count > 0)
                {
                    foreach (var row in valueRangeResult.Values)
                    {
                        if (row.Count >= 7)
                        {
                            playerList.Add(new PlayerModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[2].ToString(),
                                    row[3].ToString(),
                                    row[4].ToString(),
                                    row[5].ToString(),
                                    row[6].ToString()));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                playerList.Clear();
                SearchExceptionMsg = $"{ex}";
            }

            return playerList;
        }

        public List<TeamModel> GetTeamList()
        {
            List<TeamModel> teamList = new List<TeamModel>();

            var range = $"{_teamSheet}!A2:G";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

            var response = request.ExecuteAsync();
            var valueRangeResult = response.Result;

            try
            {
                if (valueRangeResult != null && valueRangeResult.Values.Count > 0)
                {
                    foreach (var row in valueRangeResult.Values)
                    {
                        if (row.Count >= 7)
                        {
                            teamList.Add(new TeamModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[2].ToString(),
                                    row[3].ToString(),
                                    row[4].ToString(),
                                    row[5].ToString(),
                                    row[6].ToString()));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                teamList.Clear();
                SearchExceptionMsg = $"{ex}";
            }       

            return teamList;
        }

        public List<RosterModel> GetRosterIdsList()
        {
            List<RosterModel> rosterList = new List<RosterModel>();

            var range = $"{_rosterSheet}!A2:N";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

            var response = request.ExecuteAsync();
            var valueRangeResult = response.Result;

            try
            {
                if (valueRangeResult != null && valueRangeResult.Values.Count > 0)
                {
                    foreach (var row in valueRangeResult.Values)
                    {
                        switch (row.Count)
                        {
                            case 2:
                                rosterList.Add(new RosterModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    "",
                                    "",
                                    "",
                                    "",
                                    "",
                                    ""));
                                break;
                            case 4:
                                rosterList.Add(new RosterModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[2].ToString(),
                                    "",
                                    "",
                                    "",
                                    "",
                                    ""));
                                break;
                            case 6:
                                rosterList.Add(new RosterModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[2].ToString(),
                                    row[4].ToString(),
                                    "",
                                    "",
                                    "",
                                    ""));
                                break;
                            case 8:
                                rosterList.Add(new RosterModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[2].ToString(),
                                    row[4].ToString(),
                                    row[6].ToString(),
                                    "",
                                    "",
                                    ""));
                                break;
                            case 10:
                                rosterList.Add(new RosterModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[2].ToString(),
                                    row[4].ToString(),
                                    row[6].ToString(),
                                    row[8].ToString(),
                                    "",
                                    ""));
                                break;
                            case 12:
                                rosterList.Add(new RosterModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[2].ToString(),
                                    row[4].ToString(),
                                    row[6].ToString(),
                                    row[8].ToString(),
                                    row[10].ToString(),
                                    ""));
                                break;
                            case 14:
                                rosterList.Add(new RosterModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[2].ToString(),
                                    row[4].ToString(),
                                    row[6].ToString(),
                                    row[8].ToString(),
                                    row[10].ToString(),
                                    row[12].ToString()));
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rosterList.Clear();
                SearchExceptionMsg = $"{ex}";
            }

            return rosterList;
        }

        public List<RosterModel> GetRosterNamesList()
        {
            List<RosterModel> rosterList = new List<RosterModel>();

            var range = $"{_rosterSheet}!A2:N";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

            var response = request.ExecuteAsync();
            var valueRangeResult = response.Result;

            try
            {
                if (valueRangeResult != null && valueRangeResult.Values.Count > 0)
                {
                    foreach (var row in valueRangeResult.Values)
                    {
                        switch (row.Count)
                        {
                            case 2:
                                rosterList.Add(new RosterModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    "",
                                    "",
                                    "",
                                    "",
                                    "",
                                    ""));
                                break;
                            case 4:
                                rosterList.Add(new RosterModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[3].ToString(),
                                    "",
                                    "",
                                    "",
                                    "",
                                    ""));
                                break;
                            case 6:
                                rosterList.Add(new RosterModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[3].ToString(),
                                    row[5].ToString(),
                                    "",
                                    "",
                                    "",
                                    ""));
                                break;
                            case 8:
                                rosterList.Add(new RosterModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[3].ToString(),
                                    row[5].ToString(),
                                    row[7].ToString(),
                                    "",
                                    "",
                                    ""));
                                break;
                            case 10:
                                rosterList.Add(new RosterModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[3].ToString(),
                                    row[5].ToString(),
                                    row[7].ToString(),
                                    row[9].ToString(),
                                    "",
                                    ""));
                                break;
                            case 12:
                                rosterList.Add(new RosterModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[3].ToString(),
                                    row[5].ToString(),
                                    row[7].ToString(),
                                    row[9].ToString(),
                                    row[11].ToString(),
                                    ""));
                                break;
                            case 14:
                                rosterList.Add(new RosterModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[3].ToString(),
                                    row[5].ToString(),
                                    row[7].ToString(),
                                    row[9].ToString(),
                                    row[11].ToString(),
                                    row[13].ToString()));
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rosterList.Clear();
                SearchExceptionMsg = $"{ex}";
            }

            return rosterList;
        }

        public List<PlayerStatTotalsModel> GetPlayerTotalStats()
        {
            List<PlayerStatTotalsModel> playerStatTotalList = new List<PlayerStatTotalsModel>();

            var range = $"{_playerStatTotalsSheet}!A2:H";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

            var response = request.ExecuteAsync();
            var valueRangeResult = response.Result;

            try
            {
                if (valueRangeResult != null && valueRangeResult.Values.Count > 0)
                {
                    foreach (var row in valueRangeResult.Values)
                    {
                        if (row.Count >= 7)
                        {
                            playerStatTotalList.Add(new PlayerStatTotalsModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[2].ToString(),
                                    row[3].ToString(),
                                    row[4].ToString(),
                                    row[5].ToString(),
                                    row[6].ToString(),
                                    row[7].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                playerStatTotalList.Clear();
                SearchExceptionMsg = $"{ex}";
            }

            return playerStatTotalList;
        }

        public List<MatchResultModel> GetMatchResultList()
        {
            List<MatchResultModel> matchResultList = new List<MatchResultModel>();

            var range = $"{_matchResultSheet}!A2:J";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

            var response = request.ExecuteAsync();
            var valueRangeResult = response.Result;

            try
            {
                if (valueRangeResult != null && valueRangeResult.Values.Count > 0)
                {
                    foreach (var row in valueRangeResult.Values)
                    {
                        if (row.Count >= 10)
                        {
                            matchResultList.Add(new MatchResultModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[2].ToString(),
                                    row[3].ToString(),
                                    row[4].ToString(),
                                    row[5].ToString(),
                                    row[6].ToString(),
                                    row[7].ToString(),
                                    row[8].ToString(),
                                    row[9].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                matchResultList.Clear();
                SearchExceptionMsg = $"{ex}";
            }

            return matchResultList;
        }

        public List<string> GetGuidList(string sheetName,string definedRange ,string args)
        {
            List<string> guidList = new List<string>();
            var range = $"{sheetName}!{definedRange}";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

            var response = request.ExecuteAsync();
            var valueRangeResult = response.Result;

            try
            {
                if (valueRangeResult != null && valueRangeResult.Values.Count > 0)
                {
                    foreach (var row in valueRangeResult.Values)
                    {
                        guidList.Add(row[0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                SearchExceptionMsg = $"Exception: {ex}";
            }

            return guidList;
        }

        public RosterModel SearchRoster(RosterModel args)
        {
            var rosterList = GetRosterIdsList();
            RosterModel searchResult;
            var id = args.Id;
            searchResult = rosterList.Single(s => s.Id == id);

            return searchResult;
        }

        public RosterModel SearchRosterSingle(string args)
        {
            var rosterList = GetRosterNamesList();
            RosterModel searchResult;
            searchResult = rosterList.Single(r => r.TeamName.ToUpper() == args.ToUpper());

            return searchResult;
        }

        public TeamModel SearchTeam(string args)
        {
            var teamList = GetTeamList();
            TeamModel searchResult;
            searchResult = teamList.Single(t => t.TeamName.ToUpper() == args.ToUpper());

            return searchResult;
        }

        public PlayerModel SearchPlayer(string args)
        {
            var playerList = GetPlayerList();
            PlayerModel searchResult;
            searchResult = playerList.Single(p => p.ActivsionId.ToUpper() == args.ToUpper());

            return searchResult;
        }

        public MatchResultModel SearchMatchResult(string args)
        {
            var matchResultList = GetMatchResultList();
            MatchResultModel searchResult;
            searchResult = matchResultList.Single(m => m.GUID.ToUpper() == args.ToUpper());

            return searchResult;
        }

        public PlayerStatTotalsModel SearchPlayerStatTotals(string args)
        {
            var playerTotalStatsList = GetPlayerTotalStats();
            PlayerStatTotalsModel searchResult;
            searchResult = playerTotalStatsList.Single(s => s.PlayerName.ToUpper() == args.ToUpper());

            return searchResult;
        }

        public string SearchGUID(string sheetName, string definedRange, string args)
        {
            var guidList = GetGuidList(sheetName, definedRange, args);
            string searchResult = string.Empty;
            searchResult = guidList.Single(g => g.ToUpper().Equals(args.ToUpper()));

            return searchResult;
        }
    }
}
