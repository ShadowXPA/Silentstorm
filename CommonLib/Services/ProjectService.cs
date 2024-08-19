using CommonLib.Data;
using CommonLib.Data.Databases;
using CommonLib.Models;
using CommonLib.Utils;

namespace CommonLib.Services
{
    public class ProjectService
    {
        private readonly SilentstormDatabase _db;
        private readonly PropertiesService _propertiesService;

        public ProjectService(SilentstormDatabase database, PropertiesService propertiesService)
        {
            _db = database;
            _propertiesService = propertiesService;
        }

        public async Task<ProjectDto?> GetProjectAsync(uint id, bool eagerLoad = false)
        {
            return (await _db.Projects.FindAsync(id, eagerLoad))?.ToDto();
        }

        public async Task<ProjectDto?> GetProjectByUserIdAsync(uint id, uint userId, bool eagerLoad = false)
        {
            return (await _db.Projects.FindAsync(id, userId, eagerLoad))?.ToDto();
        }

        public async Task<List<ProjectDto>> GetAllProjectsAsync(bool eagerLoad = false)
        {
            return (await _db.Projects.GetAllAsync(eagerLoad))
                .Select(p => p.ToDto())
                .ToList();
        }

        public async Task<List<ProjectDto>> GetAllProjectsByUserAsync(uint userId, bool eagerLoad = false)
        {
            return (await _db.Projects.GetAllByUserIdAsync(userId, eagerLoad))
                .Select(p => p.ToDto())
                .ToList();
        }

        public async Task<List<ProjectDto>> GetAllProjectsByUserAndStatusAsync(uint userId, string status, bool eagerLoad = false)
        {
            return (await _db.Projects.GetAllByUserIdAndStatusAsync(userId, status, eagerLoad))
                .Select(p => p.ToDto())
                .ToList();
        }

        public async Task<List<ProjectDto>> GetProjectsByStatusAsync(string status, bool eagerLoad = false)
        {
            return (await _db.Projects.GetAllByStatusAsync(status, eagerLoad))
                .Select(p => p.ToDto())
                .ToList();
        }

        public Task<bool> AddProjectAsync(ProjectDto project)
        {
            project.ProjectStatus = _propertiesService.GetPropertyOrDefault("silentstorm.project.create-status", ProjectStatus.Status.Created);
            project.CreatedAt = DateTime.Now;
            project.FinishedAt = null;
            return _db.Projects.AddAsync(project.ToEntity());
        }

        public Task<bool> UpdateProjectAsync(ProjectDto project)
        {
            return _db.Projects.UpdateAsync(project.ToEntity());
        }

        public Task<bool> RemoveProjectAsync(uint id)
        {
            return _db.Projects.RemoveAsync(id);
        }

        public async Task<ProjectAnnouncementDto?> GetProjectAnnouncementAsync(uint projectId, string projectStatus, bool eagerLoad = false)
        {
            return (await _db.ProjectAnnouncements.FindAsync(new(projectId, projectStatus), eagerLoad))?.ToDto();
        }

        public async Task<List<ProjectAnnouncementDto>> GetProjectAnnouncementsAsync(uint? projectId = null, bool eagerLoad = false)
        {
            List<ProjectAnnouncement> projectAnnouncements;

            if (projectId is null)
                projectAnnouncements = await _db.ProjectAnnouncements.GetAllAsync(eagerLoad);
            else
                projectAnnouncements = await _db.ProjectAnnouncements.GetAllForProjectIdAsync((uint)projectId, eagerLoad);

            return projectAnnouncements
                .Select(pa => pa.ToDto())
                .ToList();
        }

        public async Task<List<ProjectAnnouncementDto>> GetPendingProjectAnnouncementsAsync(bool eagerLoad = false)
        {
            List<ProjectAnnouncement> projectAnnouncements = await _db.ProjectAnnouncements.GetPendingAsync(eagerLoad);
            return projectAnnouncements
                .Select(pa => pa.ToDto())
                .ToList();
        }

        public Task<bool> AddProjectAnnouncementAsync(ProjectAnnouncementDto announcement)
        {
            announcement.WasAnnounced = false;
            return _db.ProjectAnnouncements.AddAsync(announcement.ToEntity());
        }

        public Task<bool> UpdateProjectAnnouncementAsync(ProjectAnnouncementDto announcement)
        {
            return _db.ProjectAnnouncements.UpdateAsync(announcement.ToEntity());
        }

        public Task<bool> RemoveProjectAnnouncementAsync(uint projectId, string projectStatus)
        {
            return _db.ProjectAnnouncements.RemoveAsync(new(projectId, projectStatus));
        }

        public async Task<SongSubmissionDto?> GetSongSubmissionByIdAsync(uint submissionId, bool eagerLoad = false)
        {
            return (await _db.SongSubmissions.FindAsync(submissionId, eagerLoad))?.ToDto();
        }

        public async Task<List<SongSubmissionDto>> GetAllSongSubmissionsByProjectIdAsync(uint projectId, bool eagerLoad = false)
        {
            return (await _db.SongSubmissions.GetAllByProjectIdAsync(projectId, eagerLoad))
                .Select(ss => ss.ToDto())
                .ToList();
        }

        public async Task<List<SongSubmissionDto>> GetAllSongSubmissionsByProjectIdAndSelectedForVotingAsync(uint projectId, bool eagerLoad = false)
        {
            return (await _db.SongSubmissions.GetAllByProjectIdAndSelectedForVotingAsync(projectId, eagerLoad))
                .Select(ss => ss.ToDto())
                .ToList();
        }

        public async Task<bool> AddSongSubmissionAsync(SongSubmissionDto songSubmission)
        {
            var submission = songSubmission.ToEntity();
            submission.IsSelectedForVoting = false;
            var project = (await _db.Projects.FindAsync(submission.ProjectId))!;
            var submissionStatus = _propertiesService.GetPropertyOrDefault("silentstorm.project.submission-status", ProjectStatus.Status.Submission);
            if (project.ProjectStatus != submissionStatus) return false;
            return await _db.SongSubmissions.AddAsync(submission);
        }

        public Task<bool> UpdateSongSubmissionAsync(SongSubmissionDto songSubmission)
        {
            return _db.SongSubmissions.UpdateAsync(songSubmission.ToEntity());
        }

        public Task<bool> RemoveSongSubmissionAsync(uint id)
        {
            return _db.SongSubmissions.RemoveAsync(id);
        }

        public async Task<List<SongVoteDto>> GetAllSongVotesByProjectIdAsync(uint projectId, bool eagerLoad = false)
        {
            return (await _db.SongVotes.GetAllByProjectIdAsync(projectId, eagerLoad))
                .Select(sv => sv.ToDto())
                .ToList();
        }

        public async Task<bool> AddSongVoteAsync(SongVoteDto songVote)
        {
            var vote = songVote.ToEntity();
            var project = (await _db.Projects.FindAsync(vote.ProjectId))!;
            var votingStatus = _propertiesService.GetPropertyOrDefault("silentstorm.project.voting-status", ProjectStatus.Status.Voting);
            if (project.ProjectStatus != votingStatus) return false;
            var submissions = await _db.SongSubmissions.GetAllByProjectIdAndSelectedForVotingAsync(project.Id);
            var hasSubmission = submissions.Any(s => s.Id == vote.SubmissionId);
            return hasSubmission && await _db.SongVotes.AddAsync(vote);
        }

        public Task<bool> RemoveSongVoteAsync(uint projectId, uint userId)
        {
            return _db.SongVotes.RemoveAsync(new(projectId, userId));
        }

        public Task<uint?> GetLastInsertedIdAsync()
        {
            return _db.GetLastInsertedIdAsync();
        }
    }
}
