using CommonLib.Data.Databases;

namespace CommonLib.Data
{
    public class SongSubmission : IEntity
    {
        public uint Id { get; set; }
        public uint ProjectId { get; set; }
        public Project? Project { get; set; }
        public uint UserId { get; set; }
        public SilentstormUser? User { get; set; }
        public string Uri { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsSelectedForVoting { get; set; }
    }
}
