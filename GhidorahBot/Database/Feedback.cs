﻿using Discord.WebSocket;
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
        public string RespondFeedbackMessage = string.Empty;

        private SheetsService _service;
        private IConfiguration _config;

        public Feedback(IConfiguration config, SheetsService service)
        {
            _config = config;
            _service = service;
        }

        public void AddFeedback(SocketModal modal, string feedbackSheetName, int id)
        {
            try
            {
                var fullDiscordName = $"{modal.User.Username}#{modal.User.Discriminator}";

                var modalName = modal.Data.CustomId;
                var components = modal.Data.Components.ToList();

                string type = components
                .First(x => x.CustomId == "feedback_feedbacktype").Value;

                string feedback = components
                .First(x => x.CustomId == "feedback").Value;

                var range = $"{feedbackSheetName}!A:E";
                var valueRange = new ValueRange();

                var objectList = new List<object>() { id, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), fullDiscordName, type, feedback};

                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequest = _service.Spreadsheets.Values.Append(valueRange, _config.GetRequiredSection("Settings")["GoogleSheetsId"], range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.Execute();
            }
            catch (Exception ex)
            {
                RespondFeedbackMessage = $"{modal.User.Mention}\r" +
                    $"An Error has occured while submitting feedback.\r\r" +
                    $"Exception: {ex}";
            }
            
        }
    }
}
