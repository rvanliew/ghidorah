using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class UpdateTeamUserInput
    {
        public string TeamName { get; set; }
        public string Twitter { get; set; }
        public string Group { get; set; }
        public string Active { get; set; }

        public UpdateTeamUserInput(string teamName, string twitter, string group, string active)
        {
            TeamName = teamName;
            Twitter = twitter;
            Group = group;
            Active = active;
        }
    }
}
