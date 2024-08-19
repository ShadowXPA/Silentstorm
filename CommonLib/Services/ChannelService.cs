using CommonLib.Data.Databases;
using CommonLib.Models;
using CommonLib.Utils;

namespace CommonLib.Services
{
    public class ChannelService
    {
        private readonly SilentstormDatabase _db;

        public ChannelService(SilentstormDatabase database)
        {
            _db = database;
        }

        public async Task<ChannelDto?> GetChannelAsync(string id)
        {
            return (await _db.Channels.FindAsync(id))?.ToDto();
        }

        public async Task<List<ChannelDto>> GetAllChannelsAsync()
        {
            return (await _db.Channels.GetAllAsync())
                .Select(c => c.ToDto())
                .ToList();
        }

        public async Task<bool> AddChannelAsync(ChannelDto channel)
        {
            return await _db.Channels.AddAsync(channel.ToEntity());
        }

        public async Task<bool> RemoveChannelAsync(string id)
        {
            return await _db.Channels.RemoveAsync(id);
        }
    }
}
