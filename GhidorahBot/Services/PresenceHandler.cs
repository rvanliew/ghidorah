using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GhidorahBot.Common;
using GhidorahBot.Database;
using GhidorahBot.Init;
using GhidorahBot.Modules;

namespace GhidorahBot.Services
{
    public class PresenceHandler : IPresenceHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public PresenceHandler(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), Bootstrapper.ServiceProvider);
            _client.PresenceUpdated += HandlePresenceAsync;
        }

        private async Task HandlePresenceAsync(SocketUser arg1, SocketPresence arg2, SocketPresence arg3)
        {
            throw new NotImplementedException();
        }
    }
}
