using Discord.WebSocket;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Database
{
    public class Feedback
    {
        private SheetsService _service;
        private IConfiguration _config;
        private string _sheetName = "Feedback";
        private int _id = 1;

        public Feedback(IConfiguration config, SheetsService service)
        {
            _config = config;
            _service = service;
        }

        private void IncrementId()
        {
            var range = $"{_sheetName}!A2:A";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

            var reponse = request.ExecuteAsync();
            var requestResponse = reponse.Result;

            if (requestResponse != null && requestResponse.Values != null && requestResponse.Values.Count > 0)
            {
                var lastId = requestResponse.Values.Last();
                _id = Convert.ToInt32(lastId[0]);
                _id++;
            }
        }

        public void AddFeedback(SocketModal modal)
        {
            IncrementId();

            var discordName = modal.User.Username;

            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            string like = components
            .First(x => x.CustomId == "feedback_like").Value;

            string dislike = components
            .First(x => x.CustomId == "feedback_dislike").Value;

            string improvements = components
            .First(x => x.CustomId == "feedback_improvements").Value;

            string other = components
            .First(x => x.CustomId == "feedback_other").Value;

            var range = $"{_sheetName}!A:G";
            var valueRange = new ValueRange();

            var objectList = new List<object>() { _id, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), discordName, like, dislike, improvements, other };

            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
        }
    }
}
