using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using DiscordMusicBot.Domain.Configuration;
using Microsoft.Extensions.Options;

namespace DiscordMusicBot.Application.Services
{
    public class CommandHandler
    {
        private readonly CommandService _commandService;
        private readonly BotOptions _botOptions;
        private readonly DiscordSocketClient _socketClient;
        private readonly IServiceProvider _serviceProvider;

        public CommandHandler(CommandService commandService,
            IOptions<BotOptions> botOptions,
            DiscordSocketClient socketClient,
            IServiceProvider serviceProvider)
        {
            _commandService = commandService;
            _botOptions = botOptions.Value;
            _socketClient = socketClient;
            _serviceProvider = serviceProvider;
        }

        public async Task InstallCommandsAsync()
        {
            _socketClient.MessageReceived += HandleCommandAsync;

            await _commandService.AddModulesAsync(assembly: Assembly.GetExecutingAssembly(),
                services: _serviceProvider);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;

            if (message == null)
            {
                return;
            }

            int argPos = 0;
            bool isValidCommand = message.HasStringPrefix(_botOptions.CommandPrefix, ref argPos) 
                                  || message.HasMentionPrefix(_socketClient.CurrentUser, ref argPos)
                                  || message.Author.IsBot;

            if (!isValidCommand)
            {
                return;
            }
            
            var context = new SocketCommandContext(_socketClient, message);

            await _commandService.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _serviceProvider);
        }
    }
}
