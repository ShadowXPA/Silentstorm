using CommonLib.Data.Databases;

namespace CommonLib.Data
{
    public class ProjectAnnouncement : IEntity
    {
        public ProjectAnnouncementId Id { get; set; } = new ProjectAnnouncementId(default, string.Empty);
        public string ChannelId { get; set; } = string.Empty;
        public Channel? Channel { get; set; }
        public string? Announcement { get; set; }
        public DateTime? AnnouncementDate { get; set; }
        public bool WasAnnounced { get; set; }
    }
}
