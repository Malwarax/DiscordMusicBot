using DiscordMusicBot.Application.Services;
using DiscordMusicBot.Domain.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.InfrastructureIoC
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<MusicBot>();
            services.AddOptions<BotOptions>().BindConfiguration(nameof(BotOptions));
        }

        public static void RegisterConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            //TODO
        }
    }
}
