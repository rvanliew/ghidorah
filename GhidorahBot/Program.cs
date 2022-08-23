using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Interactions;
using GhidorahBot.Common;
using GhidorahBot.Init;
using GhidorahBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GhidorahBot.Database;
using Google.Apis.Sheets.v4;
using GhidorahBot.Validation;

var config = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var discordConfig = new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.All,
    LogGatewayIntentWarnings = false,
    AlwaysDownloadUsers = true,
    LogLevel = LogSeverity.Debug
};

var client = new DiscordSocketClient(discordConfig);

var commands = new CommandService(new CommandServiceConfig
{
    LogLevel = LogSeverity.Info,
    CaseSensitiveCommands = false,
});

var interactionService = new InteractionServiceConfig
{
    DefaultRunMode = Discord.Interactions.RunMode.Async
};

var interactionCommands = new InteractionService(client, interactionService);

GoogleCredentialHandler credentials = new GoogleCredentialHandler();
SheetsService service = credentials.GetCredentials();

var search = new Search(config, service);
var update = new Update(config, service, search);
var newentry = new NewEntry(config, service);
var feedback = new Feedback(config, service);
var playerQue = new PlayerQueueService(client);
var createNewSpreadSheet = new CreateNewSpreadSheet(config, service);
var createLeague = new CreateCodLeague(createNewSpreadSheet, search, newentry);
var validation = new DataValidation(search, update, newentry, feedback, config, service, client, createLeague);

// Setup your DI container.
Bootstrapper.Init();
Bootstrapper.RegisterInstance(client);
Bootstrapper.RegisterInstance(commands);
Bootstrapper.RegisterInstance(interactionCommands);
Bootstrapper.RegisterType<ICommandHandler, CommandHandler>();
Bootstrapper.RegisterType<IInteractionHandler, InteractionHandler>();
Bootstrapper.RegisterInstance(config);
Bootstrapper.RegisterInstance(search);
Bootstrapper.RegisterInstance(update);
Bootstrapper.RegisterInstance(newentry);
Bootstrapper.RegisterInstance(feedback);
Bootstrapper.RegisterInstance(playerQue);
Bootstrapper.RegisterInstance(createNewSpreadSheet);
Bootstrapper.RegisterInstance(createLeague);
Bootstrapper.RegisterInstance(validation);

await MainAsync();

async Task MainAsync()
{
    await Bootstrapper.ServiceProvider.GetRequiredService<ICommandHandler>().InitializeAsync(search, playerQue, validation);
    await Bootstrapper.ServiceProvider.GetRequiredService<IInteractionHandler>().InitializeAsync(validation, playerQue);

    // Login and connect.
    var token = config.GetRequiredSection("Settings")["DiscordBotToken"];
    if (string.IsNullOrWhiteSpace(token))
    {
        await Logger.Log(LogSeverity.Error, $"{nameof(Program)} | {nameof(MainAsync)}", "Token is null or empty.");
        return;
    }

    await client.LoginAsync(TokenType.Bot, token);
    await client.StartAsync();

    client.Ready += async () =>
    {
        try
        {
            await interactionCommands.RegisterCommandsGloballyAsync(true);
            await Logger.Log(LogSeverity.Info, "Interaction Commands", $"Registered");
        }
        catch (Exception ex)
        {
            await Logger.Log(LogSeverity.Info, "", $"ERROR: {ex}");
        }
    };

    playerQue.ClearQueueHandler();

    // Wait infinitely so your bot actually stays connected.
    await Task.Delay(Timeout.Infinite);
}