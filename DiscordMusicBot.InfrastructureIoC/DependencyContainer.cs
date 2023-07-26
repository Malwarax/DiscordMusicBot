using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordMusicBot.Application.Services;
using DiscordMusicBot.Domain.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.InfrastructureIoC
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {
            //app services
            services.AddSingleton<MusicBotService>();
            services.AddSingleton<CommandHandler>();
            services.AddSingleton<LoggingService>();
            services.AddSingleton<MusicHandler>();
            services.AddSingleton<LinkHandler>();

            //discord.net services
            services.AddSingleton<CommandService>();
            services.AddSingleton<DiscordSocketConfig>(s => new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            });
            services.AddSingleton<DiscordSocketClient>();

            //options
            services.AddOptions<BotOptions>().BindConfiguration(nameof(BotOptions));
        }
    }
}
