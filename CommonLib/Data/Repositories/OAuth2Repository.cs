using CommonLib.Data.Databases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Data.Repositories
{
    public class OAuth2Repository : IReposiroty<string, OAuth2>
    {
        private readonly SilentstormDatabase _db;

        public OAuth2Repository(SilentstormDatabase database)
        {
            _db = database;
        }

        public async Task<OAuth2?> FindAsync(string id)
        {
            OAuth2? oauth2 = null;

            using var reader = await _db.ExecuteReaderAsync("SELECT * FROM oauth2 WHERE access_token = @accessToken", new()
            {
                { "accessToken", id }
            });

            if (reader == null || !await reader.ReadAsync()) return oauth2;

            oauth2 = await ParseAsync(reader);
            return oauth2;
        }

        public Task<List<OAuth2>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddAsync(OAuth2 row)
        {
            var result = await _db.ExecuteNonQueryAsync("INSERT INTO oauth2(access_token, token_type, expires_in, refresh_token, scope) "
                + "VALUES(@accessToken, @tokenType, @expiresIn, @refreshToken, @scope)", new()
            {
                { "accessToken", row.AccessToken },
                { "tokenType", row.TokenType },
                { "expiresIn", row.ExpiresIn },
                { "refreshToken", row.RefreshToken },
                { "scope", row.Scope }
            });

            return result != 0;
        }

        public Task<bool> UpdateAsync(OAuth2 row)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveAsync(string id)
        {
            var result = await _db.ExecuteNonQueryAsync("DELETE FROM oauth2 WHERE access_token = @accessToken", new()
            {
                { "accessToken", id }
            });

            return result != 0;
        }

        public async Task<OAuth2> ParseAsync(DbDataReader reader)
        {
            return new OAuth2
            {
                AccessToken = await reader.GetFieldValueAsync<string>("access_token"),
                TokenType = await reader.GetFieldValueAsync<string>("token_type"),
                ExpiresIn = await reader.GetFieldValueAsync<uint>("expires_in"),
                RefreshToken = await reader.GetFieldValueAsync<string>("refresh_token"),
                Scope = await reader.GetFieldValueAsync<string>("scope")
            };
        }
    }
}
