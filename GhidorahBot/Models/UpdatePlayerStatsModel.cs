using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class UpdatePlayerStatsModel
    {
        public string Id { get; set; }
        public string Guid { get; set; }
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string Map { get; set; }
        public string Mode { get; set; }
        public string Kills { get; set; }
        public string Deaths { get; set; }
        public string HillTime { get; set; }
        public string BombsPlanted { get; set; }
        public string ObjKills { get; set; }
        public string LastUpdatedDt { get; set; }
        public string LastUpdatedBy { get; set; }
        public string DateCreated { get; set; }

        public UpdatePlayerStatsModel(string id,
            string guid,
            string playerId,
            string playerName,
            string map,
            string mode,
            string kills,
            string deaths,
            string hillTime,
            string bombsPlanted,
            string objKills,
            string lastUpdatedDt,
            string lastUpdatedBy,
            string dateCreated)
        {
            Id = id;
            Guid = guid;
            PlayerId = playerId;
            PlayerName = playerName;
            Map = map;
            Mode = mode;
            Kills = kills;
            Deaths = deaths;
            HillTime = hillTime;
            BombsPlanted = bombsPlanted;
            ObjKills = objKills;
            LastUpdatedDt = lastUpdatedDt;
            LastUpdatedBy = lastUpdatedBy;
            DateCreated = dateCreated;
        }
    }
}
