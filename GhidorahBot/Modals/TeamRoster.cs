using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Modals
{
    public class TeamRoster
    {
        public string TeamId { get; set; }
        public string PlayerId { get; set; }

        public TeamRoster(string teamId, string playerId)
        {
            TeamId = teamId;
            PlayerId = playerId;
        }
    }
}
