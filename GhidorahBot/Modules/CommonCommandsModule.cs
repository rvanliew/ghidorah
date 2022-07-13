using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using GhidorahBot.Services;
using RunMode = Discord.Commands.RunMode;

namespace GhidorahBot.Modules
{
    public class CommonCommandsModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping", RunMode = RunMode.Async)]
        public async Task Hello()
        {
            await Context.Message.ReplyAsync($"Receiving {Context.User.Username} loud and clear! Ghidorah is ready to accept commands.");
        }

        [Command("leaguehelp", RunMode = RunMode.Async)]
        public async Task LeagueHelp()
        {
            var embed = new EmbedBuilder()
            {
                Title = "League Bot Help",
                Description = "Below you will find more information about how to use this bot"
            };
        }
    }
}
