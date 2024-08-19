namespace CommonLib.Models
{
    public class SongSubmissionDto
    {
        public uint? Id { get; set; }
        public ProjectDto? Project { get; set; }
        public SilentstormUserDto? User { get; set; }
        public string? Uri { get; set; }
        public string? Title { get; set; }
        public bool? IsSelectedForVoting { get; set; }
    }
}
