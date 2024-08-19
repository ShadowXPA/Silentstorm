namespace DiscordBot.Bot
{
    public interface IBot
    {
        Task ConnectAsync();

        Task LoadConfigAsync(string configFile);

        Task ReconnectAsync();

        Task DisconnectAsync();
    }
}
