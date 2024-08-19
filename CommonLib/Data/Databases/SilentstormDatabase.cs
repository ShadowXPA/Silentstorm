using CommonLib.Configurations;
using CommonLib.Data.Repositories;
using System.Data;

namespace CommonLib.Data.Databases
{
    public partial class SilentstormDatabase : MySqlDatabase
    {
        public OAuth2Repository OAuth2 { get; }
        public PropertyRepository Properties { get; }
        public SilentstormUserRepository SilentstormUsers { get; }
        public ChannelRepository Channels { get; }
        public ProjectTypeRepository ProjectTypes { get; }
        public ProjectStatusRepository ProjectStatuses { get; }
        public ProjectRepository Projects { get; }
        public ProjectAnnouncementRepository ProjectAnnouncements { get; }
        public SongSubmissionRepository SongSubmissions { get; }
        public SongVoteRepository SongVotes { get; }

        public SilentstormDatabase(DatabaseConfiguration dbConfig) : base(dbConfig)
        {
            OAuth2 = new OAuth2Repository(this);
            Properties = new PropertyRepository(this);
            SilentstormUsers = new SilentstormUserRepository(this);
            Channels = new ChannelRepository(this);
            ProjectTypes = new ProjectTypeRepository(this);
            ProjectStatuses = new ProjectStatusRepository(this);
            Projects = new ProjectRepository(this);
            ProjectAnnouncements = new ProjectAnnouncementRepository(this);
            SongSubmissions = new SongSubmissionRepository(this);
            SongVotes = new SongVoteRepository(this);
        }

        public async Task<uint?> GetLastInsertedIdAsync()
        {
            using var reader = await ExecuteReaderAsync("SELECT LAST_INSERT_ID() AS last_insert_id");

            if (reader == null || !await reader.ReadAsync()) return null;

            var lastInsertId = await reader.GetFieldValueAsync<uint>("last_insert_id");

            return lastInsertId == 0 ? null : lastInsertId;
        }
    }
}
