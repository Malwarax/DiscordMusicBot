using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordMusicBot.Application.EventHandlers;
using DiscordMusicBot.Application.Services;
using DiscordMusicBot.Domain.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.InfrastructureIoC;

public class DependencyContainer
{
    public static void RegisterServices(IServiceCollection services)
    {
        //app services
        services.AddSingleton<BotService>();
        services.AddSingleton<MusicPlayerService>();
        services.AddSingleton<AudioStreamService>();
        services.AddSingleton<YoutubeService>();
        services.AddSingleton<YoutubeLinkService>();

        services.AddHttpClient();

        services.AddSingleton<CommandHandler>();
        services.AddSingleton<LoggingHandler>();

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
