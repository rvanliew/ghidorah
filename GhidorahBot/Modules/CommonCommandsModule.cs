using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GhidorahBot.Database;
using GhidorahBot.Models;
using GhidorahBot.Services;
using RunMode = Discord.Commands.RunMode;

namespace GhidorahBot.Modules
{
    public class CommonCommandsModule : ModuleBase<SocketCommandContext>
    {
        private Search _search { get; set; }
        private PlayerQueueService _playerQueueService { get; set; }

        public CommonCommandsModule(Search search, PlayerQueueService playerQue)
        {
            _search = search;
            _playerQueueService = playerQue;
        }

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

        [Command("playerstats", RunMode = RunMode.Async)]
        public async Task PlayerStats(string args)
        {
            string activisionId = args;
            PlayerModel player = _search.SearchPlayer(activisionId);
            PlayerStatTotalsModel playerStats = _search.SearchPlayerStatTotals(activisionId);

            if (player == null)
            {
                await Context.Message.ReplyAsync($"{Context.User.Mention}\r" +
                    $"Error: Could not find player by name: {activisionId}");
            }
            else if (playerStats == null)
            {
                await Context.Message.ReplyAsync($"{Context.User.Mention}\r" +
                    $"Error retrieving stats for player {activisionId}");
            }
            else
            {
                await Context.Message.ReplyAsync($"{Context.User.Mention}\r\r" +
                    $"Player Information:\r" +
                    $"Id: {player.Id}\r" +
                    $"Activision Id: {player.ActivsionId}\r" +
                    $"Discord Name: {player.DiscordName}\r" +
                    $"Twitter: {player.Twitter}\r" +
                    $"Date Created: {player.DateCreated}\r" +
                    $"Last Updated: {player.LastUpdated}\r" +
                    $"Active: {player.Active}\r\r" +
                    $"Total Player Stats:\r" +
                    $"Kills: {playerStats.Kills}\r" +
                    $"Deaths: {playerStats.Deaths}\r" +
                    $"Hardpoint Hill Time: {playerStats.HillTime}\r" +
                    $"Bombs Planted: {playerStats.BombsPlanted}\r" +
                    $"Objective Kills (Control): {playerStats.ObjKills}\r" +
                    $"K/D Ratio: {playerStats.KdRatio}");
            }
        }

        //8s Que System
        [Command("joinque", RunMode = RunMode.Async)]
        public async Task JoinQue(string args)
        {
            _playerQueueService.JoinQueue(Context.User, args);
            await Context.Message.ReplyAsync($"{_playerQueueService.Notification}");
        }

        [Command("leaveque", RunMode = RunMode.Async)]
        public async Task LeaveQue()
        {
            _playerQueueService.LeaveQueue(Context.User);
            await Context.Message.ReplyAsync($"{_playerQueueService.Notification}");
        }

        [Command("que", RunMode = RunMode.Async)]
        public async Task GetQueInformation()
        {
            _playerQueueService.GetQueueStatus();
            await Context.Message.ReplyAsync($"{_playerQueueService.Notification}");
        }
    }
}
