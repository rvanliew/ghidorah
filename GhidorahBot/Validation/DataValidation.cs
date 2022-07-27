using Discord.WebSocket;
using GhidorahBot.Database;
using GhidorahBot.Extensions;
using GhidorahBot.Models;
using System.Text.RegularExpressions;

namespace GhidorahBot.Validation
{
    public class DataValidation
    {
        private Search _search { get; set; }

        public string ValidationErrorMsg = "";
        public bool IsRequestValid;
        public string ValidationMsg = "";
        public string TeamId;
        public string PlayerId;
        public List<PlayerModel> ActivePlayerList = new List<PlayerModel>();
        public List<PlayerModel> InactivePlayerList = new List<PlayerModel>();
        public List<PlayerModel> RemovePlayerList = new List<PlayerModel>();
        public List<PlayerModel> AddPlayerList = new List<PlayerModel>();
        public RosterModel RosterMatch;

        //PlayerStats
        public PlayerModel Player;
        public StatsModel Stats;
        public MapModeModel MapMode;
        public string GUID;
        //

        //MatchResult
        public MatchResultModel MatchResult;
        public TeamModel TeamOne;
        public TeamModel TeamTwo;
        public int TeamOneMapsWon;
        public int TeamTwoMapsWon;
        public string MatchGUID;
        public string WinningTeam = string.Empty;
        public string LosingTeam = string.Empty;
        public string MatchGroup = string.Empty;
        //

        private static int _maxSlots = 6;
        private int _unavailableSlots = 0;
        private string _userInputTeam;
        private string _userInputPlayer;
        private bool _isActive;
        private bool _isPlayerValid;
        private bool _isPlayerRosterValid;
        private bool _exists;
        private bool _rosterExists;

        //Roster Variables
        private List<string> _userInputList = new List<string>();
        private List<string> _removePlayerList = new List<string>();
        private List<string> _addPlayerList = new List<string>();
        private List<string> _invalidPlayerList = new List<string>();
        //

        public DataValidation(Search search)
        {
            _search = search;
        }

        private void PlayerExists(SocketModal modal, string customId, List<PlayerModel> playerList)
        {
            _exists = false;
            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            _userInputPlayer = components
                .First(x => x.CustomId == $"{customId}").Value;

            foreach (var player in playerList)
            {
                if (_userInputPlayer == player.ActivsionId)
                {
                    //Used for Updating Player
                    PlayerId = player.Id;
                    _exists = true;
                    return;
                }
            }
        }

        private void IsPlayerActive(List<string> userInputPlayerList, List<PlayerModel> fullPlayerList)
        {
            ActivePlayerList.Clear();
            InactivePlayerList.Clear();
            ValidationMsg = String.Empty;
            string inactivePlayersMsg = "";

            foreach (var userPlayer in userInputPlayerList)
            {
                foreach(var player in fullPlayerList)
                {
                    if(userPlayer == player.ActivsionId)
                    {
                        if(player.Active.Equals("Y"))
                        {
                            ActivePlayerList.Add(player);
                        }
                        else
                        {
                            InactivePlayerList.Add(player);
                        }
                    }
                }
            }

            if(InactivePlayerList.Any())
            {
                _isPlayerValid = false;
                foreach(var inactivePlayer in InactivePlayerList)
                {
                    inactivePlayersMsg += $"Activision Id: {inactivePlayer.ActivsionId}\r";
                }

                ValidationErrorMsg += $"The players listed below are currently Inactive. " +
                                        $"You must activate these players before you can add them to a roster.\r\r" +
                                        $"Inactive:\r" +
                                        $"{inactivePlayersMsg}\r\r";
            }
        }

        private void IsPlayerOnRoster(List<RosterModel> fullRosterList, List<PlayerModel> addPlayerList)
        {
            _isPlayerRosterValid = true;
            foreach (var player in fullRosterList)
            {
                foreach (var addPlayer in addPlayerList)
                {
                    if (addPlayer.Id.EqualsAnyOf(
                        player.PlayerOne.ToUpper(),
                        player.PlayerTwo.ToUpper(),
                        player.PlayerThree.ToUpper(),
                        player.PlayerFour.ToUpper(),
                        player.PlayerFive.ToUpper(),
                        player.PlayerSix.ToUpper()))
                    {
                        ValidationErrorMsg += $"Player: {addPlayer.ActivsionId} is already on {player.TeamName}.\r";
                        _isPlayerRosterValid = false;
                    }
                }
            }
        }

        private void TeamExists(SocketModal modal, string customId, List<TeamModel> teamList)
        {
            _exists = false;
            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            _userInputTeam = components
                .First(x => x.CustomId == $"{customId}").Value;

            foreach (var team in teamList)
            {
                if (_userInputTeam == team.Id ||
                    _userInputTeam.ToUpper() == team.TeamName.ToUpper())
                {
                    ValidationMsg = $"{_userInputTeam.ToUpper()}";
                    //Used for Updating Team
                    TeamId = team.Id;
                    _exists = true;
                    return;
                }
            }
        }

        private void IsTeamActive(SocketModal modal, string customId, List<TeamModel> fullTeamList)
        {
            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            _userInputTeam = components
                .First(x => x.CustomId == $"{customId}").Value;

            foreach(var team in fullTeamList)
            {
                if(_userInputTeam.ToUpper() == team.TeamName.ToUpper() ||
                    _userInputTeam == team.Id)
                {
                    if(team.Active.Equals("Y"))
                    {
                        _isActive = true;
                        return;
                    }
                    else
                    {
                        _isActive = false;
                    }
                }
            }

            if (!_isActive)
            {
                ValidationErrorMsg += $"Team name entered has an active status of 'N' (No).\r" +
                    $"Please update this teams Active status before adding players to the roster\r\r";
            }
        }

        private void RosterExist(SocketModal modal, string customId, List<RosterModel> fullRosterList)
        {
            _unavailableSlots = 0;
            _rosterExists = false;
            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            _userInputTeam = components
                .First(x => x.CustomId == $"{customId}").Value;

            //fullRosterList.ToList().ForEach(x => x.ValidateUserInput(_userInputTeam, x.Id, x.TeamName));

            foreach (var roster in fullRosterList)
            {
                if (_userInputTeam.ToUpper() == roster.TeamName.ToUpper() ||
                    _userInputTeam == roster.Id)
                {
                    ValidationMsg = $"{_userInputTeam.ToUpper()}";
                    TeamId = roster.Id;
                    _rosterExists = true;

                    RosterMatch = new RosterModel(
                        roster.Id,
                        roster.TeamName,
                        roster.PlayerOne,
                        roster.PlayerTwo,
                        roster.PlayerThree,
                        roster.PlayerFour,
                        roster.PlayerFive,
                        roster.PlayerSix);

                    if (!string.IsNullOrWhiteSpace(roster.PlayerOne))
                    {
                        _unavailableSlots++;
                    }

                    if (!string.IsNullOrWhiteSpace(roster.PlayerTwo))
                    {
                        _unavailableSlots++;
                    }
                    
                    if (!string.IsNullOrWhiteSpace(roster.PlayerThree))
                    {
                        _unavailableSlots++;
                    }
                    
                    if (!string.IsNullOrWhiteSpace(roster.PlayerFour))
                    {
                        _unavailableSlots++;
                    }
                    
                    if (!string.IsNullOrWhiteSpace(roster.PlayerFive))
                    {
                        _unavailableSlots++;
                    }
                    
                    if (!string.IsNullOrWhiteSpace(roster.PlayerSix))
                    {
                        _unavailableSlots++;
                    }

                    return;
                }
            }
        }

        private List<PlayerModel> TransmutePlayerList(List<string> userInputList, List<PlayerModel> fullPlayerList)
        {
            List<PlayerModel> transmutedPlayerList = new List<PlayerModel>();

            foreach (var user in userInputList)
            {
                foreach(var player in fullPlayerList)
                {
                    if(user == player.ActivsionId.ToUpper())
                    {
                        transmutedPlayerList.Add(new PlayerModel(
                            player.Id,
                            player.ActivsionId,
                            player.DiscordName,
                            player.Twitter,
                            player.DateCreated,
                            player.LastUpdated,
                            player.Active));
                    }
                }
            }

            return transmutedPlayerList;
        }

        private void ValidateRosterUserInput(SocketModal modal, List<PlayerModel> fullPlayerList)
        {
            List<string> activisionIdList = fullPlayerList.Select(x => x.ActivsionId).ToList();
            activisionIdList = activisionIdList.ConvertAll(x => x.ToUpper());
            _userInputList.Clear();
            _invalidPlayerList.Clear();

            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            string removePlayer = components
                .First(x => x.CustomId == $"updateroster_removeplayer").Value;

            if(!string.IsNullOrWhiteSpace(removePlayer))
            {
                _removePlayerList = removePlayer.Split(" ").ToList();
                _removePlayerList = _removePlayerList.ConvertAll(x => x.ToUpper());

                RemovePlayerList = TransmutePlayerList(_removePlayerList, fullPlayerList);
            }         

            string addPlayer = components
                .First(x => x.CustomId == $"updateroster_addplayer").Value;

            if(!string.IsNullOrWhiteSpace(addPlayer))
            {
                _addPlayerList = addPlayer.Split(" ").ToList();

                if(_addPlayerList.Count > 6)
                {
                    _isPlayerValid = false;
                    ValidationErrorMsg += "A maximum of 6 players are allotted per roster.";
                }

                _addPlayerList = _addPlayerList.ConvertAll(x => x.ToUpper());

                AddPlayerList = TransmutePlayerList(_addPlayerList, fullPlayerList);
            }         

            _userInputList.AddRange(_removePlayerList);
            _userInputList.AddRange(_addPlayerList);

            _invalidPlayerList = _userInputList.Except(activisionIdList).ToList();

            if(_invalidPlayerList.Any())
            {
                _isPlayerValid = false;
                ValidationErrorMsg += "Invalid Player(s):\r";
                foreach(var player in _invalidPlayerList)
                {
                    ValidationErrorMsg += $"Player Name: {player}\r";
                }

                return;
            }
            else
            {
                _isPlayerValid = true;
            }
        }

        public void ValidateUpdateRoster(SocketModal modal)
        {
            ValidationErrorMsg = "";
            var fullRosterList = _search.GetRosterIdsList();

            if (fullRosterList.Any())
            {
                RosterExist(modal, "updateroster_search_team_name", fullRosterList);

                if(_rosterExists)
                {
                    var fullPlayerList = _search.GetPlayerList();

                    if(fullPlayerList.Any())
                    {
                        ValidateRosterUserInput(modal, fullPlayerList);

                        if (_isPlayerValid)
                        {
                            IsPlayerOnRoster(fullRosterList, AddPlayerList);

                            if (_isPlayerRosterValid)
                            {
                                int openSlots = _maxSlots - _unavailableSlots;
                                int slotsRequired = openSlots - (RemovePlayerList.Count + AddPlayerList.Count);

                                if (slotsRequired < 0)
                                {
                                    ValidationErrorMsg = $"Open Roster spots {openSlots}.\r" +
                                        $"Roster spots pending {Math.Abs(slotsRequired)}.\r" +
                                        $"A Roster can have a maximum of 6 total players.";
                                    IsRequestValid = false;
                                }
                                else
                                {
                                    IsRequestValid = true;
                                }
                            }
                            else
                            {
                                IsRequestValid = false;
                            }
                        }
                        else
                        {
                            IsRequestValid = false;
                        }
                    }
                    else
                    {
                        ValidationErrorMsg = _search.SearchExceptionMsg;
                        IsRequestValid = false;
                    }                  
                }
                else
                {
                    ValidationErrorMsg = $"Roster does not exist.";
                    IsRequestValid = false;
                }
            }
            else
            {
                ValidationErrorMsg = _search.SearchExceptionMsg;
                IsRequestValid = false;
            }
        }

        public void ValidateNewTeam(SocketModal modal)
        {
            var fullTeamList = _search.GetTeamList();

            if (fullTeamList.Any())
            {
                TeamExists(modal, "team_Name", fullTeamList);

                if (_exists)
                {
                    ValidationErrorMsg = $"Team: {ValidationErrorMsg} already exists";
                    IsRequestValid = false;
                    return;
                }
                else
                {
                    IsRequestValid = true;
                }
            }
            else
            {
                IsRequestValid = false;
            }
        }

        public void ValidateUpdateTeam(SocketModal modal)
        {
            var fullTeamList = _search.GetTeamList();

            if (fullTeamList.Any())
            {
                TeamExists(modal, "search_team_Name", fullTeamList);

                if(_exists)
                {
                    IsRequestValid = true;
                }
                else
                {
                    IsRequestValid = false;
                    ValidationErrorMsg += "Could not find team.";
                }
            }
            else
            {
                IsRequestValid = false;
            }
        }

        public void ValidateNewPlayer(SocketModal modal)
        {
            var fullPlayerList = _search.GetPlayerList();

            if(fullPlayerList.Any())
            {
                PlayerExists(modal, "player_activ_id", fullPlayerList);

                if(_exists)
                {
                    ValidationErrorMsg = $"Player already exists";
                    IsRequestValid = false;
                    return;
                }
                else
                {
                    IsRequestValid = true;
                }
            }
            else
            {
                IsRequestValid = false;
            }
        }

        public void ValidateUpdatePlayer(SocketModal modal)
        {
            var fullPlayerList = _search.GetPlayerList();

            if (fullPlayerList.Any())
            {
                PlayerExists(modal, "search_player_id", fullPlayerList);

                if(_exists)
                {
                    IsRequestValid = true;
                }
                else
                {
                    IsRequestValid = false;
                    ValidationErrorMsg += "Could not find player.";
                }
            }
            else
            {
                IsRequestValid = false;
            }
        }

        public void ValidateNewPlayerStats(SocketModal modal)
        {
            MapModeModel mapModeItem;         

            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();
            string guid = components
                .First(x => x.CustomId == $"newstats_search_matchguid").Value;

            string playerName = components
                .First(x => x.CustomId == "newstats_playername").Value;

            string mapMode = components
            .First(x => x.CustomId == "newstats_mapmode").Value;

            mapMode.TrimStart();
            mapMode.TrimEnd();
            mapMode = Regex.Replace(mapMode, @"\s+", " ");
            string[] splitMapMode = mapMode.Split(" ");

            if (string.IsNullOrWhiteSpace(splitMapMode[0]))
            {
                IsRequestValid = false;
                ValidationErrorMsg = "Cannot complete request. Map is missing.";
                return;
            }
            else if (string.IsNullOrWhiteSpace(splitMapMode[1]))
            {
                IsRequestValid = false;
                ValidationErrorMsg = "Cannot complete request. Mode is missing.";
                return;
            }
            else
            {
                mapModeItem = new MapModeModel(splitMapMode[0], splitMapMode[1]);
            }

            string stats = components
            .First(x => x.CustomId == "newstats_stats").Value;

            stats.TrimStart();
            stats.TrimEnd();
            stats = Regex.Replace(stats, @"\s+", " ");
            string[] splitStats = stats.Split(" ");

            if (splitStats.Count() != 5)
            {
                IsRequestValid = false;
                ValidationErrorMsg = "Cannot complete request. Missing required stats.";
            }

            string matchGuid = _search.SearchGUID("MatchResult", "B2:B", guid);
            PlayerModel player = _search.SearchPlayer(playerName);

            if (matchGuid != null)
            {
                GUID = matchGuid;

                if (player != null)
                {
                    var statsItem = new StatsModel(splitStats[0], splitStats[1], splitStats[2], splitStats[3], splitStats[4]);

                    Player = player;
                    MapMode = mapModeItem;
                    Stats = statsItem;
                    IsRequestValid = true;
                }
                else
                {
                    IsRequestValid = false;
                    ValidationErrorMsg = $"{modal.User.Mention} Could not find player: {playerName}";
                }
            }
            else
            {
                IsRequestValid = false;
                ValidationErrorMsg = $"{modal.User.Mention} Could not find GUID: {guid}";
            }
        }

        public void ValidateMatchResult(SocketModal modal)
        {
            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            string teamOneName = components
            .First(x => x.CustomId == "team_1_name").Value;

            string teamOneMapsWon = components
            .First(x => x.CustomId == "team_1_maps_won").Value;

            int teamOneMapsWonInt = Convert.ToInt32(teamOneMapsWon);

            string teamTwoName = components
            .First(x => x.CustomId == "team_2_name").Value;

            string teamTwoMapsWon = components
            .First(x => x.CustomId == "team_2_maps_won").Value;

            int teamTwoMapsWonInt = Convert.ToInt32(teamTwoMapsWon);

            string group = components
            .First(x => x.CustomId == "group_id").Value;

            MatchGroup = group;

            TeamOneMapsWon = teamOneMapsWonInt;
            TeamTwoMapsWon = teamTwoMapsWonInt;

            TeamOne = _search.SearchTeam(teamOneName);
            TeamTwo = _search.SearchTeam(teamTwoName);

            if (TeamOne == null)
            {
                IsRequestValid = false;
                ValidationErrorMsg = $"Error: Could not find data for {teamOneName}";
                return;
            }
            else if (TeamTwo == null)
            {
                IsRequestValid = false;
                ValidationErrorMsg = $"Error: Could not find data for {teamTwoName}";
                return;
            }

            if (teamOneMapsWonInt >= 3)
            {
                WinningTeam = teamOneName;
                LosingTeam = teamTwoName;
                IsRequestValid = true;
            }
            else if (teamTwoMapsWonInt >= 3)
            {
                WinningTeam = teamTwoName;
                LosingTeam = teamOneName;
                IsRequestValid = true;
            }
        }

        public void ValidateUpdateMatchResult(SocketModal modal)
        {
            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            string guid = components
            .First(x => x.CustomId == "updatematch_searchmatchGUID").Value;

            string teamOneName = components
            .First(x => x.CustomId == "updatematch_team_1_name").Value;

            string teamOneMapsWon = components
            .First(x => x.CustomId == "updatematch_team_1_maps_won").Value;

            int teamOneMapsWonInt = Convert.ToInt32(teamOneMapsWon);

            string teamTwoName = components
            .First(x => x.CustomId == "updatematch_team_2_name").Value;

            string teamTwoMapsWon = components
            .First(x => x.CustomId == "updatematch_team_2_maps_won").Value;

            int teamTwoMapsWonInt = Convert.ToInt32(teamTwoMapsWon);

            TeamOneMapsWon = teamOneMapsWonInt;
            TeamTwoMapsWon = teamTwoMapsWonInt;

            MatchResult = _search.SearchMatchResult(guid);

            if(MatchResult != null)
            {
                MatchGUID = MatchResult.GUID;
            }
            else
            {
                MatchGUID = string.Empty;
                IsRequestValid = false;
                ValidationErrorMsg = $"Could not find a match for GUID: {guid}";
                return;
            }          

            TeamOne = _search.SearchTeam(teamOneName);
            TeamTwo = _search.SearchTeam(teamTwoName);

            if(TeamOne == null || TeamTwo == null)
            {
                IsRequestValid = false;
                ValidationErrorMsg = $"One more Team names entered do not match any teams in the database.\r" +
                    $"TEAM 1: {teamOneName}\r" +
                    $"TEAM 2: {teamTwoName}";
                return;
            }
            else
            {
                IsRequestValid = true;
            }
        }
    }
}
