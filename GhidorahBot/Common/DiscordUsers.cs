using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Common
{
    public class DiscordUsers
    {
        private DiscordSocketClient _client { get; set; }

        public DiscordUsers()
        {
            //
        }

        public List<SocketGuildUser> DownloadAllUsers(SocketInteractionContext ctx)
        {
            List<SocketGuildUser> userList = new List<SocketGuildUser>();

            foreach(SocketGuildUser serverUser in ctx.Guild.Users)
            {
                userList.Add(serverUser);
            }

            return userList;
        }
    }
}
