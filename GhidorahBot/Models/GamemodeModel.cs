using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class GamemodeModel
    {
        public string Id { get; set; }
        public string ModeName { get; set; }
        public GamemodeModel(string id, string modeName)
        {
            Id = id;
            ModeName = modeName;
        }
    }
}
