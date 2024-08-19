using System.Data.Common;

namespace CommonLib.Data.Databases
{
    public interface IDatabase : IDisposable
    {
        Task InitDatabaseAsync();
        Task OpenAsync();
        Task CloseAsync();
        Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object?>? parameters = null);
        Task<DbDataReader?> ExecuteReaderAsync(string query, Dictionary<string, object?>? parameters = null);
    }
}
