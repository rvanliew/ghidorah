using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class PlayerModel
    {
        public string Id { get; set; }
        public string ActivsionId { get; set; }
        public string DiscordName { get; set; }
        public string Twitter { get; set; }
        public string DateCreated { get; set; }
        public string LastUpdated { get; set; }
        public string Active { get; set; }

        public PlayerModel(string id, string activsionId, string discordName, string twitter, string dateCreated, string lastUpdated, string active)
        {
            Id = id;
            ActivsionId = activsionId;
            DiscordName = discordName;
            Twitter = twitter;
            DateCreated = dateCreated;
            LastUpdated = lastUpdated;
            Active = active;
        }
    }
}
