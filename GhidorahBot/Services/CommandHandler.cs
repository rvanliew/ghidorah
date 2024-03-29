﻿using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GhidorahBot.Common;
using GhidorahBot.Database;
using GhidorahBot.Init;
using GhidorahBot.Modules;
using GhidorahBot.Validation;

namespace GhidorahBot.Services
{
    public class CommandHandler : ICommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        private Search _search { get; set; }
        private PlayerQueueService _playerQueue { get; set; }
        private DataValidation _validation { get; set; }

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;
        }

        public async Task InitializeAsync(Search search, PlayerQueueService playerQueueService, DataValidation validation)
        {
            _search = search;
            _playerQueue = playerQueueService;
            _validation = validation;

            var commondCommands = new CommonCommandsModule(_search, _playerQueue, _client, _validation);
            // add the public modules that inherit InteractionModuleBase<T> to the InteractionService
            await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), Bootstrapper.ServiceProvider);
        
            // Subscribe a handler to see if a message invokes a command.
            _client.MessageReceived += HandleCommandAsync;
            

            _commands.CommandExecuted += async (optional, context, result) =>
            {
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    // the command failed, let's notify the user that something happened.
                    await context.Channel.SendMessageAsync($"error: {result}");
                }
            };

            foreach (var module in _commands.Modules)
            {
                await Logger.Log(LogSeverity.Info, $"{nameof(CommandHandler)} | Commands", $"Module '{module.Name}' initialized.");
            }
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            // Bail out if it's a System Message.
            if (arg is not SocketUserMessage msg)
                return;

            // We don't want the bot to respond to itself or other bots.
            if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot)
                return;

            // Create a Command Context.
            var context = new SocketCommandContext(_client, msg);

            var markPos = 0;
            if (msg.HasCharPrefix('!', ref markPos) || msg.HasCharPrefix('?', ref markPos))
            {
                var result = await _commands.ExecuteAsync(context, markPos, Bootstrapper.ServiceProvider);
            }
        }
    }
}
