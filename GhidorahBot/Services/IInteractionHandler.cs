using GhidorahBot.Database;
using GhidorahBot.Validation;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Services
{
    public interface IInteractionHandler
    {
        Task InitializeAsync(
            DataValidation validation, PlayerQueueService playerQueueService);
    }
}
