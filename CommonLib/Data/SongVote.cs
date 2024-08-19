using CommonLib.Data.Databases;

namespace CommonLib.Data
{
    public class SongVote : IEntity
    {
        public uint ProjectId { get; set; }
        public Project? Project { get; set; }
        public uint UserId { get; set; }
        public SilentstormUser? User { get; set; }
        public uint SubmissionId { get; set; }
        public SongSubmission? Submission { get; set; }
    }
}
