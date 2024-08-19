using CommonLib.Data.Databases;
using CommonLib.Models;
using CommonLib.Utils;

namespace CommonLib.Services
{
    public class SilentstormUserService
    {
        private readonly SilentstormDatabase _db;

        public SilentstormUserService(SilentstormDatabase database)
        {
            _db = database;
        }

        public async Task<SilentstormUserDto?> GetSilentStormUserAsync(uint id)
        {
            return (await _db.SilentstormUsers.FindAsync(id))?.ToDto();
        }

        public async Task<SilentstormUserDto?> GetSilentStormUserByUsernameAsync(string username)
        {
            return (await _db.SilentstormUsers.FindByUsernameAsync(username))?.ToDto();
        }

        public async Task<List<SilentstormUserDto>> GetAllSilentStormUsersAsync()
        {
            return (await _db.SilentstormUsers.GetAllAsync())
                .Select(u => u.ToDto())
                .ToList();
        }

        public async Task<bool> AddSilentStormUserAsync(SilentstormUserDto silentStormUser)
        {
            return await _db.SilentstormUsers.AddAsync(silentStormUser.ToEntity());
        }

        public async Task<bool> UpdateSilentStormUserAsync(SilentstormUserDto silentStormUser)
        {
            return await _db.SilentstormUsers.UpdateAsync(silentStormUser.ToEntity());
        }

        public async Task<bool> RemoveSilentStormUserAsync(uint id)
        {
            return await _db.SilentstormUsers.RemoveAsync(id);
        }
    }
}
