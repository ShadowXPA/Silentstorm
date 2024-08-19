using CommonLib.Utils;
using DSharpPlus;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using Serilog;

namespace DiscordBot.Bot
{
    public class Bot : IBot
    {
        public DiscordClient Client { get; private set; }
        public DiscordConfiguration Configuration { get; private set; }
        public string Name { get; private set; }

        public Bot(DiscordConfiguration? botConfiguration, string name = "NoName")
        {
            Name = name;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.Console(outputTemplate: $@"{{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}} [{{Level}}] {{Message:lj}}{{NewLine}}{{Exception}}")
                .WriteTo.File($@"logs{Path.DirectorySeparatorChar}DiscordBot_{name}_.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var logFactory = new LoggerFactory().AddSerilog();

            Configuration = botConfiguration ?? new();
            Configuration.LoggerFactory = logFactory;
            Client = new(Configuration);
        }

        ~Bot()
        {
            Log.Information($@"({Name}) Closing");
            Log.CloseAndFlush();
        }

        public virtual async Task ConnectAsync()
        {
            await Client.ConnectAsync();
            Log.Information($@"({Name}) Bot connected");
        }

        public virtual async Task LoadConfigAsync(string configFile)
        {
            Configuration = ConfigurationFileUtils.LoadConfiguration<DiscordConfiguration>(configFile);
            Log.Information($@"({Name}) Loaded configuration");
            Log.Information($@"({Name}) Reconnecting...");
            await ReconnectAsync();
            Log.Information($@"({Name}) Bot reconnected");
        }

        public virtual async Task ReconnectAsync()
        {
            await DisconnectAsync();
            Client = new(Configuration);
            await ConnectAsync();
        }

        public virtual async Task DisconnectAsync()
        {
            await Client.DisconnectAsync();
            Log.Information($@"({Name}) Bot disconnected");
        }
    }
}
