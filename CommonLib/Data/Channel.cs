using CommonLib.Data.Databases;

namespace CommonLib.Data
{
    public class Channel : IEntity
    {
        public string Id { get; set; } = string.Empty;
        public string GuildId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
