using CommonLib.Data.Databases;
using System.Data.Common;

namespace CommonLib.Data.Repositories
{
    public interface IReposiroty<ID, T> where T : IEntity
    {
        Task<T?> FindAsync(ID id);
        Task<List<T>> GetAllAsync();
        Task<bool> AddAsync(T row);
        Task<bool> UpdateAsync(T row);
        Task<bool> RemoveAsync(ID id);
        Task<T> ParseAsync(DbDataReader reader);
    }
}
