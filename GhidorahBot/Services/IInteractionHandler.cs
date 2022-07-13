using GhidorahBot.Database;
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
            Roster roster,
            Search search,
            Update editDatabase,
            NewEntry newDatabase,
            Feedback feedback);
    }
}
