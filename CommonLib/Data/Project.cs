using CommonLib.Data.Databases;

namespace CommonLib.Data
{
    public class Project : IEntity
    {
        public uint Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ProjectType { get; set; }
        public string? ProjectStatus { get; set; }
        public uint UserId { get; set; }
        public SilentstormUser? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? FinishedAt { get; set; }
    }
}
