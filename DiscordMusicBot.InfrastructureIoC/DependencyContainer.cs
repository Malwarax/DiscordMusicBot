using DiscordMusicBot.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordMusicBot.InfrastructureIoC
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<MusicBot>();
        }

        public static void RegisterConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            //TODO
        }
    }
}
