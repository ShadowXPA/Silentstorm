using CommonLib.Data;
using CommonLib.Data.Databases;
using CommonLib.Models;
using CommonLib.Utils;

namespace CommonLib.Services
{
    public class ProjectStatusService
    {
        private readonly SilentstormDatabase _db;

        public ProjectStatusService(SilentstormDatabase database)
        {
            _db = database;
        }

        public async Task<ProjectStatusDto?> GetProjectStatusAsync(string name)
        {
            return (await _db.ProjectStatuses.FindAsync(name))?.ToDto();
        }

        public async Task<List<ProjectStatusDto>> GetAllProjectStatusesAsync()
        {
            return (await _db.ProjectStatuses.GetAllAsync())
                .Select(pt => pt.ToDto())
                .ToList();
        }

        public Task<bool> AddProjectStatusAsync(ProjectStatusDto projectStatus)
        {
            return _db.ProjectStatuses.AddAsync(projectStatus.ToEntity());
        }

        public Task<bool> UpdateProjectStatusAsync(ProjectStatusDto projectStatus)
        {
            return _db.ProjectStatuses.UpdateAsync(projectStatus.ToEntity());
        }

        public Task<bool> RemoveProjectStatusAsync(string name)
        {
            return _db.ProjectStatuses.RemoveAsync(name);
        }

        public string? FindNextStatus(string? projectStatus)
        {
            return projectStatus switch
            {
                ProjectStatus.Status.Created => ProjectStatus.Status.Submission,
                ProjectStatus.Status.Submission => ProjectStatus.Status.Voting,
                ProjectStatus.Status.Voting => ProjectStatus.Status.Developing,
                ProjectStatus.Status.Developing => ProjectStatus.Status.Finished,
                ProjectStatus.Status.Finished => ProjectStatus.Status.Finished,
                _ => projectStatus
            };
        }
    }
}
