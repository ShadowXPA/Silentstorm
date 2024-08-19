using Serilog;
using System.Data.Common;

namespace CommonLib.Data.Databases
{
    public abstract class Database : IDatabase
    {
        protected DbConnection _dbConnection;

        public Database(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task InitDatabaseAsync()
        {
            await OpenAsync();
        }

        public async Task OpenAsync()
        {
            await _dbConnection.OpenAsync();
        }

        public async Task CloseAsync()
        {
            await _dbConnection.CloseAsync();
        }

        public abstract Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object?>? parameters = null);

        public abstract Task<DbDataReader?> ExecuteReaderAsync(string query, Dictionary<string, object?>? parameters = null);

        protected async Task<int> ExecuteNonQueryAsync(DbCommand? command)
        {
            if (command == null) return 0;

            try
            {
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occured while executing Non Query");
                return 0;
            }
        }

        protected async Task<DbDataReader?> ExecuteReaderAsync(DbCommand? command)
        {
            if (command == null) return null;

            try
            {
                return await command.ExecuteReaderAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occured while executing Reader");
                return null;
            }
        }

        public void Dispose()
        {
            CloseAsync().GetAwaiter().GetResult();
            _dbConnection.Dispose();
        }
    }
}
