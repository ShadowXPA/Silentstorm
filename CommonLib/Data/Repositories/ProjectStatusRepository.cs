using CommonLib.Data.Databases;
using CommonLib.Utils;
using System.Data;
using System.Data.Common;

namespace CommonLib.Data.Repositories
{
    public class ProjectStatusRepository : IReposiroty<string, ProjectStatus>
    {
        private readonly SilentstormDatabase _db;

        public ProjectStatusRepository(SilentstormDatabase db)
        {
            _db = db;
        }

        public async Task<ProjectStatus?> FindAsync(string id)
        {
            ProjectStatus? projectStatus = null;

            using var reader = await _db.ExecuteReaderAsync("SELECT * FROM project_status WHERE proj_status = @name", new()
            {
                { "name", id }
            });

            if (reader == null || !await reader.ReadAsync()) return projectStatus;

            projectStatus = await ParseAsync(reader);

            return projectStatus;
        }

        public async Task<List<ProjectStatus>> GetAllAsync()
        {
            var projectStatus = new List<ProjectStatus>();

            using var reader = await _db.ExecuteReaderAsync("SELECT * FROM project_status");

            if (reader == null) return projectStatus;

            while (await reader.ReadAsync())
                projectStatus.Add(await ParseAsync(reader));

            return projectStatus;
        }

        public async Task<bool> AddAsync(ProjectStatus row)
        {
            var result = await _db.ExecuteNonQueryAsync("INSERT INTO project_status(proj_status, description) VALUES(@name, @description)", new()
            {
                { "name", row.Name },
                { "description", row.Description }
            });

            return result != 0;
        }

        public async Task<bool> UpdateAsync(ProjectStatus row)
        {
            var result = await _db.ExecuteNonQueryAsync("UPDATE project_status "
                + "SET description = @description WHERE proj_status = @name", new()
            {
                { "name", row.Name },
                { "description", row.Description}
            });

            return result != 0;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            var result = await _db.ExecuteNonQueryAsync("DELETE FROM project_status WHERE proj_status = @name", new()
            {
                { "name", id }
            });

            return result != 0;
        }

        public async Task<ProjectStatus> ParseAsync(DbDataReader reader)
        {
            return new ProjectStatus
            {
                Name = await reader.GetFieldValueAsync<string>("proj_status"),
                Description = await reader.GetNullableFieldValueAsync<string?>("description")
            };
        }
    }
}
