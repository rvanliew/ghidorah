using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class UpdateTeamModel
    {
        public string TeamName { get; set; }
        public string Twitter { get; set; }
        public string Group { get; set; }
        public string Captain { get; set; }
        public string Manager { get; set; }

        public UpdateTeamModel(string teamName, string twitter, string group, string captain, string manager)
        {
            TeamName = teamName;
            Twitter = twitter;
            Group = group;
            Captain = captain;
            Manager = manager;
        }
    }
}
