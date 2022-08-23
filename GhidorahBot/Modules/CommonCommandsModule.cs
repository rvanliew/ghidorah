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
using GhidorahBot.Validation;
using RunMode = Discord.Commands.RunMode;

namespace GhidorahBot.Modules
{
    public class CommonCommandsModule : ModuleBase<SocketCommandContext>
    {
        private Search _search { get; set; }
        private PlayerQueueService _playerQueueService { get; set; }
        private DiscordSocketClient _discordClient { get; set; }
        private DataValidation _validation { get; set; }

        public CommonCommandsModule(Search search, PlayerQueueService playerQue, DiscordSocketClient client, DataValidation validation)
        {
            _search = search;
            _playerQueueService = playerQue;
            _discordClient = client;
            _validation = validation;
        }

        [Command("ping", RunMode = RunMode.Async)]
        public async Task Hello()
        {
            await Context.Message.ReplyAsync($"Receiving {Context.User.Username} loud and clear! Ghidorah is ready to accept commands.");
        }

        [Command("ghidorah", RunMode = RunMode.Async)]
        public async Task Help()
        {
            var embed = new EmbedBuilder()
            {
                Title = "Ghidorah Bot Help",
            };

            embed.AddField("About Ghidorah",
                $"Version: v1.0.0.144\r\r" +
                "About:\r" +
                "Ghidorah is a multi-purpose Discord bot. Most of the features center around helping streamline and facilitate a Call of Duty League.\r\r" +
                "Ghidorah hosts many features that will help organizers manage teams, players, roster, player stats, etc.\r" +
                "Make sure and take advantage of Ghidorah's queue system (8s queue and Scrim Finder)\r\r" +
                "Github: [ghidorahbot](https://github.com/rvanliew/ghidorah)\r" +
                "Terms of Service: [Ghidorah ToS](https://github.com/rvanliew/ghidorah/blob/main/README.md)", false);
            embed.AddField("Slash Commands",
                "/newteam\r" +
                "/updateteam\r" +
                "/newplayer\r" +
                "/updateplayer\r" +
                "/newmatchresult\r" +
                "/updatematchresult\r" +
                "/rosteraddplayers\r" +
                "/rosterremoveplayers\r" +
                "/newplayerstats\r" +
                "/updateplayerstats\r" +
                "/searchplayer\r" +
                "/searchteam\r" +
                "/searchmatchresult\r" +
                "/feedback\r" +
                "/freeagent\r" +
                "/requestadmin\r" +
                "/newscrim\r" +
                "/registerchannel\r" +
                "/requestcaster\r", false);
            embed.AddField("General Commands",
                "!ghidorah\r" +
                "!standings\r" +
                "!mapsmodes\r" +
                "!help8sq\r" +
                "!joinq {Activision Id}\r" +
                "!leaveq\r" +
                "!qstatus\r" +
                "!acceptscrim {Scrim Id}\r" +
                "!scrims\r" +
                "!createleague {league password}", false);
            embed.AddField("Help Commands",
                "!helpmatchresult\r" +
                "!helpstats" +
                "!helproster\r" +
                "!helpplayer\r" +
                "!helpteam\r" +
                "!helpsearch\r" +
                "!helpadminrequest", false);

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
            embed.AddField("/newmatchresult", "Creates a new match result\r\r" +
                "Matches are currently structured to be winner out of 5 maps (best of 5).\r\r" +
                "If a match is not recored put a 0 (zero) for both teams map won. Both teams with recieve a match loss (0-3) map count.\r" +
                "Any forfeit matches will be recorded as a 3-0\r\r" +
                "Example Match Input: TeamOne 3(wins), TeamTwo 2(wins)\r", false);

            await Context.Message.ReplyAsync($"{Context.User.Mention}", false, embed.Build());
        }

        [Command("helpstats", RunMode = RunMode.Async)]
        public async Task HelpPlayerStats()
        {
            var embed = new EmbedBuilder()
            {
                Title = "Player Stats Help"
            };

            embed.WithColor(Color.Blue);
            embed.AddField("/newplayerstats", "Creates a new player stat entry\r" +
                "A {Match GUID} is required to add new player stats. This is how the Bot knows which match the stats should be connected to.", false);
            embed.AddField("/updateplayerstats", "Update a existing player stat entry\r" +
                "A {Match GUID} and Player {Activision Id} is required to update player stats. This is how the Bot knows which match the stats should be updated for.", false);
            embed.AddField("Match GUID", "Match GUIDs are automaitcally generated when a new match result is created.\r" +
                "How to update Match GUID in Player Stats Table:\r" +
                "1. Get the existing Match GUID\r" +
                "2. Get the new Match GUID\r\r" +
                "Enter the {Existing GUID} comma(,) {New GUID} (NO SPACES, No brackets, no parathesis)\r" +
                "Example: 863964e3-0b73-4880-a655-10a64dca8a5d,052383e0-bc80-4d18-b6ce-f7244462a4bf\r\r" +
                "The rest of the stat information should remain the same, unless you wish to also update that information.\r" +
                "You will need to re-enter everything regardless of what is being updated (ex: map, mode, kills, deaths, etc.\r\r" +
                "I understand re-typing everything (map, mode, stats, etc.) is redundant, but it helps ensure the Bot knows which record to update.", false);
            embed.AddField("Maps and Modes", "Map and Mode must be comma separated (no spaces)\r" +
                "Example: Highrise,hp\r\r" +
                "Please use abbriviations for gamemodes (hp, snd, ctrl, dom, ctf, etc.).\r" +
                "*Note: Maps and Mode are subject to change per new Call of Duty title.\r\r" +
                "If you are not sure what maps/modes are currently in available please use !mapsmodes");
            embed.AddField("Player Stats", "Stats must be comma separated (no spaces)\r" +
                "User Input Description: Kills,Deaths,HP_Time(h:mm:ss),BombsPlanted,ObjKills\r" +
                "Example: 20,15,0:01:30,4,10", false);

            await Context.Message.ReplyAsync($"{Context.User.Mention}", false, embed.Build());
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
                "To only update the captain simply put in the captains discord name\r" +
                "To only update the managers discord name put a comma followed by the managers discord name\r" +
                "Example: ,DiscordName#1234" +
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
            embed.AddField("/searchteam", "Search for a team by team name\rReturns team and roster information as well as Team total stats", false);
            embed.AddField("/searchmatchresult", "Search for a match result using the match GUID.\rReturns Teams and map count for the specified match.");

            await Context.Message.ReplyAsync($"{Context.User.Mention}", false, embed.Build());
        }

        [Command("helpregisterchannel", RunMode = RunMode.Async)]
        public async Task HelpRegisterChannel()
        {
            var embed = new EmbedBuilder()
            {
                Title = "Register Channel Help"
            };

            embed.WithColor(Color.Blue);
            embed.AddField("/registerchannel", "[Tutorial](https://github.com/rvanliew/ghidorah/blob/main/README.md#register-discord-channel)");

            await Context.Message.ReplyAsync($"{Context.User.Mention}", false, embed.Build());
        }

        [Command("helpadminrequest", RunMode = RunMode.Async)]
        public async Task HelpAdminRequest()
        {
            var embed = new EmbedBuilder()
            {
                Title = "Admin Request Help"
            };

            embed.WithColor(Color.Blue);
            embed.AddField("/requestadmin", "Fill out a short form describing your problem.\r" +
                "Once you click submit a request will populate inside the admin request channel where admins can review it and take action.\r\r" +
                "Note: You must register a caster request discord channel before you can use this request.\r" +
                "If you need help registering a discord channel please use command: !registerchannel", false);

            await Context.Message.ReplyAsync($"{Context.User.Mention}", false, embed.Build());
        }

        [Command("helprequestcaster", RunMode = RunMode.Async)]
        public async Task HelpRequestCaster()
        {
            var embed = new EmbedBuilder()
            {
                Title = "Request a Caster Help"
            };

            embed.WithColor(Color.Blue);
            embed.AddField("/requestcaster", "Creates a new Caster request\r" +
                "Provide basic information about your match\r" +
                "Ex: Team 1 vs Team 2 10pm mst\r\r" +
                "Anyone with the 'Caster' discord role will see your request in their registered caster request channel\r\r" +
                "Note: You must register a caster request discord channel before you can use this request.\r" +
                "If you need help registering a discord channel please use command: !registerchannel", false);

            await Context.Message.ReplyAsync($"{Context.User.Mention}", false, embed.Build());
        }

        [Command("mapsmodes", RunMode = RunMode.Async)]
        public async Task GetMapsAndModes()
        {
            int _index = 1;
            List<MapModel> fullMapList = new List<MapModel>();
            List<GamemodeModel> fullGamemodeList = new List<GamemodeModel>();

            fullMapList = _search.GetMapList();
            fullGamemodeList = _search.GetGamemodeList();

            string mapsMessage = string.Empty;
            foreach(MapModel map in fullMapList)
            {
                mapsMessage += $"{_index}: {map.MapName}\r";
                _index++;
            }

            mapsMessage += "\r";
            mapsMessage += $"Last Updated Date: {fullMapList[0].LastUpdateDt}\r" +
                    $"Last Updated By: {fullMapList[0].LastUpdatedBy}\r";

            _index = 1;
            string modeMessage = string.Empty;
            foreach(GamemodeModel gamemode in fullGamemodeList)
            {
                modeMessage += $"{_index}: {gamemode.ModeName}\r";
                _index++;
            }

            modeMessage += "\r";
            modeMessage += $"Last Updated Date: {fullGamemodeList[0].LastUpdated}\r" +
                    $"Last Updated By: {fullGamemodeList[0].LastUpdatedBy}\r";

            var embed = new EmbedBuilder()
            {
                Title = "Map and Gamemode List"
            };

            embed.AddField("Maps", $"{mapsMessage}", false);
            embed.AddField("Modes", $"{modeMessage}", false);

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
            embed.AddField("!joinq {activisionId}", "Use this command to add yourself to the current queue.\rNOTE: Do not forget to add your Activision Id.", false);
            embed.AddField("!leaveq", "Use this command to remove yourself from the current queue.", false);
            embed.AddField("!qstatus", "Use this command to check the status of the current queue.", false);

            await Context.Message.ReplyAsync($"{Context.User.Mention}", false, embed.Build());
        }

        [Command("joinq", RunMode = RunMode.Async)]
        public async Task JoinQue(string args)
        {
            if(args != null)
            {
                _playerQueueService.JoinQueue(Context.User, args, Context.Guild.Id, Context.Channel.Id);
                if(!string.IsNullOrWhiteSpace(_playerQueueService.DirectMessage))
                {
                    await Context.User.SendMessageAsync($"{Context.User.Mention} {_playerQueueService.DirectMessage}");
                    _playerQueueService.DirectMessage = string.Empty;
                }

                if(!string.IsNullOrWhiteSpace(_playerQueueService.LocalNotification))
                {
                    await Context.Message.ReplyAsync($"{Context.User.Mention} {_playerQueueService.LocalNotification}");
                    _playerQueueService.LocalNotification = string.Empty;
                }

                if(!string.IsNullOrWhiteSpace(_playerQueueService.GlobalNotification))
                {
                    foreach (PlayerQueueGuildModel guild in _playerQueueService.GuildIdList)
                    {
                        await _discordClient.GetGuild(guild.GuildId).GetTextChannel(guild.ChannelId).SendMessageAsync($"{_playerQueueService.GlobalNotification}");
                    }

                    _playerQueueService.GlobalNotification = string.Empty;
                }
            }
            else
            {
                await Context.Message.ReplyAsync($"{Context.User.Mention} Please enter your Activision Id.");
            }
        }

        [Command("leaveq", RunMode = RunMode.Async)]
        public async Task LeaveQue()
        {
            _playerQueueService.LeaveQueue(Context.User);
            if (!string.IsNullOrWhiteSpace(_playerQueueService.DirectMessage))
            {
                await Context.User.SendMessageAsync($"{Context.User.Mention} {_playerQueueService.DirectMessage}");
                _playerQueueService.DirectMessage = string.Empty;
            }
        }

        [Command("qstatus", RunMode = RunMode.Async)]
        public async Task GetQueInformation()
        {
            _playerQueueService.GetQueueStatus();
            if (!string.IsNullOrWhiteSpace(_playerQueueService.LocalNotification))
            {
                await Context.Message.ReplyAsync($"{Context.User.Mention} {_playerQueueService.LocalNotification}");
                _playerQueueService.LocalNotification = string.Empty;
            }

            if(!string.IsNullOrWhiteSpace(_playerQueueService.GlobalNotification))
            {
                foreach (PlayerQueueGuildModel guild in _playerQueueService.GuildIdList)
                {
                    await _discordClient.GetGuild(guild.GuildId).GetTextChannel(guild.ChannelId).SendMessageAsync($"{_playerQueueService.GlobalNotification}");
                }

                _playerQueueService.GlobalNotification = string.Empty;
                _playerQueueService.GuildIdList.Clear();
            }
        }

        //Scrimmage Commands
        [Command("acceptscrim", RunMode = RunMode.Async)]
        public async Task AcceptScrim(string args)
        {
            if(args != null)
            {
                _playerQueueService.AcceptScrim(Context.User, args);
                await Context.Message.ReplyAsync($"{_playerQueueService.LocalNotification}");
            }
            else
            {
                await Context.Message.ReplyAsync($"{Context.User.Mention} Please enter the Scrim Id you'd like to accept.");
            }
        }

        [Command("scrims", RunMode = RunMode.Async)]
        public async Task ScrimQueueStatus()
        {
            _playerQueueService.GetScrimQueueStatus();
            await Context.Message.ReplyAsync($"{_playerQueueService.LocalNotification}");
        }

        //Secret Command for League Staff Only
        [Command("clearscrimq", RunMode = RunMode.Async)]
        public async Task ClearScrimQueue()
        {
            _playerQueueService.ClearScrimQueue();
            await Context.Message.ReplyAsync($"{_playerQueueService.LocalNotification}");
        }
        //

        [Command("createleague", RunMode = RunMode.Async)]
        public async Task CreateLeague(string args)
        {
            await Context.Message.ReplyAsync("Creating League, please wait...");
            await _validation.ValidateLeagueCredentialsAsync(Context, args);
            await Context.Message.ReplyAsync(_validation.RespondMessage);
        }

        //Standing and Groups
        [Command("standings", RunMode = RunMode.Async)]
        public async Task DisplayTeamStandings()
        {
            bool isLeagueStaff = false;
            var leagueStaffRole = Context.Guild.Roles.FirstOrDefault(x => x.Name == "League Staff");
            SocketGuildUser discordUser = Context.User as SocketGuildUser;

            if (discordUser != null)
            {
                foreach (SocketRole role in discordUser.Roles)
                {
                    if(role == leagueStaffRole)
                    {
                        isLeagueStaff = true;
                        break;
                    }
                }
            }

            if(isLeagueStaff)
            {
                LeagueStandings leagueStandings = new LeagueStandings(_search);
                leagueStandings.SortTeamsByGroup(Context);
            }
            else
            {
                await Context.Message.ReplyAsync($"{Context.User.Mention}\r" +
                    $"You do not have permission to use this command.");
            }
        }
    }
}
