using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using GhidorahBot.Models;
using GhidorahBot.Extensions;

namespace GhidorahBot.Validation
{
    public class UserValidation
    {
        public string ValidationMsg;
        public bool bValidationError;

        public List<PlayerModel> ActivePlayerList = new List<PlayerModel>();
        public List<PlayerModel> InactivePlayerList = new List<PlayerModel>();

        private bool _teamExist;
        private bool _playerExist;
        private bool _isActive;
        private bool _isValid;
        private List<string> _playerInputList = new List<string>();

        public UserValidation()
        {
            //
        }

        public List<string> ValidateRosterUserInput(SocketModal modal, string customId)
        {
            _playerInputList.Clear();
            bValidationError = false;

            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            string userInput = components
                .First(x => x.CustomId == $"{customId}").Value;

            _playerInputList = userInput.Split(" ").ToList();

            if (_playerInputList.Count < 4 || _playerInputList.Count > 6)
            {
                ValidationMsg = $"A team roster must have a minimum of 4 players and a maximum of 6 players.";
                bValidationError = true;
            }
            else
            {
                bValidationError = false;
            }

            return _playerInputList;
        }

        public bool TeamExists(SocketModal modal, string customId, List<TeamModel> teamList)
        {
            _teamExist = false;
            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            string userInput = components
                .First(x => x.CustomId == $"{customId}").Value;

            foreach (var team in teamList)
            {
                if (userInput == team.Id ||
                    userInput.ToUpper() == team.TeamName.ToUpper())
                {
                    ValidationMsg = $"{userInput.ToUpper()}";
                    _teamExist = true;
                }
            }

            return _teamExist;
        }

        public bool PlayerExists(SocketModal modal, string customId, List<PlayerModel> playerList)
        {
            _playerExist = false;
            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            string userInput = components
                .First(x => x.CustomId == $"{customId}").Value;

            foreach (var player in playerList)
            {
                if (userInput == player.ActivsionId)
                {
                    ValidationMsg = $"{userInput.ToUpper()} already exists";
                    _playerExist = true;
                }
            }

            return _playerExist;
        }

        public void IsPlayerActive(List<string> userInputPlayerList, List<PlayerModel> fullPlayerList)
        {
            bValidationError = false;
            ActivePlayerList.Clear();
            InactivePlayerList.Clear();
            ValidationMsg = String.Empty;

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
                bValidationError = true;
                foreach(var inactivePlayer in InactivePlayerList)
                {
                    ValidationMsg += $"Activision Id: {inactivePlayer.ActivsionId}\r";
                }
            }
        }

        public bool IsTeamActive(SocketModal modal, string customId, List<TeamModel> fullTeamList)
        {
            _isActive = false;
            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            string userInput = components
                .First(x => x.CustomId == $"{customId}").Value;

            foreach(var team in fullTeamList)
            {
                if(userInput.ToUpper() == team.TeamName.ToUpper() ||
                    userInput == team.Id)
                {
                    if(team.Active.Equals("Y"))
                    {
                        ValidationMsg = $"{userInput.ToUpper()}";
                        _isActive = true;
                        return _isActive;
                    }
                    else
                    {
                        _isActive = false;
                    }
                }
            }

            return _isActive;
        }

        public bool IsPlayerOnRoster(List<RosterModel> fullRosterList, List<string> userInputPlayerList)
        {
            foreach(var player in fullRosterList)
            {
                foreach(var userPlayer in userInputPlayerList)
                {
                    if (userPlayer.ToUpper().EqualsAnyOf(
                        player.PlayerOne.ToUpper(),
                        player.PlayerTwo.ToUpper(),
                        player.PlayerThree.ToUpper(),
                        player.PlayerFour.ToUpper(),
                        player.PlayerFive.ToUpper(),
                        player.PlayerSix.ToUpper()))
                    {
                        ValidationMsg = $"{userPlayer}";
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
