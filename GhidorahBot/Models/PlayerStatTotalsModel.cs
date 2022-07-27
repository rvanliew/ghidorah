using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class PlayerStatTotalsModel
    {
        public string Id { get; set; }
        public string PlayerName { get; set; }
        public string Kills { get; set; }
        public string Deaths { get; set; }
        public string HillTime { get; set; }
        public string BombsPlanted { get; set; }
        public string ObjKills { get; set; }
        public string KdRatio { get; set; }

        public PlayerStatTotalsModel(
            string id, 
            string playerName, 
            string kills, 
            string deaths, 
            string hillTime, 
            string bombsPlanted, 
            string objKills, 
            string kdRatio)
        {
            Id = id;
            PlayerName = playerName;
            Kills = kills;
            Deaths = deaths;
            HillTime = hillTime;
            BombsPlanted = bombsPlanted;
            ObjKills = objKills;
            KdRatio = kdRatio;
        }
    }
}
