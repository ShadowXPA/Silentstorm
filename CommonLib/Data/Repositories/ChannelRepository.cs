using CommonLib.Data.Databases;
using System.Data;
using System.Data.Common;

namespace CommonLib.Data.Repositories
{
    public class ChannelRepository : IReposiroty<string, Channel>
    {
        private readonly SilentstormDatabase _db;

        public ChannelRepository(SilentstormDatabase database)
        {
            _db = database;
        }

        public async Task<Channel?> FindAsync(string id)
        {
            Channel? channel = null;

            using var reader = await _db.ExecuteReaderAsync("SELECT * FROM channel WHERE channel_id = @id", new()
            {
                { "id", id }
            });

            if (reader == null || !await reader.ReadAsync()) return channel;

            channel = await ParseAsync(reader);

            return channel;
        }

        public async Task<List<Channel>> GetAllAsync()
        {
            var channels = new List<Channel>();

            using var reader = await _db.ExecuteReaderAsync("SELECT * FROM channel");

            if (reader == null) return channels;

            while (await reader.ReadAsync())
                channels.Add(await ParseAsync(reader));

            return channels;
        }

        public async Task<bool> AddAsync(Channel row)
        {
            var result = await _db.ExecuteNonQueryAsync("INSERT INTO channel(channel_id, guild_id, name) VALUES(@id, @guildId, @name)", new()
            {
                { "id", row.Id },
                { "guildId", row.GuildId },
                { "name", row.Name }
            });

            return result != 0;
        }

        public Task<bool> UpdateAsync(Channel row)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveAsync(string id)
        {
            var result = await _db.ExecuteNonQueryAsync("DELETE FROM channel WHERE channel_id = @id", new()
            {
                { "id", id }
            });

            return result != 0;
        }

        public async Task<Channel> ParseAsync(DbDataReader reader)
        {
            return new Channel
            {
                Id = await reader.GetFieldValueAsync<string>("channel_id"),
                GuildId = await reader.GetFieldValueAsync<string>("guild_id"),
                Name = await reader.GetFieldValueAsync<string>("name")
            };
        }
    }
}
