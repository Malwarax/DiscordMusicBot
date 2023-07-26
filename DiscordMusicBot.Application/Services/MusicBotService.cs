using Discord;
using Discord.WebSocket;
using DiscordMusicBot.Domain.Configuration;
using Microsoft.Extensions.Options;

namespace DiscordMusicBot.Application.Services
{
    public class MusicBotService
    {
        private readonly BotOptions _botOptions;
        private readonly CommandHandler _commandHandler;
        private readonly DiscordSocketClient _socketClient;

        public MusicBotService(IOptions<BotOptions> botOptions,
            CommandHandler commandHandler,
            DiscordSocketClient socketClient)
        {
            _botOptions = botOptions.Value;
            _commandHandler = commandHandler;
            _socketClient = socketClient;
        }

        public async Task StartAsync()
        {
            _socketClient.Log += Log;
            await _commandHandler.InstallCommandsAsync();
            await _socketClient.LoginAsync(TokenType.Bot, _botOptions.Token);
            await _socketClient.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
