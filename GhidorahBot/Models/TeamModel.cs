using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class TeamModel
    {
        public string Id { get; set; }
        public string TeamName { get; set; }
        public string Twitter { get; set; }
        public string Group { get; set; }
        public string DateCreated { get; set; }
        public string LastUpdated { get; set; }
        public string Active { get; set; }

        public TeamModel(string id, string teamName, string twitter, string group, string dateCreated, string lastUpdated, string active)
        {
            Id = id;
            TeamName = teamName;
            Twitter = twitter;
            Group = group;
            DateCreated = dateCreated;
            LastUpdated = lastUpdated;
            Active = active;
        }
    }
}
