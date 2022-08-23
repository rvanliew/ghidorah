using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class MapModel
    {
        public string Id { get; set; }
        public string MapName { get; set; }
        public string LastUpdateDt { get; set; }
        public string LastUpdatedBy { get; set; }
        public MapModel(string id, string mapName, string lastUpdateDt, string lastUpdatedBy)
        {
            Id = id;
            MapName = mapName;
            LastUpdateDt = lastUpdateDt;
            LastUpdatedBy = lastUpdatedBy;
        }
    }
}
