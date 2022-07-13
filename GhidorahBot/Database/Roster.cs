using Discord.WebSocket;
using GhidorahBot.Modals;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace GhidorahBot.Database
{
    public class Roster
    {
        public int Id = 0;

        private IConfiguration _config;
        private SheetsService _service;
        private readonly string _rosterSheetName = "Roster";
        private readonly string _teamSheetName = "Team";
        private readonly string _playerSheetName = "Player";
        private bool _isValid;
        private bool _matchFound;
        private string _id;
        private string _name;
        private string _isActive;

        public Roster(IConfiguration config, SheetsService service)
        {
            _config = config;
            _service = service;
        }

        public void NewRoster(SocketModal modal, string sheetName, List<Player> playersList)
        {
            IncrementId(sheetName);

            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            string teamName = components
                .First(x => x.CustomId == $"newroster_search_team_name").Value;

            var range = $"{sheetName}!A:I";
            var valueRange = new ValueRange();

            var objectList = new List<object>();

            switch(playersList.Count)
            {
                case 4:
                    objectList = new List<object>
                    {
                        Id,
                        teamName,
                        playersList[0].ActivsionId,
                        playersList[1].ActivsionId,
                        playersList[2].ActivsionId,
                        playersList[3].ActivsionId,
                        "",
                        "",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    break;
                case 5:
                    objectList = new List<object>
                    {
                        Id,
                        teamName,
                        playersList[0].ActivsionId,
                        playersList[1].ActivsionId,
                        playersList[2].ActivsionId,
                        playersList[3].ActivsionId,
                        playersList[4].ActivsionId,
                        "",
                        DateTime.Now
                    };
                    break;
                case 6:
                    objectList = new List<object>
                    {
                        Id,
                        teamName,
                        playersList[0].ActivsionId,
                        playersList[1].ActivsionId,
                        playersList[2].ActivsionId,
                        playersList[3].ActivsionId,
                        playersList[4].ActivsionId,
                        playersList[5].ActivsionId,
                        DateTime.Now
                    };
                    break;
            }

            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
        }

        public void UpdateRoster(SocketModal modal)
        {
            //
        }

        public (bool, int, List<Player>, List<Player>) SearchForPlayer(SocketModal modal)
        {
            List<Player> _activePlayers = new List<Player>();
            List<Player> _inactivePlayers = new List<Player>();

            var range = $"{_playerSheetName}!A2:G";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

            var response = request.ExecuteAsync();
            var valueRangeResult = response.Result;

            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            var player = components
            .First(x => x.CustomId == "newroster_addplayer").Value;

            List<string> splitStr = player.Split(" ").ToList();
            List<Player> playerList = new List<Player>();

            if (valueRangeResult != null && valueRangeResult.Values.Count > 0)
            {
                foreach (var row in valueRangeResult.Values)
                {
                    playerList.Add(new Player(
                        row[0].ToString(),
                        row[1].ToString(),
                        row[2].ToString(),
                        row[3].ToString(),
                        row[4].ToString(),
                        row[5].ToString(),
                        row[6].ToString()));
                }
            }

            foreach(var name in splitStr)
            {
                foreach(var item in playerList)
                {
                    if(item.ActivsionId.ToLower().Contains(name.ToLower()) && item.Active.Equals("Y") ||
                        item.Id.Equals(name.ToLower()) && item.Active.Equals("Y"))
                    {
                        _activePlayers.Add(item);
                    }
                    else if (item.Active.Equals("N"))
                    {
                        _inactivePlayers.Add(item);
                    }    
                }
            }

            //remove duplicates
            var activePlayersNoDupes = _activePlayers.Distinct().ToList();
            var inactivePlayersNoDupes = _inactivePlayers.Distinct().ToList();

            return (_isValid, activePlayersNoDupes.Count, inactivePlayersNoDupes, activePlayersNoDupes);
        }

        public (bool, string) SearchForTeam(SocketModal modal)
        {
            var range = $"{_teamSheetName}!A2:G";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

            var response = request.ExecuteAsync();
            var valueRangeResult = response.Result;

            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            var userInput = components
            .First(x => x.CustomId == "newroster_search_team_name").Value;

            if (valueRangeResult != null && valueRangeResult.Values.Count > 0)
            {
                foreach (var row in valueRangeResult.Values)
                {
                    if (row.Count <= 1)
                    {
                        _id = row[0].ToString() != "" ? _id = row[0].ToString() : _id = "";
                        _name = string.Empty;
                    }
                    else
                    {
                        _id = row[0].ToString() != "" ? _id = row[0].ToString() : _id = "";
                        _name = row[1].ToString() != "" ? _name = row[1].ToString() : _name = "";
                        _isActive = row[6].ToString() != "" ? _isActive = row[6].ToString() : _isActive = "";
                    }

                    if (_id.ToLower().Equals(userInput.ToLower()) && _isActive.Equals("Y") ||
                        _name.ToLower().Contains(userInput.ToLower()) && _isActive.Equals("Y"))
                    {
                        _matchFound = true;
                        break;
                    }
                    else
                    {
                        _matchFound = false;
                    }
                }
            }

            return (_matchFound, userInput);
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
    }
}
