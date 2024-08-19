using System.ComponentModel.DataAnnotations;

namespace CommonLib.Models
{
    public class ProjectDto
    {
        public uint? Id { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ProjectType { get; set; }
        public string? ProjectStatus { get; set; }
        public SilentstormUserDto? User { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
    }
}
