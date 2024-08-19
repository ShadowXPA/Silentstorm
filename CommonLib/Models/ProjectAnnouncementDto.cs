using CommonLib.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CommonLib.Models
{
    public class ProjectAnnouncementDto
    {
        [Required]
        public uint? ProjectId { get; set; }
        [Required]
        public string? ProjectStatus { get; set; }
        [Required]
        public ChannelDto? Channel { get; set; }
        [Required]
        public string? Announcement { get; set; }
        [Required]
        [ValidateDate]
        public DateTime? AnnouncementDate { get; set; }
        public bool? WasAnnounced { get; set; }
    }
}
