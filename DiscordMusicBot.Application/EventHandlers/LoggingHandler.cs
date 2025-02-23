using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordMusicBot.Application.EventHandlers;

public class LoggingHandler(CommandService commandService,
    DiscordSocketClient socketClient)
{
    public void InstallLogging()
    {
        socketClient.Log += LogAsync;
        commandService.Log += LogAsync;
    }

    private Task LogAsync(LogMessage message)
    {
        if (message.Exception is CommandException cmdException)
        {
            Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                              + $" failed to execute in {cmdException.Context.Channel}.");
            Console.WriteLine(cmdException);
        }
        else
        {
            Console.WriteLine($"[General/{message.Severity}] {message}");
        }

        return Task.CompletedTask;
    }
}
