using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GhidorahBot.Database;
using GhidorahBot.Models;
using GhidorahBot.Global;
using Discord;
using Discord.Commands;
using System.Text.RegularExpressions;

namespace GhidorahBot.Services
{
    public class LeagueStandings
    {
        private Search _search { get; set; }

        private string _teamNames;
        private string _records;
        private string _mapWinPercents;
        private string _mapWL;

        static int tableWidth = 73;
        static int _index = 1;
        string _standingsPrintMsg;
        public string StandingsPrintMsg;

        private char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public LeagueStandings(Search search)
        {
            _search = search;
        }

        public async void SortTeamsByGroup(SocketCommandContext context)
        {
            await context.Message.ReplyAsync($"{context.User.Mention} - Please wait while we retrieve the current group standings...");

            string guildNameNoSpace = Regex.Replace(context.Guild.Name, @"\s+", "");
            var teamTotalStatsList = _search.GetTeamTotalStats($"{guildNameNoSpace}_{Values.GoogleSheets.teamStatTotal}");

            if(!teamTotalStatsList.Any())
            {
                await context.Message.ReplyAsync($"{context.User.Mention} Error retrieving group data.");
                return;
            }

            teamTotalStatsList = teamTotalStatsList
                .OrderBy(x => x.Group)
                .ToList();

            for (char c = 'A'; c <= 'Z'; c++)
            {
                var groupList = teamTotalStatsList
                    .Where(x => x.Group.Equals($"{c}"))
                    .OrderByDescending(x => x.MatchWinPercentage)
                    .ToList();

                if(!groupList.Any())
                {
                    await context.Message.ReplyAsync($"Request complete.\rLast Updated: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                    break;
                }

                PrintLine();
                PrintRow("Grp & Team", "Record", "Map W%", "Map W/L");
                PrintLine();


                for(int index = 0; index < groupList.Count; index++)
                {
                    PrintRow($"{groupList[index].Group}-{groupList[index].TeamName}", 
                        $"{groupList[index].MatchWins}-{groupList[index].MatchLoses}", 
                        $"{groupList[index].MapWinPercentage}", $"{groupList[index].TotalMapsWon}-{groupList[index].TotalMapsLost}");
                }

                PrintLine();
                await context.Message.ReplyAsync($"{context.User.Mention}\r" +
                    $"{_standingsPrintMsg}");
                //Pausing 10 seconds so Discord Message cap can reset
                Thread.Sleep(10000);
                _standingsPrintMsg = string.Empty;

                continue;
            }      
        }

        private void PrintLine()
        {
            _standingsPrintMsg += new string('-', tableWidth);
            _standingsPrintMsg += "\r";
        }

        private void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            _standingsPrintMsg += row;
            _standingsPrintMsg += "\r";
        }

        private string AlignCentre(string text, int width)
        {
            int textLength = text.Length + 7;

            text = textLength > width ? text.Substring(0, width - 7) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }
    }
}
