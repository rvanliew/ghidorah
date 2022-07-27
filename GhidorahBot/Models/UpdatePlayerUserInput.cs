using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class UpdatePlayerUserInput
    {
        public string ActivisionId { get; set; }
        public string DiscordName { get; set; }
        public string Twitter { get; set; }
        public string Active { get; set; }

        public UpdatePlayerUserInput(string activisionId, string discordName, string twitter, string active)
        {
            ActivisionId = activisionId;
            DiscordName = discordName;
            Twitter = twitter;
            Active = active;
        }
    }
}
