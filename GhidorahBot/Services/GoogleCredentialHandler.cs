using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Services
{
    public class GoogleCredentialHandler
    {
        private static SheetsService _sheetService;
        private static readonly string[] _scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string applicationName = "LeagueStatsDatabase";

        public SheetsService GetCredentials()
        {
            GoogleCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(_scopes);
            }

            _sheetService = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });

            return _sheetService;
        }
    }
}
