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
        public MapModel(string id, string mapName)
        {
            Id = id;
            MapName = mapName;
        }
    }
}
