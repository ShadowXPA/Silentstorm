using System.Data;
using System.Data.Common;

namespace CommonLib.Utils
{
    public static class DatabaseExtension
    {
        public static async Task<T?> GetNullableFieldValueAsync<T>(this DbDataReader reader, string column)
        {
            var value = await reader.GetFieldValueAsync<object>(column);
            return value == DBNull.Value ? default : (T)value;
        }
    }
}
