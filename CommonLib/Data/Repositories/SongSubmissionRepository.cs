using CommonLib.Data.Databases;
using System.Data;
using System.Data.Common;

namespace CommonLib.Data.Repositories
{
    public class SongSubmissionRepository : IReposiroty<uint, SongSubmission>
    {
        private readonly SilentstormDatabase _db;

        public SongSubmissionRepository(SilentstormDatabase database)
        {
            _db = database;
        }

        public Task<SongSubmission?> FindAsync(uint id)
        {
            return FindAsync(id, false);
        }

        public async Task<SongSubmission?> FindAsync(uint id, bool eagerLoad)
        {
            SongSubmission? submission = null;
            var query = "SELECT * FROM song_submission ss";

            if (eagerLoad)
            {
                query += " INNER JOIN project p ON ss.proj_id = p.proj_id";
                query += " INNER JOIN silentstorm_user su ON ss.user_id = su.user_id";
            }

            query += " WHERE ss.subm_id = @id";

            using var reader = await _db.ExecuteReaderAsync(query, new()
            {
                { "id", id }
            });

            if (reader == null || !await reader.ReadAsync()) return submission;

            submission = await ParseAsync(reader, eagerLoad);

            return submission;
        }

        public Task<List<SongSubmission>> GetAllAsync()
        {
            return GetAllAsync(false);
        }

        public async Task<List<SongSubmission>> GetAllAsync(bool eagerLoad)
        {
            var submissions = new List<SongSubmission>();
            var query = "SELECT * FROM song_submission ss";

            if (eagerLoad)
            {
                query += " INNER JOIN project p ON ss.proj_id = p.proj_id";
                query += " INNER JOIN silentstorm_user su ON ss.user_id = su.user_id";
            }

            using var reader = await _db.ExecuteReaderAsync(query);

            if (reader == null) return submissions;

            while (await reader.ReadAsync())
                submissions.Add(await ParseAsync(reader, eagerLoad));

            return submissions;
        }

        public Task<List<SongSubmission>> GetAllByProjectIdAsync(uint projectId)
        {
            return GetAllByProjectIdAsync(projectId, false);
        }

        public async Task<List<SongSubmission>> GetAllByProjectIdAsync(uint projectId, bool eagerLoad)
        {
            var submissions = new List<SongSubmission>();
            var query = "SELECT * FROM song_submission ss";

            if (eagerLoad)
            {
                query += " INNER JOIN project p ON ss.proj_id = p.proj_id";
                query += " INNER JOIN silentstorm_user su ON ss.user_id = su.user_id";
            }

            query += " WHERE ss.proj_id = @id";

            using var reader = await _db.ExecuteReaderAsync(query, new()
            {
                { "id", projectId }
            });

            if (reader == null) return submissions;

            while (await reader.ReadAsync())
                submissions.Add(await ParseAsync(reader, eagerLoad));

            return submissions;
        }

        public Task<List<SongSubmission>> GetAllByProjectIdAndSelectedForVotingAsync(uint projectId)
        {
            return GetAllByProjectIdAndSelectedForVotingAsync(projectId, false);
        }

        public async Task<List<SongSubmission>> GetAllByProjectIdAndSelectedForVotingAsync(uint projectId, bool eagerLoad)
        {
            var submissions = new List<SongSubmission>();
            var query = "SELECT * FROM song_submission ss";

            if (eagerLoad)
            {
                query += " INNER JOIN project p ON ss.proj_id = p.proj_id";
                query += " INNER JOIN silentstorm_user su ON ss.user_id = su.user_id";
            }

            query += " WHERE ss.proj_id = @id AND ss.is_selected_for_voting = @isSelectedForVoting";

            using var reader = await _db.ExecuteReaderAsync(query, new()
            {
                { "id", projectId },
                { "isSelectedForVoting", 1 }
            });

            if (reader == null) return submissions;

            while (await reader.ReadAsync())
                submissions.Add(await ParseAsync(reader, eagerLoad));

            return submissions;
        }

        public async Task<bool> AddAsync(SongSubmission row)
        {
            var result = await _db.ExecuteNonQueryAsync("INSERT INTO song_submission(proj_id, user_id, uri, title, is_selected_for_voting)"
                + " VALUES(@projectId, @userId, @uri, @title, @isSelectedForVoting)", new()
            {
                { "projectId", row.ProjectId },
                { "userId", row.UserId },
                { "uri", row.Uri },
                { "title", row.Title },
                { "isSelectedForVoting", row.IsSelectedForVoting }
            });

            return result != 0;
        }

        public async Task<bool> UpdateAsync(SongSubmission row)
        {
            var result = await _db.ExecuteNonQueryAsync("UPDATE song_submission "
                + "SET uri = @uri, title = @title, is_selected_for_voting = @isSelectedForVoting WHERE subm_id = @id", new()
            {
                { "id", row.Id },
                { "uri", row.Uri },
                { "title", row.Title },
                { "isSelectedForVoting", row.IsSelectedForVoting }
            });

            return result != 0;
        }

        public async Task<bool> RemoveAsync(uint id)
        {
            var result = await _db.ExecuteNonQueryAsync("DELETE FROM song_submission WHERE subm_id = @id", new()
            {
                { "id", id }
            });

            return result != 0;
        }

        public Task<SongSubmission> ParseAsync(DbDataReader reader)
        {
            return ParseAsync(reader, false);
        }

        public async Task<SongSubmission> ParseAsync(DbDataReader reader, bool eagerLoad)
        {
            return new SongSubmission
            {
                Id = await reader.GetFieldValueAsync<uint>("subm_id"),
                ProjectId = await reader.GetFieldValueAsync<uint>("proj_id"),
                Project = eagerLoad ? await _db.Projects.ParseAsync(reader) : null,
                UserId = await reader.GetFieldValueAsync<uint>("user_id"),
                User = eagerLoad ? await _db.SilentstormUsers.ParseAsync(reader) : null,
                Uri = await reader.GetFieldValueAsync<string>("uri"),
                Title = await reader.GetFieldValueAsync<string>("title"),
                IsSelectedForVoting = await reader.GetFieldValueAsync<bool>("is_selected_for_voting")
            };
        }
    }
}
