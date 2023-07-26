using Discord;
using Discord.WebSocket;
using DiscordMusicBot.Application.EventHandlers;
using DiscordMusicBot.Domain.Configuration;
using Microsoft.Extensions.Options;

namespace DiscordMusicBot.Application.Services
{
    public class BotService
    {
        private readonly BotOptions _botOptions;
        private readonly CommandHandler _commandHandler;
        private readonly DiscordSocketClient _socketClient;
        private readonly LoggingHandler _loggingHandler;

        public BotService(IOptions<BotOptions> botOptions,
            CommandHandler commandHandler,
            DiscordSocketClient socketClient,
            LoggingHandler loggingHandler)
        {
            _botOptions = botOptions.Value;
            _commandHandler = commandHandler;
            _socketClient = socketClient;
            _loggingHandler = loggingHandler;
        }

        public async Task StartAsync()
        {
            _loggingHandler.InstallLogging();
            await _commandHandler.InstallCommandsAsync();
            await _socketClient.LoginAsync(TokenType.Bot, _botOptions.Token);
            await _socketClient.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
    }
}
