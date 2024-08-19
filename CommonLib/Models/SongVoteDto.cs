namespace CommonLib.Models
{
    public class SongVoteDto
    {
        public ProjectDto? Project { get; set; }
        public SilentstormUserDto? User { get; set; }
        public SongSubmissionDto? SongSubmission { get; set; }
    }
}
