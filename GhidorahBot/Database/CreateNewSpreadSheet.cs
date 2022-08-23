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
    public class CreateNewSpreadSheet
    {
        private SheetsService _service;
        private IConfiguration _config;

        private string _sheetName = "";

        public CreateNewSpreadSheet(IConfiguration config, SheetsService service)
        {
            _config = config;
            _service = service;
        }

        public async Task CreateSpreadSheetAsync(string guildName, string sheetName)
        {
            _sheetName = string.Format($"{guildName}_{sheetName}");
            var addSheetRequest = new AddSheetRequest();
            addSheetRequest.Properties = new SheetProperties();
            addSheetRequest.Properties.Title = _sheetName;
            BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
            batchUpdateSpreadsheetRequest.Requests = new List<Request>();
            batchUpdateSpreadsheetRequest.Requests.Add(new Request
            {
                AddSheet = addSheetRequest
            });

            var batchUpdateRequest =
                _service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, _config.GetRequiredSection("Settings")["GoogleSheetsId"]);

            await batchUpdateRequest.ExecuteAsync();
        }
    }
}
