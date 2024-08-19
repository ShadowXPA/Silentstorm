using CommonLib.Data.Databases;

namespace CommonLib.Data
{
    public class ProjectType : IEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
