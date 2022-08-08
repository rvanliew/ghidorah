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
        public InteractionService Commands { get; set; }

        public InteractiveModule()
        {
            //
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
                .AddTextInput("Team Captain", "team_captain", TextInputStyle.Short, "(Discord Name)Team Captain", 1, 254, true)
                .AddTextInput("Team Manager", "team_manager", TextInputStyle.Short, "(Discord Name)Team Manager", 1, 254, true);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        [SlashCommand("updateteam", "Update Team", false, RunMode.Async)]
        public async Task UpdateTeam()
        {
            var mb = new ModalBuilder()
                .WithTitle("Update Team Information")
                .WithCustomId("modal_updateteam")
                .AddTextInput("Search by Team Name", "search_team_Name", TextInputStyle.Short, "", 1, 254, true)
                .AddTextInput("Edit Name", "edit_team_Name", TextInputStyle.Short, "Team Name", 1, 254, false)
                .AddTextInput("Edit Twitter", "edit_team_twitter", TextInputStyle.Short, "@ExampleTwitter", 1, 254, false)
                .AddTextInput("Edit Group", "edit_team_group", TextInputStyle.Short, "A", 1, 1, false)
                .AddTextInput("Edit Captain,Manager (Comma Delimited)", "edit_team_leads", TextInputStyle.Short, "", 1, 400, false);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        //Player Commands
        [SlashCommand("newplayer", "New Player", false, RunMode.Async)]
        public async Task NewPlayer()
        {
            var mb = new ModalBuilder()
                .WithTitle("Add New Player")
                .WithCustomId("modal_addnewplayer")
                .AddTextInput("Activision Id", "player_activ_id", TextInputStyle.Short, "TestPlayer#123456", 1, 254, true)
                .AddTextInput("Discord Name", "player_discord_name", TextInputStyle.Short, "testplayer#1234", 1, 254, false)
                .AddTextInput("Twitter", "player_twitter", TextInputStyle.Short, "@testplayer", 1, 254, false);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        [SlashCommand("updateplayer", "Update Player", false, RunMode.Async)]
        public async Task UpdatePlayer()
        {
            var mb = new ModalBuilder()
                .WithTitle("Update Player Information")
                .WithCustomId("modal_updateplayer")
                .AddTextInput("Search Player by Activision Id or Player Id", "search_player_id", TextInputStyle.Short, "TestPlayer#123456 or 1", 1, 254, true)
                .AddTextInput("Activision Id", "edit_player_activ_id", TextInputStyle.Short, "TestPlayer#123456", 1, 254, false)
                .AddTextInput("Discord Name", "edit_player_discord_name", TextInputStyle.Short, "testplayer#1234", 1, 254, false)
                .AddTextInput("Twitter", "edit_player_twitter", TextInputStyle.Short, "@testplayer", 1, 254, false)
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
                .AddTextInput("Maps won", "team_1_maps_won", TextInputStyle.Short, "1-9", 1, 1, true)
                .AddTextInput("Team 2 Name", "team_2_name", TextInputStyle.Short, "Team Two Name", 1, 254, true)
                .AddTextInput("Maps won", "team_2_maps_won", TextInputStyle.Short, "1-9", 1, 1, true)
                .AddTextInput("Group", "group_id", TextInputStyle.Short, "", 1, 1, true);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        [SlashCommand("updatematchresult", "Update Match Result", false, RunMode.Async)]
        public async Task UpdateMatchResult()
        {
            var mb = new ModalBuilder()
                .WithTitle("Update Match Result")
                .WithCustomId("modal_updatematchresult")
                .AddTextInput("Search Match GUID", "updatematch_searchmatchGUID", TextInputStyle.Short, "EXAMPLE-GUID-1234-ABCD-12345ABCDEF", 1, 254, true)
                .AddTextInput("Team 1 Name", "updatematch_team_1_name", TextInputStyle.Short, "Team One Name", 1, 254, true)
                .AddTextInput("Maps won", "updatematch_team_1_maps_won", TextInputStyle.Short, "1-9", 1, 1, true)
                .AddTextInput("Team 2 Name", "updatematch_team_2_name", TextInputStyle.Short, "Team Two Name", 1, 254, true)
                .AddTextInput("Maps won", "updatematch_team_2_maps_won", TextInputStyle.Short, "1-9", 1, 1, true);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        //PlayerStats Commands
        [SlashCommand("newplayerstats", "New Player Stats", false, RunMode.Async)]
        public async Task NewPlayerStats()
        {
            var mb = new ModalBuilder()
                .WithTitle("New Player Stats")
                .WithCustomId("modal_newplayerstats")
                .AddTextInput("Match Guid", "newstats_search_matchguid", TextInputStyle.Short, "EXAMPLE-GUID-1234-ABCD-12345ABCDEF", 1, 254, true)
                .AddTextInput("Activision Id", "newstats_playername", TextInputStyle.Short, "TestPlayer#123456", 1, 254, true)
                .AddTextInput("Map and Mode (comma delimited)", "newstats_mapmode", TextInputStyle.Short, "Map,Mode", 1, 254, true)
                .AddTextInput("Kills,Deaths,HP_Time,BombsPlanted,ObjKills", "newstats_stats", 
                TextInputStyle.Short, $"(Comma delimited)kills,deaths,HP_Time(hh:mm:ss),bombsplanted,objkills", 1, 500, true);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }
        [SlashCommand("updateplayerstats", "Update Player Stats", false, RunMode.Async)]
        public async Task UpdatePlayerStats()
        {
            var mb = new ModalBuilder()
                .WithTitle("Update Player Stats")
                .WithCustomId("modal_updateplayerstats")
                .AddTextInput("Id", "updatestats_id", TextInputStyle.Short, "", 1, 254, true)
                .AddTextInput("Activision Id", "updatestats_playername", TextInputStyle.Short, "TestPlayer#123456", 1, 254, false)
                .AddTextInput("Map and Mode (comma delimited)", "updatestats_mapmode", TextInputStyle.Short, "Map,Mode", 1, 254, false)
                .AddTextInput("Kills,Deaths,HP_Time,BombsPlanted,ObjKills", "updatestats_stats",
                TextInputStyle.Short, $"(Comma delimited)kills,deaths,HP_Time(hh:mm:ss),bombsplanted,objkills", 1, 500, false);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        //Roster Commands 
        [SlashCommand("updateroster", "Update Team Roster", false, RunMode.Async)]
        public async Task UpdateRoster()
        {
            var mb = new ModalBuilder()
                .WithTitle("Update Team Roster")
                .WithCustomId("modal_updateRoster")
                .AddTextInput("Search by Team Name or Team Id", "updateroster_search_team_name", TextInputStyle.Short, "Team name or Id", 1, 254, true)
                .AddTextInput("Remove Player(s) (Comma Delimited)", "updateroster_removeplayer", TextInputStyle.Short, "TestPlayer#123456", 1, 1000, false)
                .AddTextInput("Add Player(s) (Comma Delimited)", "updateroster_addplayer", TextInputStyle.Short, "TestPlayer#123456", 1, 1000, false);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        //Search
        [SlashCommand("searchplayer", "Search for Player", false, RunMode.Async)]
        public async Task SearchPlayer()
        {
            var mb = new ModalBuilder()
                .WithTitle("Search for a specific player")
                .WithCustomId("modal_searchplayer")
                .AddTextInput("Player search", "search_player", TextInputStyle.Short, "TestPlayer#123456", 1, 254, true);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        [SlashCommand("searchteam", "Search for Team", false, RunMode.Async)]
        public async Task SearchTeam()
        {
            var mb = new ModalBuilder()
                .WithTitle("Search for a specific Team")
                .WithCustomId("modal_searchteam")
                .AddTextInput("Team search", "search_team", TextInputStyle.Short, "Team Name", 1, 254, true);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }

        [SlashCommand("searchmatchresult", "Search for a Match Result", false, RunMode.Async)]
        public async Task SearchMatchResult()
        {
            var mb = new ModalBuilder()
                .WithTitle("Search for a specific Match Result")
                .WithCustomId("modal_searchMatchResult")
                .AddTextInput("Match Result GUID Search", "search_matchresultguid", TextInputStyle.Short, "EXAMPLE-GUID-1234-ABCD-12345ABCDEF", 1, 254, true);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }
        //Feedback Command
        [SlashCommand("feedback", "Help us improve your experience", false, RunMode.Async)]
        public async Task Feedback()
        {
            var mb = new ModalBuilder()
                .WithTitle("Help us improve your experience")
                .WithCustomId("modal_feedback")
                .AddTextInput("What improvements would you like to see?", "feedback_improvements", TextInputStyle.Paragraph, "", 1, 4000, false)
                .AddTextInput("What do you like about the bot?", "feedback_like", TextInputStyle.Paragraph, "", 1, 4000, false)
                .AddTextInput("What do you dislike about the bot?", "feedback_dislike", TextInputStyle.Paragraph, "", 1, 4000, false)               
                .AddTextInput("Other", "feedback_other", TextInputStyle.Paragraph, "", 1, 4000, false);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }
        //Free Agent Command
        [SlashCommand("freeagent", "Free Agent", false, RunMode.Async)]
        public async Task FreeAgent()
        {
            var mb = new ModalBuilder()
                .WithTitle("Free Agent Finder")
                .WithCustomId("modal_freeagent")
                .AddTextInput("Type", "fa_type", TextInputStyle.Short, "F/A, TO3, TO2", 1, 10, false)
                .AddTextInput("Player Age", "fa_age", TextInputStyle.Short, "18", 1, 2, false)
                .AddTextInput("Role", "fa_role", TextInputStyle.Short, "flex, ar, sub", 1, 10, false)
                .AddTextInput("Region & Time Zone", "fa_regiontimezone", TextInputStyle.Short, "NA PST", 1, 254, false)
                .AddTextInput("Other Notes", "fa_other", TextInputStyle.Paragraph, "Any additional information goes here", 1, 4000, false);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }
        //Request Admin
        [SlashCommand("requestadmin", "Request a League Admin", false, RunMode.Async)]
        public async Task RequestAdmin()
        {
            var mb = new ModalBuilder()
                .WithTitle("Request an Admin")
                .WithCustomId("modal_requestadmin")
                .AddTextInput("Issue", "admin_issue", TextInputStyle.Paragraph, "Describe in a few sentences the issue you are having", 1, 1000, true);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }
        //Scrimmage
        [SlashCommand("newscrim", "Create a new team scrim", false, RunMode.Async)]
        public async Task NewScrim()
        {
            var mb = new ModalBuilder()
                .WithTitle("Create a new team scrim")
                .WithCustomId("modal_newscrim")
                .AddTextInput("Your Activision Id", "scrim_activisionId", TextInputStyle.Short, "", 1, 254, true)
                .AddTextInput("Custom Scrim Date/Time", "scrim_time", TextInputStyle.Short, "The Date/Time you would like to host your scrim", 1, 254, false)
                .AddTextInput("Other Notes", "scrim_notes", TextInputStyle.Paragraph, "Additional notes about your scrimmage.\rExample: Hardpoint only", 1, 500, false);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }
        [SlashCommand("registeradminchannel", "Register your Admin Channnel", false, RunMode.Async)]
        public async Task RegisterAdminChannel()
        {
            var mb = new ModalBuilder()
                .WithTitle("Register your admin channel")
                .WithCustomId("modal_registeradminchnl")
                .AddTextInput("Admin Channel Id", "admin_chnl", TextInputStyle.Short, "Channel ID", 1, 254, false);

            await Context.Interaction.RespondWithModalAsync(mb.Build());
        }
    }
}
