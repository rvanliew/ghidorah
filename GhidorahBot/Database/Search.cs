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
        private static readonly string _teamStatTotalsSheet = "TeamStatsTotals";
        private static readonly string _matchResultSheet = "MatchResult";
        private static readonly string _mapsSheet = "Maps";
        private static readonly string _gamemodesSheet = "Gamemodes";
        private static readonly string _discordChnlSheet = "AdminChannel";

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
                        if (row.Count >= 6)
                        {
                            playerList.Add(new PlayerModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[2].ToString(),
                                    row[3].ToString(),
                                    row[4].ToString(),
                                    row[5].ToString(),
                                    row[6].ToString())
                                );
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

            var range = $"{_teamSheet}!A2:I";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

            var response = request.ExecuteAsync();
            var valueRangeResult = response.Result;

            try
            {
                if (valueRangeResult != null && valueRangeResult.Values.Count > 0)
                {
                    foreach (var row in valueRangeResult.Values)
                    {
                        if (row.Count >= 8)
                        {
                            teamList.Add(new TeamModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[2].ToString(),
                                    row[3].ToString(),
                                    row[4].ToString(),
                                    row[5].ToString(),
                                    row[6].ToString(),
                                    row[7].ToString(),
                                    row[8].ToString())
                                );
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

        public List<TeamStatTotalsModel> GetTeamTotalStats()
        {
            List<TeamStatTotalsModel> teamStatTotalList = new List<TeamStatTotalsModel>();

            var range = $"{_teamStatTotalsSheet}!A2:N";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

            var response = request.ExecuteAsync();
            var valueRangeResult = response.Result;

            try
            {
                if (valueRangeResult != null && valueRangeResult.Values.Count > 0)
                {
                    foreach (var row in valueRangeResult.Values)
                    {
                        teamStatTotalList.Add(new TeamStatTotalsModel(
                            row[0].ToString(),
                            row[1].ToString(),
                            row[7].ToString(),
                            row[8].ToString(),
                            row[9].ToString(),
                            row[10].ToString(),
                            row[11].ToString(),
                            row[12].ToString(),
                            row[13].ToString()
                            ));
                    }
                }
            }
            catch(Exception ex)
            {
                teamStatTotalList.Clear();
                SearchExceptionMsg = $"{ex}";
            }

            return teamStatTotalList;
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

        public List<MapModel> GetMapList()
        {
            List<MapModel> mapList = new List<MapModel>();
            var range = $"{_mapsSheet}!A2:B";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
            var response = request.ExecuteAsync();
            var valueRangeResult = response.Result;

            try
            {
                if (valueRangeResult != null && valueRangeResult.Values.Count > 0)
                {
                    foreach (var row in valueRangeResult.Values)
                    {
                        if (row.Count >= 2)
                        {
                            mapList.Add(new MapModel(
                                    row[0].ToString(),
                                    row[1].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mapList.Clear();
                SearchExceptionMsg = $"{ex}";
            }

            return mapList;
        }

        public List<GamemodeModel> GetGamemodeList()
        {
            List<GamemodeModel> gamemodeList = new List<GamemodeModel>();
            var range = $"{_gamemodesSheet}!A2:B";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
            var response = request.ExecuteAsync();
            var valueRangeResult = response.Result;

            try
            {
                if (valueRangeResult != null && valueRangeResult.Values.Count > 0)
                {
                    foreach (var row in valueRangeResult.Values)
                    {
                        if (row.Count >= 2)
                        {
                            gamemodeList.Add(new GamemodeModel(
                                    row[0].ToString(),
                                    row[1].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                gamemodeList.Clear();
                SearchExceptionMsg = $"{ex}";
            }

            return gamemodeList;
        }

        public List<ChannelModel> GetDiscordChannelInfoList()
        {
            List<ChannelModel> channelIdList = new List<ChannelModel>();
            var range = $"{_discordChnlSheet}!A2:C";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
            var response = request.ExecuteAsync();
            var valueRangeResult = response.Result;

            try
            {
                if(valueRangeResult != null && valueRangeResult.Values.Count > 0)
                {
                    foreach(var row in valueRangeResult.Values)
                    {
                        channelIdList.Add(new ChannelModel(
                            row[0].ToString(),
                            row[1].ToString(),
                            row[2].ToString()
                            ));
                    }
                }
            }
            catch(Exception ex)
            {
                channelIdList.Clear();
                SearchExceptionMsg = $"{ex}";
            }

            return channelIdList;
        }

        public List<string> GetSingleColumnList(string sheetName,string definedRange ,string args)
        {
            List<string> columnList = new List<string>();
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
                        columnList.Add(row[0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                SearchExceptionMsg = $"Exception: {ex}";
            }

            return columnList;
        }

        public RosterModel SearchRoster(RosterModel args)
        {
            SearchExceptionMsg = "";
            var rosterList = GetRosterIdsList();
            RosterModel searchResult = null;
            var id = args.Id;
            try
            {
                searchResult = rosterList.Single(s => s.Id == id);
                return searchResult;
            }
            catch (Exception ex)
            {
                SearchExceptionMsg = $"Roster Search: Search contains no matching element";
            }
            
            return searchResult;
        }

        public RosterModel SearchRosterSingle(string args)
        {
            SearchExceptionMsg = "";
            var rosterList = GetRosterNamesList();
            RosterModel searchResult = null;
            try
            {
                searchResult = rosterList.Single(r => r.TeamName.ToUpper() == args.ToUpper());
                return searchResult;
            }
            catch (Exception ex)
            {
                SearchExceptionMsg = $"Search Roster Single: Search contains no matching element";
            }           

            return searchResult;
        }

        public RosterModel SearchRosterForPlayer(string args)
        {
            SearchExceptionMsg = "";
            var rosterList = GetRosterNamesList();
            RosterModel searchResult = null;
            try
            {
                foreach(var roster in rosterList)
                {
                    if (args.EqualsAnyOf(
                        roster.PlayerOne,
                        roster.PlayerTwo,
                        roster.PlayerThree,
                        roster.PlayerFour,
                        roster.PlayerFive,
                        roster.PlayerSix))
                    {
                        searchResult = roster;
                        return searchResult;
                    }
                }
            }
            catch
            {
                SearchExceptionMsg = $"Search Roster For Player: Search contains no matching element";
            }

            return searchResult;
        }

        public TeamModel SearchTeam(string args)
        {
            SearchExceptionMsg = "";
            var teamList = GetTeamList();
            TeamModel searchResult = null;
            try
            {
                searchResult = teamList.Single(t => t.TeamName.ToUpper() == args.ToUpper());
                return searchResult;
            }
            catch(Exception ex)
            {
                SearchExceptionMsg = $"Search contains no matching element";
            }

            return searchResult;
        }

        public PlayerModel SearchPlayer(string args)
        {
            SearchExceptionMsg = "";
            var playerList = GetPlayerList();
            PlayerModel searchResult = null;
            try
            {
                searchResult = playerList.Single(p => p.ActivsionId.ToUpper() == args.ToUpper());
                return searchResult;
            }
            catch(Exception ex)
            {
                SearchExceptionMsg = $"Player Search: Search contains no matching element";
            }        

            return searchResult;
        }

        public MapModel SearchMap(string args)
        {
            SearchExceptionMsg = "";
            var mapList = GetMapList();
            MapModel searchResult = null;
            try
            {
                searchResult = mapList.Single(m => m.MapName.ToUpper() == args.ToUpper());
                return searchResult;
            }
            catch(Exception ex)
            {
                SearchExceptionMsg = $"Map search contains no matching element";
            }

            return searchResult;
        }

        public GamemodeModel SearchGamemode(string args)
        {
            SearchExceptionMsg = "";
            var gamemodeList = GetGamemodeList();
            GamemodeModel searchResult = null;
            try
            {
                searchResult = gamemodeList.Single(g => g.ModeName.ToUpper() == args.ToUpper());
                return searchResult;
            }
            catch (Exception ex)
            {
                SearchExceptionMsg = $"Map search contains no matching element";
            }

            return searchResult;
        }

        public MatchResultModel SearchMatchResult(string args)
        {
            SearchExceptionMsg = "";
            var matchResultList = GetMatchResultList();
            MatchResultModel searchResult = null;
            try
            {
                searchResult = matchResultList.Single(m => m.GUID.ToUpper() == args.ToUpper());
                return searchResult;
            }
            catch (Exception ex)
            {
                SearchExceptionMsg = $"Match Result search contains no matching element";
            }
            
            return searchResult;
        }

        public PlayerStatTotalsModel SearchPlayerStatTotals(string args)
        {
            SearchExceptionMsg = "";
            var playerTotalStatsList = GetPlayerTotalStats();
            PlayerStatTotalsModel searchResult = null;
            try
            {
                searchResult = playerTotalStatsList.Single(s => s.PlayerName.ToUpper() == args.ToUpper());
                return searchResult;
            }
            catch (Exception ex)
            {
                SearchExceptionMsg = $"Player Stats Totals: Search contains no matching element";
            }

            return searchResult;
        }

        public TeamStatTotalsModel SearchTeamStatTotals(string args)
        {
            SearchExceptionMsg = "";
            var teamTotalStatsList = GetTeamTotalStats();
            TeamStatTotalsModel searchResult = null;
            try
            {
                searchResult = teamTotalStatsList.Single(t => t.TeamName.ToUpper() == args.ToUpper());
                return searchResult;
            }
            catch
            {
                SearchExceptionMsg = $"Team Stats Totals: Search contains no matching element";
            }

            return searchResult;
        }

        public string SingleColumnSearch(string sheetName, string definedRange, string args)
        {
            string searchResult = string.Empty;

            try
            {
                var searchList = GetSingleColumnList(sheetName, definedRange, args);                
                searchResult = searchList.Single(g => g.ToUpper().Equals(args.ToUpper()));
                return searchResult;
            }
            catch
            {
                SearchExceptionMsg = "Search contains no matching element";
            }

            return searchResult;
        }

        public ChannelModel SearchChannelTable(ulong guildId)
        {
            ChannelModel searchResult = null;

            try
            {
                var searchList = GetDiscordChannelInfoList();
                searchResult = searchList.Single(g => g.GuildId.Equals(guildId.ToString()));
                return searchResult;
            }
            catch(Exception ex)
            {
                SearchExceptionMsg = "Guild Id search contains no matching element";
            }

            return searchResult;
        }
    }
}
