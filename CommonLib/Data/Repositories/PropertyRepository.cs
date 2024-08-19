using CommonLib.Data.Databases;
using CommonLib.Utils;
using System.Data;
using System.Data.Common;

namespace CommonLib.Data.Repositories
{
    public class PropertyRepository : IReposiroty<string, Property>
    {
        private readonly SilentstormDatabase _db;

        public PropertyRepository(SilentstormDatabase database)
        {
            _db = database;
        }

        public async Task<Property?> FindAsync(string id)
        {
            Property? property = null;

            using var reader = await _db.ExecuteReaderAsync("SELECT * FROM property WHERE prop_id = @id", new()
            {
                { "id", id }
            });

            if (reader == null || !await reader.ReadAsync()) return property;

            property = await ParseAsync(reader);
            return property;
        }

        public async Task<List<Property>> GetAllAsync()
        {
            var properties = new List<Property>();

            using var reader = await _db.ExecuteReaderAsync("SELECT * FROM property");

            if (reader == null) return properties;

            while (await reader.ReadAsync())
                properties.Add(await ParseAsync(reader));

            return properties;
        }

        public async Task<bool> AddAsync(Property row)
        {
            var result = await _db.ExecuteNonQueryAsync("INSERT INTO property(prop_id, value) VALUES(@id, @value)", new()
            {
                { "id", row.Id },
                { "value", row.Value }
            });

            return result != 0;
        }

        public async Task<bool> UpdateAsync(Property row)
        {
            var result = await _db.ExecuteNonQueryAsync("UPDATE property SET value = @value WHERE prop_id = @id", new()
            {
                { "id", row.Id },
                { "value", row.Value }
            });

            return result != 0;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            var result = await _db.ExecuteNonQueryAsync("DELETE FROM property WHERE prop_id = @id", new()
            {
                { "id", id }
            });

            return result != 0;
        }

        public async Task<Property> ParseAsync(DbDataReader reader)
        {
            return new Property
            {
                Id = await reader.GetFieldValueAsync<string>("prop_id"),
                Value = await reader.GetNullableFieldValueAsync<string?>("value")
            };
        }
    }
}
