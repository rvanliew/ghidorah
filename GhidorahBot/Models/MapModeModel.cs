using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class MapModeModel
    {
        public string Map { get; set; }
        public string Mode { get; set; }

        public MapModeModel(string map, string mode)
        {
            Map = map;
            Mode = mode;
        }
    }
}
