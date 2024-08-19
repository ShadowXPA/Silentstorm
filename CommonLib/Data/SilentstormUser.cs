using CommonLib.Data.Databases;

namespace CommonLib.Data
{
    public class SilentstormUser : IEntity
    {
        public uint Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? DiscordId { get; set; }
    }
}
