using Discord.WebSocket;
using GhidorahBot.Models;
using GhidorahBot.Extensions;
using GhidorahBot.Global;
using Google.Apis.Sheets.v4;
using Microsoft.Extensions.Configuration;

namespace GhidorahBot.Database
{
    public class Search
    {
        public string SearchExceptionMsg;

        private IConfiguration _config;
        private SheetsService _service;

        public Search(IConfiguration config, SheetsService service)
        {
            _config = config;
            _service = service;
        }

        public List<PlayerModel> GetPlayerList(string guildId)
        {
            List<PlayerModel> playerList = new List<PlayerModel>();

            var range = $"{guildId}!A2:G";
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

        public List<TeamModel> GetTeamList(string guildId)
        {
            List<TeamModel> teamList = new List<TeamModel>();

            var range = $"{guildId}!A2:I";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

            var response = request.ExecuteAsync();
            var valueRangeResult = response.Result;

            try
            {
                if (valueRangeResult != null && valueRangeResult.Values.Count > 0 && valueRangeResult.Values != null)
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

        public List<RosterModel> GetRosterIdsList(string guildId)
        {
            List<RosterModel> rosterList = new List<RosterModel>();

            var range = $"{guildId}!A2:N";
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

        public List<RosterModel> GetRosterNamesList(string guildId)
        {
            List<RosterModel> rosterList = new List<RosterModel>();

            var range = $"{guildId}!A2:N";
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

        public List<PlayerStatTotalsModel> GetPlayerTotalStats(string guildId)
        {
            List<PlayerStatTotalsModel> playerStatTotalList = new List<PlayerStatTotalsModel>();

            var range = $"{guildId}!A2:H";
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
                                    row[6].ToString()
                                    ));
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

        public List<UpdatePlayerStatsModel> GetPlayerStatsTableData(string guildId)
        {
            List<UpdatePlayerStatsModel> playerStatDataList = new List<UpdatePlayerStatsModel>();

            var range = $"{guildId}!A2:N";
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
                            playerStatDataList.Add(new UpdatePlayerStatsModel(
                                    row[0].ToString(),
                                    row[1].ToString(),
                                    row[2].ToString(),
                                    row[3].ToString(),
                                    row[4].ToString(),
                                    row[5].ToString(),
                                    row[6].ToString(),
                                    row[7].ToString(),
                                    row[8].ToString(),
                                    row[9].ToString(),
                                    row[10].ToString(),
                                    row[11].ToString(),
                                    row[12].ToString(),
                                    row[13].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                playerStatDataList.Clear();
                SearchExceptionMsg = $"{ex}";
            }

            return playerStatDataList;
        }

        public List<TeamStatTotalsModel> GetTeamTotalStats(string guildId)
        {
            List<TeamStatTotalsModel> teamStatTotalList = new List<TeamStatTotalsModel>();

            var range = $"{guildId}!A2:N";
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
                            row[2].ToString(),
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

        public List<MatchResultModel> GetMatchResultList(string guildId)
        {
            List<MatchResultModel> matchResultList = new List<MatchResultModel>();

            var range = $"{guildId}!A2:J";
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
            var range = $"{Values.GoogleSheets.maps}!A2:D";
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
                                    row[1].ToString(),
                                    row[2].ToString(),
                                    row[3].ToString()));
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
            var range = $"{Values.GoogleSheets.gamemodes}!A2:E";
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
                                    row[1].ToString(),
                                    row[2].ToString(),
                                    row[3].ToString(),
                                    row[4].ToString()));
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

        public List<ChannelModel> GetRegisteredChannelInfoList()
        {
            List<ChannelModel> channelIdList = new List<ChannelModel>();
            var range = $"{Values.GoogleSheets.registerChannel}!A2:D";
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
                            row[2].ToString(),
                            row[3].ToString()
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
                if (valueRangeResult != null && valueRangeResult.Values.Count > 0 && valueRangeResult.Values != null)
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

        public RosterModel SearchRoster(RosterModel args, string guildId)
        {
            SearchExceptionMsg = "";
            var rosterList = GetRosterIdsList(guildId);
            RosterModel searchResult = null;         
            try
            {
                var id = args.Id;
                searchResult = rosterList.Single(s => s.Id == id);
                return searchResult;
            }
            catch (Exception ex)
            {
                SearchExceptionMsg = $"Roster Search: Search contains no matching element";
            }
            
            return searchResult;
        }

        public RosterModel SearchRosterSingle(string args, string guildId)
        {
            SearchExceptionMsg = "";
            var rosterList = GetRosterNamesList(guildId);
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

        public RosterModel SearchRosterForPlayer(string args, string guildId)
        {
            SearchExceptionMsg = "";
            var rosterList = GetRosterNamesList(guildId);
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

        public TeamModel SearchTeam(string args, string guildId)
        {
            SearchExceptionMsg = "";
            var teamList = GetTeamList(guildId);
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

        public PlayerModel SearchPlayer(string args, string guildId)
        {
            SearchExceptionMsg = "";
            var playerList = GetPlayerList(guildId);
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

        public MatchResultModel SearchMatchResult(string args, string guildId)
        {
            SearchExceptionMsg = "";
            var matchResultList = GetMatchResultList(guildId);
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

        public UpdatePlayerStatsModel SearchPlayerStatsDataTable(string guid, string rowId, string guildId)
        {
            SearchExceptionMsg = "";
            var playerStatsDataTableList = GetPlayerStatsTableData(guildId);
            UpdatePlayerStatsModel searchResult = null;
            try
            {
                searchResult = playerStatsDataTableList.Single(s => s.Guid.ToUpper() == guid.ToUpper() && s.Id == rowId);
                return searchResult;
            }
            catch
            {
                SearchExceptionMsg = "Player Stats Table: Search contains no matching element";
            }

            return searchResult;
        }

        public PlayerStatTotalsModel SearchPlayerStatTotals(string args, string guildId)
        {
            SearchExceptionMsg = "";
            var playerTotalStatsList = GetPlayerTotalStats(guildId);
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

        public TeamStatTotalsModel SearchTeamStatTotals(string args, string guildId)
        {
            SearchExceptionMsg = "";
            var teamTotalStatsList = GetTeamTotalStats(guildId);
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
            SearchExceptionMsg = "";

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

        public List<ChannelModel> SearchChannelTable(ulong guildId)
        {
            SearchExceptionMsg = "";
            List<ChannelModel> searchResultList = new List<ChannelModel>();
            string strGuildId = guildId.ToString();

            try
            {
                var searchList = GetRegisteredChannelInfoList();
                foreach(ChannelModel channel in searchList)
                {
                    if(channel.GuildId == strGuildId)
                    {
                        searchResultList.Add(channel);
                    }
                }

                return searchResultList;
            }
            catch(Exception ex)
            {
                SearchExceptionMsg = "Guild Id search contains no matching element.\r" +
                    "Please make sure you have registered an admin channel.";
            }

            return searchResultList;
        }
    }
}
