using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class TeamStatTotalsModel
    {
        public string Id { get; set; }
        public string TeamName { get; set; }
        public string TotalMapsWon { get;set; }
        public string TotalMapsLost { get;set; }
        public string MapWinPercentage { get; set; }
        public string MatchWins { get; set; }
        public string MatchLoses { get; set; }
        public string TotalMatches { get; set; }
        public string MatchWinPercentage { get; set; }

        public TeamStatTotalsModel(string id,
            string teamName,
            string totalMapsWon,
            string totalMapsLost,
            string mapWinPercentage,
            string matchWins,
            string matchLoses,
            string totalMatches,
            string matchWinPercentage)
        {
            Id = id;
            TeamName = teamName;
            TotalMapsWon = totalMapsWon;
            TotalMapsLost = totalMapsLost;
            MapWinPercentage = mapWinPercentage;
            MatchWins = matchWins;
            MatchLoses = matchLoses;
            TotalMatches = totalMatches;
            MatchWinPercentage = matchWinPercentage;
        }
    }
}
