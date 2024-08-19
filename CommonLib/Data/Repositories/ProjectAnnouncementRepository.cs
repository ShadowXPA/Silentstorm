using CommonLib.Data.Databases;
using CommonLib.Utils;
using System.Data;
using System.Data.Common;

namespace CommonLib.Data.Repositories
{
    public class ProjectAnnouncementRepository : IReposiroty<ProjectAnnouncementId, ProjectAnnouncement>
    {
        private readonly SilentstormDatabase _db;

        public ProjectAnnouncementRepository(SilentstormDatabase database)
        {
            _db = database;
        }

        public async Task<ProjectAnnouncement?> FindAsync(ProjectAnnouncementId id)
        {
            return await FindAsync(id, false);
        }

        public async Task<ProjectAnnouncement?> FindAsync(ProjectAnnouncementId id, bool eagerLoad)
        {
            ProjectAnnouncement? projectAnnouncement = null;
            var query = "SELECT * FROM project_announcement pa";

            if (eagerLoad)
                query += " INNER JOIN channel c ON pa.channel_id = c.channel_id";

            query += " WHERE pa.proj_id = @projectId AND pa.proj_status = @projectStatus";

            using var reader = await _db.ExecuteReaderAsync(query, new()
            {
                { "projectId", id.ProjectId },
                { "projectStatus", id.ProjectStatus }
            });

            if (reader == null || !await reader.ReadAsync()) return projectAnnouncement;

            projectAnnouncement = await ParseAsync(reader, eagerLoad);

            return projectAnnouncement;
        }

        public async Task<List<ProjectAnnouncement>> GetAllAsync()
        {
            return await GetAllAsync(false);
        }

        public async Task<List<ProjectAnnouncement>> GetAllAsync(bool eagerLoad)
        {
            var projectAnnouncements = new List<ProjectAnnouncement>();
            var query = "SELECT * FROM project_announcement pa";

            if (eagerLoad)
                query += " INNER JOIN channel c ON pa.channel_id = c.channel_id";

            using var reader = await _db.ExecuteReaderAsync(query);

            if (reader == null) return projectAnnouncements;

            while (await reader.ReadAsync())
                projectAnnouncements.Add(await ParseAsync(reader, eagerLoad));

            return projectAnnouncements;
        }

        public async Task<List<ProjectAnnouncement>> GetAllForProjectIdAsync(uint projectId, bool eagerLoad = false)
        {
            var projectAnnouncements = new List<ProjectAnnouncement>();
            var query = "SELECT * FROM project_announcement pa";

            if (eagerLoad)
                query += " INNER JOIN channel c ON pa.channel_id = c.channel_id";

            query += " WHERE proj_id = @projectId";

            using var reader = await _db.ExecuteReaderAsync(query, new()
            {
                { "projectId", projectId }
            });

            if (reader == null) return projectAnnouncements;

            while (await reader.ReadAsync())
                projectAnnouncements.Add(await ParseAsync(reader, eagerLoad));

            return projectAnnouncements;
        }

        public async Task<List<ProjectAnnouncement>> GetPendingAsync(bool eagerLoad = false)
        {
            var projectAnnouncements = new List<ProjectAnnouncement>();
            var query = "SELECT * FROM project_announcement pa";

            if (eagerLoad)
                query += " INNER JOIN channel c ON pa.channel_id = c.channel_id";

            query += " WHERE was_announced = @wasAnnounced AND announcement_date <= @announcementDate";

            using var reader = await _db.ExecuteReaderAsync(query, new()
            {
                { "wasAnnounced", 0 },
                { "announcementDate", DateTime.Now }
            });

            if (reader == null) return projectAnnouncements;

            while (await reader.ReadAsync())
                projectAnnouncements.Add(await ParseAsync(reader, eagerLoad));

            return projectAnnouncements;
        }

        public async Task<bool> AddAsync(ProjectAnnouncement row)
        {
            var result = await _db.ExecuteNonQueryAsync("INSERT INTO project_announcement(proj_id, proj_status, channel_id, announcement, announcement_date, was_announced)"
                + " VALUES(@projectId, @projectStatus, @channelId, @announcement, @announcementDate, @wasAnnounced)", new()
            {
                { "projectId", row.Id.ProjectId },
                { "projectStatus", row.Id.ProjectStatus },
                { "channelId", row.ChannelId },
                { "announcement", row.Announcement },
                { "announcementDate", row.AnnouncementDate },
                { "wasAnnounced", row.WasAnnounced }
            });

            return result != 0;
        }

        public async Task<bool> UpdateAsync(ProjectAnnouncement row)
        {
            var result = await _db.ExecuteNonQueryAsync("UPDATE project_announcement "
                + "SET channel_id = @channelId,"
                + " announcement = @announcement,"
                + " announcement_date = @announcementDate,"
                + " was_announced = @wasAnnounced"
                + " WHERE proj_id = @projectId AND proj_status = @projectStatus", new()
            {
                { "projectId", row.Id.ProjectId },
                { "projectStatus", row.Id.ProjectStatus },
                { "channelId", row.ChannelId },
                { "announcement", row.Announcement },
                { "announcementDate", row.AnnouncementDate },
                { "wasAnnounced", row.WasAnnounced }
            });

            return result != 0;
        }

        public async Task<bool> RemoveAsync(ProjectAnnouncementId id)
        {
            var result = await _db.ExecuteNonQueryAsync("DELETE FROM project_announcement WHERE proj_id = @projectId AND proj_status = @projectStatus", new()
            {
                { "projectId", id.ProjectId },
                { "projectStatus", id.ProjectStatus }
            });

            return result != 0;
        }

        public async Task<ProjectAnnouncement> ParseAsync(DbDataReader reader)
        {
            return await ParseAsync(reader, false);
        }

        public async Task<ProjectAnnouncement> ParseAsync(DbDataReader reader, bool eagerLoad)
        {
            var projectId = await reader.GetFieldValueAsync<uint>("proj_id");
            var projectStatus = await reader.GetFieldValueAsync<string>("proj_status");

            return new ProjectAnnouncement
            {
                Id = new ProjectAnnouncementId(projectId, projectStatus),
                ChannelId = await reader.GetFieldValueAsync<string>("channel_id"),
                Channel = eagerLoad ? await _db.Channels.ParseAsync(reader) : null,
                Announcement = await reader.GetNullableFieldValueAsync<string?>("announcement"),
                AnnouncementDate = await reader.GetNullableFieldValueAsync<DateTime?>("announcement_date"),
                WasAnnounced = await reader.GetFieldValueAsync<bool>("was_announced")
            };
        }
    }
}
