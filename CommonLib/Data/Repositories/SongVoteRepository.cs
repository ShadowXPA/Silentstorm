using CommonLib.Data.Databases;
using System.Data;
using System.Data.Common;

namespace CommonLib.Data.Repositories
{
    public class SongVoteRepository : IReposiroty<SongVoteId, SongVote>
    {
        private readonly SilentstormDatabase _db;

        public SongVoteRepository(SilentstormDatabase database)
        {
            _db = database;
        }

        public Task<SongVote?> FindAsync(SongVoteId id)
        {
            return FindAsync(id, false);
        }

        public async Task<SongVote?> FindAsync(SongVoteId id, bool eagerLoad)
        {
            SongVote? vote = null;
            var query = "SELECT * FROM song_vote sv";

            if (eagerLoad)
            {
                query += " INNER JOIN project p ON sv.proj_id = p.proj_id";
                query += " INNER JOIN silentstorm_user su ON sv.user_id = su.user_id";
                query += " INNER JOIN song_submission ss ON sv.subm_id = ss.subm_id";
            }

            query += " WHERE sv.proj_id = @projectId AND sv.user_id = @userId";

            using var reader = await _db.ExecuteReaderAsync(query, new()
            {
                { "projectId", id.ProjectId },
                { "userId", id.UserId }
            });

            if (reader == null || !await reader.ReadAsync()) return vote;

            vote = await ParseAsync(reader, eagerLoad);

            return vote;
        }

        public Task<List<SongVote>> GetAllAsync()
        {
            return GetAllAsync(false);
        }

        public async Task<List<SongVote>> GetAllAsync(bool eagerLoad)
        {
            var votes = new List<SongVote>();
            var query = "SELECT * FROM song_vote sv";

            if (eagerLoad)
            {
                query += " INNER JOIN project p ON sv.proj_id = p.proj_id";
                query += " INNER JOIN silentstorm_user su ON sv.user_id = su.user_id";
                query += " INNER JOIN song_submission ss ON sv.subm_id = ss.subm_id";
            }

            using var reader = await _db.ExecuteReaderAsync(query);

            if (reader == null) return votes;

            while (await reader.ReadAsync())
                votes.Add(await ParseAsync(reader, eagerLoad));

            return votes;
        }

        public Task<List<SongVote>> GetAllByProjectIdAsync(uint id)
        {
            return GetAllByProjectIdAsync(id, false);
        }

        public async Task<List<SongVote>> GetAllByProjectIdAsync(uint id, bool eagerLoad)
        {
            var votes = new List<SongVote>();
            var query = "SELECT * FROM song_vote sv";

            if (eagerLoad)
            {
                query += " INNER JOIN project p ON sv.proj_id = p.proj_id";
                query += " INNER JOIN silentstorm_user su ON sv.user_id = su.user_id";
                query += " INNER JOIN song_submission ss ON sv.subm_id = ss.subm_id";
            }

            query += " WHERE sv.proj_id = @projectId";

            using var reader = await _db.ExecuteReaderAsync(query, new()
            {
                { "projectId", id }
            });

            if (reader == null) return votes;

            while (await reader.ReadAsync())
                votes.Add(await ParseAsync(reader, eagerLoad));

            return votes;
        }

        public async Task<bool> AddAsync(SongVote row)
        {
            var result = await _db.ExecuteNonQueryAsync("INSERT INTO song_vote(proj_id, user_id, subm_id)"
                + " VALUES(@projectId, @userId, @submissionId)", new()
            {
                { "projectId", row.ProjectId },
                { "userId", row.UserId },
                { "submissionId", row.SubmissionId }
            });

            return result != 0;
        }

        public Task<bool> UpdateAsync(SongVote row)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveAsync(SongVoteId id)
        {
            var result = await _db.ExecuteNonQueryAsync("DELETE FROM song_vote WHERE proj_id = @projectId AND user_id = @userId", new()
            {
                { "projectId", id.ProjectId },
                { "userId", id.UserId}
            });

            return result != 0;
        }

        public Task<SongVote> ParseAsync(DbDataReader reader)
        {
            return ParseAsync(reader, false);
        }

        public async Task<SongVote> ParseAsync(DbDataReader reader, bool eagerLoad)
        {
            return new SongVote
            {
                ProjectId = await reader.GetFieldValueAsync<uint>("proj_id"),
                Project = eagerLoad ? await _db.Projects.ParseAsync(reader) : null,
                UserId = await reader.GetFieldValueAsync<uint>("user_id"),
                User = eagerLoad ? await _db.SilentstormUsers.ParseAsync(reader) : null,
                SubmissionId = await reader.GetFieldValueAsync<uint>("subm_id"),
                Submission = eagerLoad ? await _db.SongSubmissions.ParseAsync(reader) : null
            };
        }
    }
}
