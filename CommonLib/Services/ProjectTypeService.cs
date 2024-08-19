using CommonLib.Data.Databases;
using CommonLib.Models;
using CommonLib.Utils;

namespace CommonLib.Services
{
    public class ProjectTypeService
    {
        private readonly SilentstormDatabase _db;

        public ProjectTypeService(SilentstormDatabase db)
        {
            _db = db;
        }

        public async Task<ProjectTypeDto?> GetProjectTypeAsync(string name)
        {
            return (await _db.ProjectTypes.FindAsync(name))?.ToDto();
        }

        public async Task<List<ProjectTypeDto>> GetAllProjectTypesAsync()
        {
            return (await _db.ProjectTypes.GetAllAsync())
                .Select(pt => pt.ToDto())
                .ToList();
        }

        public async Task<bool> AddProjectTypeAsync(ProjectTypeDto projectType)
        {
            return await _db.ProjectTypes.AddAsync(projectType.ToEntity());
        }

        public async Task<bool> UpdateProjectTypeAsync(ProjectTypeDto projectType)
        {
            return await _db.ProjectTypes.UpdateAsync(projectType.ToEntity());
        }

        public async Task<bool> RemoveProjectTypeAsync(string name)
        {
            return await _db.ProjectTypes.RemoveAsync(name);
        }
    }
}
