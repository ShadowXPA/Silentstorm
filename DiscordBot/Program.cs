using DiscordBot.Bot;

namespace DiscordBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            var configFile = args.Length > 0 ? args[0] : $@"discordbot.json";

            var silentstorm = new SilentstormBot(configFile);

            await silentstorm.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
