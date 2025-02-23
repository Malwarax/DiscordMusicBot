using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using DiscordMusicBot.Domain.Configuration;
using Microsoft.Extensions.Options;

namespace DiscordMusicBot.Application.EventHandlers;

public class CommandHandler(CommandService commandService,
    IOptions<BotOptions> botOptions,
    DiscordSocketClient socketClient,
    IServiceProvider serviceProvider)
{
    public async Task InstallCommandsAsync()
    {
        socketClient.MessageReceived += HandleCommandAsync;

        await commandService.AddModulesAsync(assembly: Assembly.GetExecutingAssembly(),
            services: serviceProvider);
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        var message = messageParam as SocketUserMessage;

        if (message == null)
        {
            return;
        }

        int argPos = 0;
        bool isValidCommand = message.HasStringPrefix(botOptions.Value.CommandPrefix, ref argPos)
                              || message.HasMentionPrefix(socketClient.CurrentUser, ref argPos)
                              || message.Author.IsBot;

        if (!isValidCommand)
        {
            return;
        }

        var context = new SocketCommandContext(socketClient, message);

        await commandService.ExecuteAsync(
            context: context,
            argPos: argPos,
            services: serviceProvider);
    }
}
