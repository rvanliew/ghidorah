using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class ChannelModel
    {
        public string GuildId { get; set; }
        public string GuildName { get; set; }
        public string ChannelId { get; set; }
        public string ChannelName { get; set; }

        public ChannelModel(string guildId, string guildName, string channelId, string channelName)
        {
            GuildId = guildId;
            GuildName = guildName;
            ChannelId = channelId;
            ChannelName = channelName;
        }
    }
}
