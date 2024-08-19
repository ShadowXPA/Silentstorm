using CommonLib.Data;
using CommonLib.Models;

namespace CommonLib.Utils
{
    public static class DtoExtension
    {
        public static ChannelDto ToDto(this Channel channel) => new()
        {
            ChannelId = channel.Id,
            GuildId = channel.GuildId,
            ChannelName = channel.Name
        };

        public static Channel ToEntity(this ChannelDto channel) => new()
        {
            Id = channel.ChannelId!,
            GuildId = channel.GuildId!,
            Name = channel.ChannelName!
        };

        public static ProjectDto ToDto(this Project project) => new()
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            ProjectType = project.ProjectType,
            ProjectStatus = project.ProjectStatus,
            User = new()
            {
                Id = project.UserId,
                Username = project.User?.Username,
                DiscordId = project.User?.DiscordId
            },
            CreatedAt = project.CreatedAt,
            FinishedAt = project.FinishedAt
        };

        public static Project ToEntity(this ProjectDto project) => new()
        {
            Id = project.Id ?? 0,
            Title = project.Title!,
            Description = project.Description,
            ProjectType = project.ProjectType,
            ProjectStatus = project.ProjectStatus,
            UserId = (uint)project.User!.Id!,
            CreatedAt = (DateTime)project.CreatedAt!,
            FinishedAt = project.FinishedAt
        };

        public static SilentstormUserDto ToDto(this SilentstormUser user) => new()
        {
            Id = user.Id,
            Username = user.Username,
            DiscordId = user.DiscordId
        };

        public static SilentstormUser ToEntity(this SilentstormUserDto userDto) => new()
        {
            Id = userDto.Id ?? 0,
            Username = userDto.Username!,
            DiscordId = userDto.DiscordId
        };

        public static SongSubmissionDto ToDto(this SongSubmission songSubmission) => new()
        {
            Id = songSubmission.Id,
            Project = new()
            {
                Id = songSubmission.ProjectId,
                Title = songSubmission.Project?.Title,
                Description = songSubmission.Project?.Description,
                ProjectType = songSubmission.Project?.ProjectType,
                ProjectStatus = songSubmission.Project?.ProjectStatus,
                CreatedAt = songSubmission.Project?.CreatedAt,
                FinishedAt = songSubmission.Project?.FinishedAt
            },
            User = new()
            {
                Id = songSubmission.UserId,
                Username = songSubmission.User?.Username,
                DiscordId = songSubmission.User?.DiscordId
            },
            Uri = songSubmission.Uri,
            Title = songSubmission.Title,
            IsSelectedForVoting = songSubmission.IsSelectedForVoting
        };

        public static SongSubmission ToEntity(this SongSubmissionDto songSubmission) => new()
        {
            Id = songSubmission.Id ?? 0,
            ProjectId = (uint)songSubmission.Project!.Id!,
            UserId = (uint)songSubmission.User!.Id!,
            Uri = songSubmission.Uri!,
            Title = songSubmission.Title!,
            IsSelectedForVoting = songSubmission.IsSelectedForVoting ?? false
        };

        public static ProjectAnnouncementDto ToDto(this ProjectAnnouncement announcement) => new()
        {
            ProjectId = announcement.Id.ProjectId,
            ProjectStatus = announcement.Id.ProjectStatus,
            Channel = new()
            {
                ChannelId = announcement.ChannelId,
                GuildId = announcement.Channel?.GuildId
            },
            Announcement = announcement.Announcement,
            AnnouncementDate = announcement.AnnouncementDate,
            WasAnnounced = announcement.WasAnnounced
        };

        public static ProjectAnnouncement ToEntity(this ProjectAnnouncementDto announcement) => new()
        {
            Id = new ProjectAnnouncementId((uint)announcement.ProjectId!, announcement.ProjectStatus!),
            ChannelId = announcement.Channel!.ChannelId!,
            Announcement = announcement.Announcement,
            AnnouncementDate = announcement.AnnouncementDate,
            WasAnnounced = announcement.WasAnnounced ?? false
        };

        public static SongVoteDto ToDto(this SongVote songVote) => new()
        {
            Project = new()
            {
                Id = songVote.ProjectId,
                Title = songVote.Project?.Title,
                Description = songVote.Project?.Description,
                ProjectType = songVote.Project?.ProjectType,
                ProjectStatus = songVote.Project?.ProjectStatus,
                CreatedAt = songVote.Project?.CreatedAt,
                FinishedAt = songVote.Project?.FinishedAt
            },
            User = new()
            {
                Id = songVote.UserId,
                Username = songVote.User?.Username,
                DiscordId = songVote.User?.DiscordId
            },
            SongSubmission = new()
            {
                Id = songVote.SubmissionId,
                User = songVote.User?.ToDto(),
                Uri = songVote.Submission?.Uri,
                Title = songVote.Submission?.Title
            }
        };

        public static SongVote ToEntity(this SongVoteDto songVote) => new()
        {
            ProjectId = (uint)songVote.Project!.Id!,
            UserId = (uint)songVote.User!.Id!,
            SubmissionId = (uint)songVote.SongSubmission!.Id!
        };

        public static ProjectTypeDto ToDto(this ProjectType projectType) => new()
        {
            Name = projectType.Name,
            Description = projectType.Description
        };

        public static ProjectType ToEntity(this ProjectTypeDto projectType) => new()
        {
            Name = projectType.Name!,
            Description = projectType.Description
        };

        public static ProjectStatusDto ToDto(this ProjectStatus projectStatus) => new()
        {
            Name = projectStatus.Name,
            Description = projectStatus.Description
        };

        public static ProjectStatus ToEntity(this ProjectStatusDto projectStatus) => new()
        {
            Name = projectStatus.Name!,
            Description = projectStatus.Description
        };
    }
}
