using Backoffice.Services;
using CommonLib.Configurations;
using CommonLib.Data.Databases;
using CommonLib.Services;
using CommonLib.Utils;

namespace Backoffice.Utils
{
    public static class SilentstormExtension
    {
        public static IServiceCollection AddSilentStormServices(this IServiceCollection services, string configName = $@"backoffice.json")
        {
            var dbConfig = ConfigurationFileUtils.LoadConfiguration<DatabaseConfiguration>(configName);
            services.AddSingleton(dbConfig);
            services.AddSingleton<SilentstormDatabase>();
            services.AddSingleton<PropertiesService>();
            services.AddSingleton<OAuth2Service>();
            services.AddSingleton<SilentstormUserService>();
            services.AddSingleton<ChannelService>();
            services.AddSingleton<ProjectTypeService>();
            services.AddSingleton<ProjectStatusService>();
            services.AddSingleton<ProjectService>();

            return services;
        }
    }
}
