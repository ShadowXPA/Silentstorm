using CommonLib.Configurations;
using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.Lavalink;

namespace DiscordBot.Configurations
{
    public class SilentstormBotConfiguration
    {
        public DiscordConfiguration? DiscordConfiguration { get; set; }
        public DiscordActivity DiscordActivity { get; set; } = new DiscordActivity("Your beautiful MEPs", ActivityType.Watching);
        public DatabaseConfiguration? DatabaseConfiguration { get; set; }
        public LavalinkConfiguration? LavalinkConfiguration { get; set; }
    }
}
