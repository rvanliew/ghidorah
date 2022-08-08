using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        [Command("test", RunMode = RunMode.Async)]
        public async Task test()
        {
            await Context.Message.ReplyAsync($"test new command");
        }

        [Command("ghidorah", RunMode = RunMode.Async)]
        public async Task Help()
        {
            var embed = new EmbedBuilder()
            {
                Title = "Ghidorah Bot Help",
            };

            embed.AddField("About Ghidorah",
                $"Version: v1.0.0.0\r" +
                "About Ghirdorah: \r" +
                "Github: [ghidorahbot](https://github.com/rvanliew/ghidorah)\r", false);
            embed.AddField("Slash Commands",
                "/newteam\r" +
                "/updateteam\r" +
                "/newplayer\r" +
                "/updateplayer\r" +
                "/updateroster\r" +
                "/newmatchresult\r" +
                "/updatematchresult\r" +
                "/newplayerstats\r" +
                "/searchplayer\r" +
                "/searchteam\r" +
                "/feedback\r" +
                "/freeagent", false);
            embed.AddField("General Commands",
                "!ghidorah\r" +
                "!help8s\r" +
                "!joinqueue\r" +
                "!leavequeue\r" +
                "!queue", false);

            await Context.Message.ReplyAsync($"{Context.User.Mention}", false, embed.Build());
        }

        [Command("helpmatchresult", RunMode = RunMode.Async)]
        public async Task HelpMatchResult()
        {
            var embed = new EmbedBuilder()
            {
                Title = "Match Result Help"
            };

            embed.WithColor(Color.Blue);
            embed.AddField("/newmatchresult", "Create a new match result\r" +
                "Matches are currently structured to be winner out of 5 maps (best of 5).\r" +
                "Example Match Input: TeamOne 3(wins), TeamTwo 2(wins)", false);
        }

        [Command("helproster", RunMode = RunMode.Async)]
        public async Task HelpRosterManagement()
        {
            var embed = new EmbedBuilder()
            {
                Title = "Roster Management Help"
            };

            embed.WithColor(Color.Blue);
            embed.AddField("Rosters", "Rosters are created automatically\r" +
                "Team Name and Activision Id(s) will be validated against existing Team(s) or Player(s)\r" +
                "If a Team or Player does not exist please contact League Staff for help" +
                "Note: A Roster can have a maximum of 6 players.", false);
            embed.AddField("/updateroster", "Updates an existing Roster\r\r" +
                "Required Items:\r" +
                "Team Name\r\r" +
                "Who can use this slash command?\r" +
                "League Staff\r" +
                "Team Captain\r" +
                "Team Manager", false);
            embed.AddField("Remove Player(s)", "To remove a player input their Activision Id.\r" +
                "To remove multiple players please seperate each Activision Id with a comma.\r" +
                "Example: player1#1234,player2#1234,player3#1234\r" +
                "A total of 6 players can be removed at one time.", false);
            embed.AddField("Add Player(s)", "To add a player input their Activision Id.\r" +
                "To add multiple players please seperate each Activision Id with a comma.\r" +
                "Example: player1#1234,player2#1234,player3#1234\r" +
                "A total of 6 players can be added to a single Roster.", false);

            await Context.Message.ReplyAsync($"{Context.User.Mention}", false, embed.Build());
        }

        [Command("helpplayer", RunMode = RunMode.Async)]
        public async Task HelpPlayerManagement()
        {
            var embed = new EmbedBuilder()
            {
                Title = "Player Management Help"
            };

            embed.WithColor(Color.Blue);
            embed.AddField("/newplayer", "Creates a new Player\r\r" +
                "Required Items:\r" +
                "Activision Id\r\r" +
                "Who can use this slash command?\r" +
                "League Staff", false);
            embed.AddField("/updateplayer", "Updates a Players information.\r\r" +
                "Required Items: Activision Id\r" +
                "Items left blank will not be updated.\r\r" +
                "Who can use this slash command?\r" +
                "League Staff", false);

            await Context.Message.ReplyAsync($"{Context.User.Mention}", false, embed.Build());
        }

        [Command("helpteam", RunMode = RunMode.Async)]
        public async Task HelpTeamManagement()
        {
            var embed = new EmbedBuilder()
            {
                Title = "Team Management Help"
            };

            embed.WithColor(Color.Blue);
            embed.AddField("/newteam", "Creates a new Team\r\r" +
                "Required Items:\r" +
                "Team Name\r" +
                "Group (A-Z)\r" +
                "Team Captain (discord name)\r" +
                "Team Manager (discord name)\r" +
                "Example Discord Name: Discord#1234\r\r" +
                "Note: If there is no manager use the same discord name as captain\r\r" +
                "Who can use this slash command?\r" +
                "League Staff", false);
            embed.AddField("/updateteam", "Updates a Teams information.\r\r" +
                "Required Items: Team Name\r" +
                "Items left blank will not be updated.\r\r" +
                "Who can use this slash command?\r" +
                "League Staff", false);

            await Context.Message.ReplyAsync($"{Context.User.Mention}", false, embed.Build());
        }

        [Command("helpsearch", RunMode = RunMode.Async)]
        public async Task SearchHelp()
        {
            var embed = new EmbedBuilder()
            {
                Title = "Search Team or Player",
                Description = "How to search for Team or Player"
            };

            embed.WithColor(Color.Blue);
            embed.AddField("/searchplayer", "Search for a player by Activision Id\rReturns player information and total stats", false);
            embed.AddField("/searchteam", "Search for a team by team name\rReturns team information and roster information", false);

            await Context.Message.ReplyAsync($"{Context.User.Mention}", false, embed.Build());
        }

        //8s queue commands
        [Command("help8sq", RunMode = RunMode.Async)]
        public async Task QueueHelp()
        {
            var embed = new EmbedBuilder()
            {
                Title = "8s Queue Help",
                Description = "Here you will find information about how to use our random 8s queue feature."
            };

            embed.WithColor(Color.Blue);
            embed.AddField("!joinque {activisionId}", "Use this command to add yourself to the current queue.\rNOTE: Do not forget to add your Activision Id.", false);
            embed.AddField("!leaveque", "Use this command to remove yourself from the current queue.", false);
            embed.AddField("!queue", "Use this command to check the status of the current queue.", false);

            await Context.Message.ReplyAsync($"{Context.User.Mention}", false, embed.Build());
        }

        [Command("joinq", RunMode = RunMode.Async)]
        public async Task JoinQue(string args)
        {
            _playerQueueService.JoinQueue(Context.User, args);
            await Context.Message.ReplyAsync($"{_playerQueueService.Notification}");
        }

        [Command("leaveq", RunMode = RunMode.Async)]
        public async Task LeaveQue()
        {
            _playerQueueService.LeaveQueue(Context.User);
            await Context.Message.ReplyAsync($"{_playerQueueService.Notification}");
        }

        [Command("qstatus", RunMode = RunMode.Async)]
        public async Task GetQueInformation()
        {
            _playerQueueService.GetQueueStatus();
            await Context.Message.ReplyAsync($"{_playerQueueService.Notification}");
        }

        //Scrimmage Commands
        [Command("accept", RunMode = RunMode.Async)]
        public async Task AcceptScrim(string args)
        {
            _playerQueueService.AcceptScrim(Context.User, args);
            await Context.Message.ReplyAsync($"{_playerQueueService.Notification}");
        }

        [Command("scrims", RunMode = RunMode.Async)]
        public async Task ScrimQueueStatus()
        {
            _playerQueueService.GetScrimQueueStatus();
            await Context.Message.ReplyAsync($"{_playerQueueService.Notification}");
        }

        //Secret Command for League Staff Only
        [Command("clearscrimq", RunMode = RunMode.Async)]
        public async Task ClearScrimQueue()
        {
            _playerQueueService.ClearScrimQueue();
            await Context.Message.ReplyAsync($"{_playerQueueService.Notification}");
        }
    }
}
