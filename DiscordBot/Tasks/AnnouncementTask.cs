using CommonLib.Data;
using DSharpPlus.Entities;
using DSharpPlus;
using System.Text;
using CommonLib.Services;
using DiscordBot.Bot;

namespace DiscordBot.Tasks
{
    public class AnnouncementTask
    {
        private readonly ProjectService _projectService;
        private Timer? _timer;
        public Bot.Bot Bot { get; init; }
        private readonly string[] _emojis =
        {
            ":one:",
            ":two:",
            ":three:",
            ":four:"
        };

        public AnnouncementTask(Bot.Bot bot, ProjectService projectService)
        {
            Bot = bot;
            _projectService = projectService;
        }

        public void Start()
        {
            Stop();
            _timer = new Timer((object? state) => AnnounceAsync().GetAwaiter().GetResult(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        public async Task AnnounceAsync()
        {
            var announcements = await _projectService.GetPendingProjectAnnouncementsAsync(true);

            foreach (var announcement in announcements)
            {
                var project = await _projectService.GetProjectAsync((uint)announcement.ProjectId!);
                if (project is null || project.ProjectStatus != announcement.ProjectStatus) continue;
                var channel = await Bot.Client.GetChannelAsync(ulong.Parse(announcement.Channel!.ChannelId!));
                var builder = new DiscordMessageBuilder();
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(announcement.Announcement);

                if (project.ProjectStatus == ProjectStatus.Status.Voting)
                {
                    var selectedSongs = await _projectService.GetAllSongSubmissionsByProjectIdAndSelectedForVotingAsync((uint)project.Id!);
                    var buttons = new List<DiscordComponent>(4);
                    var i = 0;

                    foreach (var song in selectedSongs)
                    {
                        var emoji = _emojis[i++];
                        buttons.Add(new DiscordButtonComponent(
                            ButtonStyle.Primary,
                            $@"{song.Id}",
                            string.Empty,
                            emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(Bot.Client, emoji))));
                        stringBuilder.AppendLine($@"{emoji} - {song.Title} ({song.Uri})");
                    }

                    builder.AddComponents(buttons);
                }

                await (await channel.SendMessageAsync(builder.WithContent(stringBuilder.ToString()))).ModifyEmbedSuppressionAsync(true);
                announcement.WasAnnounced = true;
                await _projectService.UpdateProjectAnnouncementAsync(announcement);
            }
        }

        public void Stop()
        {
            _timer?.Dispose();
        }
    }
}
