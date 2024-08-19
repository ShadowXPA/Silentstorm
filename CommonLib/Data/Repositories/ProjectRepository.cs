using CommonLib.Data.Databases;
using CommonLib.Utils;
using System.Data;
using System.Data.Common;

namespace CommonLib.Data.Repositories
{
    public class ProjectRepository : IReposiroty<uint, Project>
    {
        private readonly SilentstormDatabase _db;

        public ProjectRepository(SilentstormDatabase database)
        {
            _db = database;
        }

        public Task<Project?> FindAsync(uint id)
        {
            return FindAsync(id, false);
        }

        public async Task<Project?> FindAsync(uint id, bool eagerLoad)
        {
            Project? project = null;
            var query = "SELECT * FROM project p";

            if (eagerLoad)
                query += " INNER JOIN silentstorm_user su ON p.user_id = su.user_id";

            query += " WHERE p.proj_id = @id";

            using var reader = await _db.ExecuteReaderAsync(query, new()
            {
                { "id", id }
            });

            if (reader == null || !await reader.ReadAsync()) return project;

            project = await ParseAsync(reader, eagerLoad);

            return project;
        }

        public Task<Project?> FindAsync(uint id, uint userId)
        {
            return FindAsync(id, userId, false);
        }

        public async Task<Project?> FindAsync(uint id, uint userId, bool eagerLoad)
        {
            Project? project = null;
            var query = "SELECT * FROM project p";

            if (eagerLoad)
                query += " INNER JOIN silentstorm_user su ON p.user_id = su.user_id";

            query += " WHERE p.proj_id = @id AND p.user_id = @userId";

            using var reader = await _db.ExecuteReaderAsync(query, new()
            {
                { "id", id },
                { "userId", userId }
            });

            if (reader == null || !await reader.ReadAsync()) return project;

            project = await ParseAsync(reader, eagerLoad);

            return project;
        }

        public Task<List<Project>> GetAllAsync()
        {
            return GetAllAsync(false);
        }

        public async Task<List<Project>> GetAllAsync(bool eagerLoad)
        {
            var projects = new List<Project>();
            var query = "SELECT * FROM project p";

            if (eagerLoad)
                query += " INNER JOIN silentstorm_user su ON p.user_id = su.user_id";

            query += " ORDER BY created_at DESC";

            using var reader = await _db.ExecuteReaderAsync(query);

            if (reader == null) return projects;

            while (await reader.ReadAsync())
                projects.Add(await ParseAsync(reader, eagerLoad));

            return projects;
        }

        public Task<List<Project>> GetAllByUserIdAsync(uint userId)
        {
            return GetAllByUserIdAsync(userId, false);
        }

        public async Task<List<Project>> GetAllByUserIdAsync(uint userId, bool eagerLoad)
        {
            var projects = new List<Project>();
            var query = "SELECT * FROM project p";

            if (eagerLoad)
                query += " INNER JOIN silentstorm_user su ON p.user_id = su.user_id";

            query += " WHERE p.user_id = @userId ORDER BY created_at DESC";

            using var reader = await _db.ExecuteReaderAsync(query, new()
            {
                { "userId", userId }
            });

            if (reader == null) return projects;

            while (await reader.ReadAsync())
                projects.Add(await ParseAsync(reader, eagerLoad));

            return projects;
        }

        public Task<List<Project>> GetAllByUserIdAndStatusAsync(uint userId, string status)
        {
            return GetAllByUserIdAndStatusAsync(userId, status, false);
        }

        public async Task<List<Project>> GetAllByUserIdAndStatusAsync(uint userId, string status, bool eagerLoad)
        {
            var projects = new List<Project>();
            var query = "SELECT * FROM project p";

            if (eagerLoad)
                query += " INNER JOIN silentstorm_user su ON p.user_id = su.user_id";

            query += " WHERE p.user_id = @userId AND p.proj_status = @status ORDER BY created_at DESC";

            using var reader = await _db.ExecuteReaderAsync(query, new()
            {
                { "userId", userId },
                { "status", status }
            });

            if (reader == null) return projects;

            while (await reader.ReadAsync())
                projects.Add(await ParseAsync(reader, eagerLoad));

            return projects;
        }

        public Task<List<Project>> GetAllByStatusAsync(string status)
        {
            return GetAllByStatusAsync(status, false);
        }

        public async Task<List<Project>> GetAllByStatusAsync(string status, bool eagerLoad)
        {
            var projects = new List<Project>();
            var query = "SELECT * FROM project p";

            if (eagerLoad)
                query += " INNER JOIN silentstorm_user su ON p.user_id = su.user_id";

            query += " WHERE p.proj_status = @status ORDER BY created_at DESC";

            using var reader = await _db.ExecuteReaderAsync(query, new()
            {
                { "status", status }
            });

            if (reader == null) return projects;

            while (await reader.ReadAsync())
                projects.Add(await ParseAsync(reader, eagerLoad));

            return projects;
        }

        public async Task<bool> AddAsync(Project row)
        {
            var result = await _db.ExecuteNonQueryAsync("INSERT INTO project(title, description, proj_type, proj_status, user_id, created_at, finished_at)"
                + " VALUES(@title, @description, @projectType, @projectStatus, @userId, @createdAt, @finishedAt)", new()
            {
                { "title", row.Title },
                { "description", row.Description },
                { "projectType", row.ProjectType },
                { "projectStatus", row.ProjectStatus },
                { "userId", row.UserId },
                { "createdAt", row.CreatedAt },
                { "finishedAt", row.FinishedAt }
            });

            return result != 0;
        }

        public async Task<bool> UpdateAsync(Project row)
        {
            var result = await _db.ExecuteNonQueryAsync("UPDATE project "
                + "SET title = @title,"
                + " description = @description,"
                + " proj_type = @projectType,"
                + " proj_status = @projectStatus,"
                + " user_id = @userId,"
                + " created_at = @createdAt,"
                + " finished_at = @finishedAt"
                + " WHERE proj_id = @id", new()
            {
                { "id", row.Id },
                { "title", row.Title },
                { "description", row.Description },
                { "projectType", row.ProjectType },
                { "projectStatus", row.ProjectStatus },
                { "userId", row.UserId },
                { "createdAt", row.CreatedAt },
                { "finishedAt", row.FinishedAt }
            });

            return result != 0;
        }

        public async Task<bool> RemoveAsync(uint id)
        {
            var result = await _db.ExecuteNonQueryAsync("DELETE FROM project WHERE proj_id = @id", new()
            {
                { "id", id }
            });

            return result != 0;
        }

        public Task<Project> ParseAsync(DbDataReader reader)
        {
            return ParseAsync(reader, false);
        }

        public async Task<Project> ParseAsync(DbDataReader reader, bool eagerLoad)
        {
            return new Project
            {
                Id = await reader.GetFieldValueAsync<uint>("proj_id"),
                Title = await reader.GetFieldValueAsync<string>("title"),
                Description = await reader.GetNullableFieldValueAsync<string?>("description"),
                ProjectType = await reader.GetNullableFieldValueAsync<string?>("proj_type"),
                ProjectStatus = await reader.GetNullableFieldValueAsync<string?>("proj_status"),
                UserId = await reader.GetFieldValueAsync<uint>("user_id"),
                User = eagerLoad ? await _db.SilentstormUsers.ParseAsync(reader) : null,
                CreatedAt = await reader.GetFieldValueAsync<DateTime>("created_at"),
                FinishedAt = await reader.GetNullableFieldValueAsync<DateTime?>("finished_at")
            };
        }
    }
}
