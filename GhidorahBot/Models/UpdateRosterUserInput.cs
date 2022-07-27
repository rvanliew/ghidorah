using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class UpdateRosterUserInput
    {
        public string TeamName { get; set; }
        public string POne { get; set; }
        public string PTwo { get; set; }
        public string PThree { get; set; }
        public string PFour { get; set; }
        public string PFive { get;set; }
        public string PSix { get; set; }

        public UpdateRosterUserInput(string teamName, string pOne, string pTwo, string pThree, string pFour, string pFive, string pSix)
        {
            TeamName = teamName;
            POne = pOne;
            PTwo = pTwo;
            PThree = pThree;
            PFour = pFour;
            PFive = pFive;
            PSix = pSix;
        }
    }
}
