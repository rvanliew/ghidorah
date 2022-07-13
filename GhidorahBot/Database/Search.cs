using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using GhidorahBot.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;

namespace GhidorahBot.Database
{
    public class Search
    {
        private IConfiguration _config;
        private SheetsService _service;

        private bool _matchFound;
        private string _id;
        private string _name;

        public Search(IConfiguration config, SheetsService service)
        {
            _config = config;
            _service = service;
        }

        public (bool, string, string) SearchItem(SocketModal modal, string sheetName, string customId, string sheetRange)
        {
            var range = $"{sheetName}!{sheetRange}";
            var request = _service.Spreadsheets.Values.Get(_config.GetRequiredSection("Settings")["GoogleSheetsId"], range);

            var response = request.ExecuteAsync();
            var valueRangeResult = response.Result;

            var modalName = modal.Data.CustomId;
            var components = modal.Data.Components.ToList();

            string userInput = components
                .First(x => x.CustomId == $"{customId}").Value;

            if(valueRangeResult != null && valueRangeResult.Values.Count > 0)
            {
                foreach(var row in valueRangeResult.Values)
                {
                    if(row.Count <= 1)
                    {
                        _id = row[0].ToString() != "" ? _id = row[0].ToString() : _id = "";
                        _name = string.Empty;
                    }
                    else
                    {
                        _id = row[0].ToString() != "" ? _id = row[0].ToString() : _id = "";
                        _name = row[1].ToString() != "" ? _name = row[1].ToString() : _name = "";
                    }                                   

                    if (_id.ToLower().Equals(userInput.ToLower()) || _name.ToLower().Contains(userInput.ToLower()))
                    {
                        _matchFound = true;
                        break;
                    }
                }
            }

            return (_matchFound, _id, _name);
        }
    }
}
