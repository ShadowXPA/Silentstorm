using CommonLib.Configurations;
using MySqlConnector;
using System.Data.Common;
using System.Data;

namespace CommonLib.Data.Databases
{
    public class MySqlDatabase : Database
    {
        private DatabaseConfiguration _dbConfig;

        public MySqlDatabase(DatabaseConfiguration dbConfig)
            : base(new MySqlConnection($@"Server={dbConfig.Hostname};Username={dbConfig.Username};Database={dbConfig.Database};Port={dbConfig.Port};Password={dbConfig.Password}"))
        {
            _dbConfig = dbConfig;
        }

        public override async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object?>? parameters = null)
        {
            using var command = await GetMySqlCommandAsync(query, parameters);

            return await ExecuteNonQueryAsync(command);
        }

        public override async Task<DbDataReader?> ExecuteReaderAsync(string query, Dictionary<string, object?>? parameters = null)
        {
            using var command = await GetMySqlCommandAsync(query, parameters);

            return await ExecuteReaderAsync(command);
        }

        private async Task<MySqlCommand?> GetMySqlCommandAsync(string query, Dictionary<string, object?>? parameters)
        {
            if (_dbConnection == null) return null;
            if (_dbConnection.State == ConnectionState.Closed || !await ((MySqlConnection)_dbConnection).PingAsync()) await OpenAsync();

            var command = new MySqlCommand(query, (MySqlConnection)_dbConnection);
            if (parameters != null)
                foreach (var param in parameters)
                    command.Parameters.AddWithValue(param.Key, param.Value);

            return command;
        }
    }
}
