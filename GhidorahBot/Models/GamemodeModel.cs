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
        public string Key { get; set; }
        public string LastUpdated { get; set; }
        public string LastUpdatedBy { get; set; }
        public GamemodeModel(string id, string modeName, string key, string lastUpdated, string lastUpdatedBy)
        {
            Id = id;
            ModeName = modeName;
            Key = key;
            LastUpdated = lastUpdated;
            LastUpdatedBy = lastUpdatedBy;
        }
    }
}
