using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace GhidorahBot.Modules
{
    public class InteractiveModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DiscordSocketClient _client;
        public InteractionService Commands { get; set; }

        public InteractiveModule(DiscordSocketClient client)
        {
            _client = client;
        }

        //Team Commands
        [SlashCommand("newteam", "New Team", false, RunMode.Async)]
        public async Task NewTeam()
        {
            var mb = new ModalBuilder()
                .WithTitle("Add New Team")
                .WithCustomId("modal_addnewteam")
                .AddTextInput("Team Name", "team_Name", TextInputStyle.Short, "", 1, 254, true)
                .AddTextInput("Team Twitter", "team_twitter", TextInputStyle.Short, "@ExampleTwitter", 1, 254, false)
                .AddTextInput("Group", "team_group", TextInputStyle.Short, "A", 1, 1, true)
                .AddTextInput("Active", "team_active", TextInputStyle.Short, "Y/N", 1, 1, true);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        [SlashCommand("updateteam", "Edit Team", false, RunMode.Async)]
        public async Task UpdateTeam()
        {
            var mb = new ModalBuilder()
                .WithTitle("Edit Team")
                .WithCustomId("modal_editteam")
                .AddTextInput("Search by Team Name or Team Id", "search_team_Name", TextInputStyle.Short, "", 1, 254, true)
                .AddTextInput("Team Name", "edit_team_Name", TextInputStyle.Short, "Team Name or Team Id", 1, 254, false)
                .AddTextInput("Team Twitter", "edit_team_twitter", TextInputStyle.Short, "@ExampleTwitter", 1, 254, false)
                .AddTextInput("Group", "edit_team_group", TextInputStyle.Short, "A", 1, 1, false)
                .AddTextInput("Active", "edit_team_active", TextInputStyle.Short, "Y/N", 1, 1, false);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        //Player Commands
        [SlashCommand("newplayer", "New Player", false, RunMode.Async)]
        public async Task NewPlayer()
        {
            var mb = new ModalBuilder()
                .WithTitle("Add New Player")
                .WithCustomId("modal_addnewplayer")
                .AddTextInput("Activision Id", "player_activ_id", TextInputStyle.Short, "JohnDoe#1234567890", 1, 254, true)
                .AddTextInput("Discord Name", "player_discord_name", TextInputStyle.Short, "johndoe#1234", 1, 254, false)
                .AddTextInput("Twitter", "player_twitter", TextInputStyle.Short, "@johndoe", 1, 254, false)
                .AddTextInput("Active", "player_active", TextInputStyle.Short, "Y/N", 1, 1, true);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        [SlashCommand("updateplayer", "Edit Player", false, RunMode.Async)]
        public async Task UpdatePlayer()
        {
            var mb = new ModalBuilder()
                .WithTitle("Edit Player")
                .WithCustomId("modal_editplayer")
                .AddTextInput("Search Player by Activision Id or Player Id", "search_player_id", TextInputStyle.Short, "JohnDoe#1234567890 or Player Id", 1, 254, true)
                .AddTextInput("Activision Id", "edit_player_activ_id", TextInputStyle.Short, "JohnDoe#1234567890", 1, 254, false)
                .AddTextInput("Discord Name", "edit_player_discord_name", TextInputStyle.Short, "johndoe#1234", 1, 254, false)
                .AddTextInput("Twitter", "edit_player_twitter", TextInputStyle.Short, "@johndoe", 1, 254, false)
                .AddTextInput("Active", "edit_player_active", TextInputStyle.Short, "Y/N", 1, 1, false);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        //Match Commands
        [SlashCommand("newmatchresult", "New Match Result", false, RunMode.Async)]
        public async Task NewMatchResult()
        {
            var mb = new ModalBuilder()
                .WithTitle("Match Result Information")
                .WithCustomId("modal_newmatchresult")
                .AddTextInput("Team 1 Name", "team_1_name", TextInputStyle.Short, "Team One Name", 1, 254, true)
                .AddTextInput("Maps won", "team_1_maps_won", TextInputStyle.Short, "Maps won", 1, 1, true)
                .AddTextInput("Team 2 Name", "team_2_name", TextInputStyle.Short, "Team Two Name", 1, 254, true)
                .AddTextInput("Maps won", "team_2_maps_won", TextInputStyle.Short, "Maps won", 1, 1, true)
                .AddTextInput("Group", "group_id", TextInputStyle.Short, "", 1, 1, true);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        [SlashCommand("updatematchresult", "Update Match Result", false, RunMode.Async)]
        public async Task UpdateMatchResult()
        {
            var mb = new ModalBuilder()
                .WithTitle("Update Match Result")
                .WithCustomId("modal_updatematchresult")
                .AddTextInput("Match Id", "updatematch_matchId", TextInputStyle.Short, "{EXAMPLE-GUID-1234-ABCD-12345ABCDEF}", 1, 254, true)
                .AddTextInput("Team 1 Name", "updatematch_team_1_name", TextInputStyle.Short, "Team One Name", 1, 254, false)
                .AddTextInput("Maps won", "updatematch_team_1_maps_won", TextInputStyle.Short, "1-9", 1, 1, false)
                .AddTextInput("Team 2 Name", "updatematch_team_2_name", TextInputStyle.Short, "Team Two Name", 1, 254, false)
                .AddTextInput("Maps won", "updatematch_team_2_maps_won", TextInputStyle.Short, "1-9", 1, 1, false);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        //PlayerStats Commands
        [SlashCommand("newplayerstats", "New Player Stats", false, RunMode.Async)]
        public async Task NewPlayerStats()
        {
            var mb = new ModalBuilder()
                .WithTitle("New Player Stats")
                .WithCustomId("modal_newplayerstats")
                .AddTextInput("Match Guid", "newstats_search_matchguid", TextInputStyle.Short, "{EXAMPLE-GUID-1234-ABCD-12345ABCDEF}", 1, 254, true)
                .AddTextInput("Activision Id or Player Id", "newstats_playername", TextInputStyle.Short, "JohnDoe#123456 or 190", 1, 254, true)
                .AddTextInput("Map and Mode", "newstats_mapmode", TextInputStyle.Short, "Highrise,snd", 1, 254, true)
                .AddTextInput("Stats - (comma-delimited)", "newstats_stats", TextInputStyle.Short, "kills,deaths,hilltime,bombsplanted,objkills", 1, 254, true);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        //Roster Commands
        [SlashCommand("newroster", "New Team Roster", false, RunMode.Async)]
        public async Task NewTeamRoster()
        {
            var mb = new ModalBuilder()
                .WithTitle("New Team Roster")
                .WithCustomId("modal_newteamroster")
                .AddTextInput("Search by Team Name or Team Id", "newroster_search_team_name", TextInputStyle.Short, "Team name or Id", 1, 254, true)
                .AddTextInput("Add Player(s)", "newroster_addplayer", TextInputStyle.Short, "player1#123456 player2#123456 player3#123456", 1, 254, true);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        [SlashCommand("updateroster", "Update Team Roster", false, RunMode.Async)]
        public async Task UpdateRoster()
        {
            var mb = new ModalBuilder()
                .WithTitle("Update Team Roster")
                .WithCustomId("modal_updateRoster")
                .AddTextInput("Search by Team Name or Team Id", "updateroster_search_team_name", TextInputStyle.Short, "Team name or Id", 1, 254, true)
                .AddTextInput("Add Player", "updateroster_addplayer", TextInputStyle.Short, "JohnDoe#1234567", 1, 254, false)
                .AddTextInput("Remove Player", "updateroster_removeplayer", TextInputStyle.Short, "JohnDoe#1234567", 1, 254, false);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        //Feedback Command
        [SlashCommand("feedback", "How can we improve?", false, RunMode.Async)]
        public async Task Feedback()
        {
            var mb = new ModalBuilder()
                .WithTitle("Feedback: We want to hear from you")
                .WithCustomId("modal_feedback")
                .AddTextInput("What do you like about the bot?", "feedback_like", TextInputStyle.Paragraph, "", 1, 4000, false)
                .AddTextInput("What do you dislike about the bot?", "feedback_dislike", TextInputStyle.Paragraph, "", 1, 4000, false)
                .AddTextInput("What improvements would you like to see?", "feedback_improvements", TextInputStyle.Paragraph, "", 1, 4000, false)
                .AddTextInput("Other", "feedback_other", TextInputStyle.Paragraph, "", 1, 4000, false);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }
    }
}
