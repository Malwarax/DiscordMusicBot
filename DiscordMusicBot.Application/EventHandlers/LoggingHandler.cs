using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordMusicBot.Application.EventHandlers
{
    public class LoggingHandler
    {
        private readonly CommandService _commandService;
        private readonly DiscordSocketClient _socketClient;

        public LoggingHandler(CommandService commandService,
            DiscordSocketClient socketClient)
        {
            _commandService = commandService;
            _socketClient = socketClient;
        }

        public void InstallLogging()
        {
            _socketClient.Log += LogAsync;
            _commandService.Log += LogAsync;
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
}
