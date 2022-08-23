using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class PlayerQueueGuildModel
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get;set; }

        public PlayerQueueGuildModel(ulong guildId, ulong channelId)
        {
            GuildId = guildId;
            ChannelId = channelId;
        }
    }
}
