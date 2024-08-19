using CommonLib.Data.Databases;

namespace CommonLib.Data
{
    public class Property : IEntity
    {
        public string Id { get; set; } = string.Empty;
        public string? Value { get; set; }
    }
}
