﻿using Discord.WebSocket;
using GhidorahBot.Database;
using GhidorahBot.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Services
{
    public interface ICommandHandler
    {
        Task InitializeAsync(Search search, PlayerQueueService playerQueue, DataValidation validation);
    }
}
