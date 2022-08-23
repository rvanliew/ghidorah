using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GhidorahBot.Global;

namespace GhidorahBot.Database
{
    public class CreateCodLeague
    {
        private CreateNewSpreadSheet _createNewSpreadSheet { get; set; }
        private Search _search { get; set; }
        private NewEntry _newEntry { get; set; }

        private List<object> _rosterHeader = new List<object>()
            {
                "Id",
                "TeamName",
                "P1_Id",
                "Player1",
                "P2_Id",
                "Player2",
                "P3_Id",
                "Player3",
                "P4_Id",
                "Player4",
                "P5_Id",
                "Player5",
                "P6_Id",
                "Player6",
                "LastUpdatedDt",
                "LastUpdatedBy"
            };
        private List<object> _teamHeader = new List<object>()
            {
                "Id",
                "TeamName",
                "Twitter",
                "Group",
                "TeamCaptain",
                "TeamManager",
                "LastUpdated",
                "LastUpdatedBy",
                "DateCreated",
                "CreatedBy"
            };
        private List<object> _matchResultHeader = new List<object>()
            {
                "Id",
                "GUID",
                "T1_Id",
                "TeamOneName",
                "T1MW",
                "T1MW",
                "T2_Id",
                "TeamTwoName",
                "T2MW",
                "T2ML",
                "Group",
                "Winner",
                "Loser",
                "LastUpdatedDate",
                "LastUpdatedBy",
                "MatchDate"
            };
        private List<object> _teamStatsTotalsHeader = new List<object>()
            {
                "Id",
                "TeamName",
                "Group",
                "MapW(Col1)",
                "MapW(Col2)",
                "MapL(Col1)",
                "MapL(Col2)",
                "MapsWonTotal",
                "MapsLostTotal",
                "MapW%",
                "MatchWins",
                "MatchLoss",
                "TotalMatches",
                "MatchW%"
            };
        private List<object> _playerHeader = new List<object>()
            {
                "Id",
                "ActivisionId",
                "DiscordName",
                "Twitter",
                "LastUpdatedDate",
                "CreatedDate",
                "CreatedBy"
            };
        private List<object> _playerStatsHeader = new List<object>()
            {
                "Id",
                "GUID",
                "PlayerId",
                "PlayerName",
                "Map",
                "Mode",
                "Kills",
                "Deaths",
                "HillTime",
                "BombsPlanted",
                "ObjKills",
                "LastUpdatedDate",
                "LastUpdatedBy",
                "DateCreated",
                "KdRatio"
            };
        private List<object> _playerStatsTotalsHeader = new List<object>()
            {
                "PlayerName",
                "Kills",
                "Deaths",
                "HillTime",
                "BombsPlanted",
                "ObjKills",
                "KdRatio"
            };
        private List<string> _standardLeagueSheets = new List<string>()
        {
            "Roster",
            "Team",
            "MatchResult",
            "TeamStatsTotals",
            "Player",
            "PlayerStats",
            "PlayerStatsTotals"
        };
        private List<string> _newSheetNames = new List<string>();

        public CreateCodLeague(CreateNewSpreadSheet createNewSpreadSheet, Search search, NewEntry newEntry)
        {
            _createNewSpreadSheet = createNewSpreadSheet;
            _search = search;
            _newEntry = newEntry;
        }

        public async Task CreateNewCodLeague(string guildName)
        {
            foreach(var sheet in _standardLeagueSheets)
            {
                await _createNewSpreadSheet.CreateSpreadSheetAsync(guildName, sheet);
                _newSheetNames.Add($"{guildName}_{sheet}");
            }

            await CreateHeaders(guildName);
        }

        private async Task CreateHeaders(string guildName)
        {
            foreach(string sheet in _newSheetNames)
            {
                if(sheet.Equals($"{guildName}_{Values.GoogleSheets.roster}"))
                {
                    _newEntry.CreateSpreadSheetHeaderRow(_rosterHeader, sheet, "A", "P");
                }

                if(sheet.Equals($"{guildName}_{Values.GoogleSheets.team}"))
                {
                    _newEntry.CreateSpreadSheetHeaderRow(_teamHeader, sheet, "A", "J");
                }
                
                if(sheet.Equals($"{guildName}_{Values.GoogleSheets.matchResult}"))
                {
                    _newEntry.CreateSpreadSheetHeaderRow(_matchResultHeader, sheet, "A", "P");
                }

                if (sheet.Equals($"{guildName}_{Values.GoogleSheets.teamStatTotal}"))
                {
                    _newEntry.CreateSpreadSheetHeaderRow(_teamStatsTotalsHeader, sheet, "A", "N");
                }

                if (sheet.Equals($"{guildName}_{Values.GoogleSheets.player}"))
                {
                    _newEntry.CreateSpreadSheetHeaderRow(_playerHeader, sheet, "A", "G");
                }

                if (sheet.Equals($"{guildName}_{Values.GoogleSheets.playerStats}"))
                {
                    _newEntry.CreateSpreadSheetHeaderRow(_playerStatsHeader, sheet, "A", "O");
                }

                if (sheet.Equals($"{guildName}_{Values.GoogleSheets.playerStatsTotal}"))
                {
                    _newEntry.CreateSpreadSheetHeaderRow(_playerStatsTotalsHeader, sheet, "A", "G");
                }
            }
        }
    }
}
