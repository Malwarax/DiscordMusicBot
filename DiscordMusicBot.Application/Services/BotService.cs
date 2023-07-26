using Discord;
using Discord.WebSocket;
using DiscordMusicBot.Domain.Configuration;
using Microsoft.Extensions.Options;

namespace DiscordMusicBot.Application.Services
{
    public class BotService
    {
        private readonly BotOptions _botOptions;
        private readonly CommandHandler _commandHandler;
        private readonly DiscordSocketClient _socketClient;
        private readonly LoggingService _loggingService;

        public BotService(IOptions<BotOptions> botOptions,
            CommandHandler commandHandler,
            DiscordSocketClient socketClient,
            LoggingService loggingService)
        {
            _botOptions = botOptions.Value;
            _commandHandler = commandHandler;
            _socketClient = socketClient;
            _loggingService = loggingService;
        }

        public async Task StartAsync()
        {
            _loggingService.InstallLogging();
            await _commandHandler.InstallCommandsAsync();
            await _socketClient.LoginAsync(TokenType.Bot, _botOptions.Token);
            await _socketClient.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
    }
}
