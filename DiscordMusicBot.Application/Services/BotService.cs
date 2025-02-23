using Discord;
using Discord.WebSocket;
using DiscordMusicBot.Application.EventHandlers;
using DiscordMusicBot.Domain.Configuration;
using Microsoft.Extensions.Options;

namespace DiscordMusicBot.Application.Services;

public class BotService(IOptions<BotOptions> botOptions,
    CommandHandler commandHandler,
    DiscordSocketClient socketClient,
    LoggingHandler loggingHandler)
{
    public async Task StartAsync()
    {
        loggingHandler.InstallLogging();
        await commandHandler.InstallCommandsAsync();
        await socketClient.LoginAsync(TokenType.Bot, botOptions.Value.Token);
        await socketClient.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
}
