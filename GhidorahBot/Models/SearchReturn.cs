using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class SearchReturn
    {
        public bool MatchFound { get; set; }
        public string Id { get;set; }
        public string Name { get; set; }

        public SearchReturn(bool matchFound, string id, string name)
        {
            MatchFound = matchFound;
            Id = id;
            Name = name;
        }
    }
}
