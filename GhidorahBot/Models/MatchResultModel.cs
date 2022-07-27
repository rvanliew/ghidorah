using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class MatchResultModel
    {
        public string Id { get; set; }
        public string GUID { get; set; }
        public string TeamOneId { get; set; }
        public string TeamOneName { get; set; }
        public string TeamOneMW { get; set; }
        public string TeamOneML { get; set; }
        public string TeamTwoId { get; set; }
        public string TeamTwoName { get; set; }
        public string TeamTwoMW { get; set; }
        public string TeamTwoML { get; set; }

        public MatchResultModel(
            string id, 
            string guid, 
            string teamOneId, 
            string teamOneName, 
            string teamOneMW, 
            string teamOneML, 
            string teamTwoId,
            string teamTwoName,
            string teamTwoMW, 
            string teamTwoML)
        {
            Id = id;
            GUID = guid;
            TeamOneId = teamOneId;
            TeamOneName = teamOneName;
            TeamOneMW = teamOneMW;
            TeamOneML = teamOneML;
            TeamTwoId = teamTwoId;
            TeamTwoName = teamTwoName;
            TeamTwoMW = teamTwoMW;
            TeamTwoML = teamTwoML;
        }
    }
}
