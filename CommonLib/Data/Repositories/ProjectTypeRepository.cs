using CommonLib.Data.Databases;
using CommonLib.Utils;
using System.Data;
using System.Data.Common;

namespace CommonLib.Data.Repositories
{
    public class ProjectTypeRepository : IReposiroty<string, ProjectType>
    {
        private readonly SilentstormDatabase _db;

        public ProjectTypeRepository(SilentstormDatabase database)
        {
            _db = database;
        }

        public async Task<ProjectType?> FindAsync(string id)
        {
            ProjectType? projectType = null;

            using var reader = await _db.ExecuteReaderAsync("SELECT * FROM project_type WHERE proj_type = @name", new()
            {
                { "name", id }
            });

            if (reader == null || !await reader.ReadAsync()) return projectType;

            projectType = await ParseAsync(reader);

            return projectType;
        }

        public async Task<List<ProjectType>> GetAllAsync()
        {
            var projectTypes = new List<ProjectType>();

            using var reader = await _db.ExecuteReaderAsync("SELECT * FROM project_type");

            if (reader == null) return projectTypes;

            while (await reader.ReadAsync())
                projectTypes.Add(await ParseAsync(reader));

            return projectTypes;
        }

        public async Task<bool> AddAsync(ProjectType row)
        {
            var result = await _db.ExecuteNonQueryAsync("INSERT INTO project_type(proj_type, description) VALUES(@name, @description)", new()
            {
                { "name", row.Name },
                { "description", row.Description }
            });

            return result != 0;
        }

        public async Task<bool> UpdateAsync(ProjectType row)
        {
            var result = await _db.ExecuteNonQueryAsync("UPDATE project_type "
                + "SET description = @description WHERE proj_type = @name", new()
            {
                { "name", row.Name },
                { "description", row.Description}
            });

            return result != 0;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            var result = await _db.ExecuteNonQueryAsync("DELETE FROM project_type WHERE proj_type = @name", new()
            {
                { "name", id }
            });

            return result != 0;
        }

        public async Task<ProjectType> ParseAsync(DbDataReader reader)
        {
            return new ProjectType
            {
                Name = await reader.GetFieldValueAsync<string>("proj_type"),
                Description = await reader.GetNullableFieldValueAsync<string?>("description")
            };
        }
    }
}
