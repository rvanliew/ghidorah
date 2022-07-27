using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class RosterModel
    {
        public string Id { get; set; }
        public string TeamName { get; set; }
        public string PlayerOne { get; set; }
        public string PlayerTwo { get; set; }
        public string PlayerThree { get; set; }
        public string PlayerFour { get; set; }
        public string PlayerFive { get; set; }
        public string PlayerSix { get; set; }

        public RosterModel(string id, string teamName, string playerOne, string playerTwo, string playerThree, string playerFour, string playerFive, string playerSix)
        {
            Id = id;
            TeamName = teamName;
            PlayerOne = playerOne;
            PlayerTwo = playerTwo;
            PlayerThree = playerThree;
            PlayerFour = playerFour;
            PlayerFive = playerFive;
            PlayerSix = playerSix;
        }

        //internal void ValidateUserInput(string userInput, string id, string teamName)
        //{
        //    if (userInput.ToUpper() == teamName.ToUpper() || userInput == id)
        //    {
        //        //
        //    }
        //}
    }
}
