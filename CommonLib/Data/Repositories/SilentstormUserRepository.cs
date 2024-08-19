using CommonLib.Data.Databases;
using CommonLib.Utils;
using System.Data;
using System.Data.Common;

namespace CommonLib.Data.Repositories
{
    public class SilentstormUserRepository : IReposiroty<uint, SilentstormUser>
    {
        private readonly SilentstormDatabase _db;

        public SilentstormUserRepository(SilentstormDatabase db)
        {
            _db = db;
        }

        public async Task<SilentstormUser?> FindAsync(uint id)
        {
            SilentstormUser? user = null;

            using var reader = await _db.ExecuteReaderAsync("SELECT * FROM silentstorm_user WHERE user_id = @id", new()
            {
                { "id", id }
            });

            if (reader == null || !await reader.ReadAsync()) return user;

            user = await ParseAsync(reader);

            return user;
        }

        public async Task<SilentstormUser?> FindByUsernameAsync(string username)
        {
            SilentstormUser? user = null;

            using var reader = await _db.ExecuteReaderAsync("SELECT * FROM silentstorm_user WHERE username = @username", new()
            {
                { "username", username }
            });

            if (reader == null || !await reader.ReadAsync()) return user;

            user = await ParseAsync(reader);

            return user;
        }

        public async Task<List<SilentstormUser>> GetAllAsync()
        {
            var users = new List<SilentstormUser>();

            using var reader = await _db.ExecuteReaderAsync("SELECT * FROM silentstorm_user");

            if (reader == null) return users;

            while (await reader.ReadAsync())
                users.Add(await ParseAsync(reader));

            return users;
        }

        public async Task<bool> AddAsync(SilentstormUser row)
        {
            var result = await _db.ExecuteNonQueryAsync("INSERT INTO silentstorm_user(username, discord_id) VALUES(@username, @discordId)", new()
            {
                { "username", row.Username },
                { "discordId", row.DiscordId }
            });

            return result != 0;
        }

        public async Task<bool> UpdateAsync(SilentstormUser row)
        {
            var result = await _db.ExecuteNonQueryAsync("UPDATE silentstorm_user "
                + "SET username = @username, discord_id = @discordId WHERE user_id = @id", new()
            {
                { "id", row.Id },
                { "username", row.Username },
                { "discordId", row.DiscordId }
            });

            return result != 0;
        }

        public async Task<bool> RemoveAsync(uint id)
        {
            var result = await _db.ExecuteNonQueryAsync("DELETE FROM silentstorm_user WHERE user_id = @id", new()
            {
                { "id", id }
            });

            return result != 0;
        }

        public async Task<SilentstormUser> ParseAsync(DbDataReader reader)
        {
            return new SilentstormUser
            {
                Id = await reader.GetFieldValueAsync<uint>("user_id"),
                Username = await reader.GetFieldValueAsync<string>("username"),
                DiscordId = await reader.GetNullableFieldValueAsync<string?>("discord_id")
            };
        }
    }
}
