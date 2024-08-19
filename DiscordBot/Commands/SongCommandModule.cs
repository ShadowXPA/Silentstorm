using CommonLib.Data;
using CommonLib.Models;
using CommonLib.Services;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands
{
    public class SongCommandModule : ApplicationCommandModule
    {
        private readonly ProjectService _projectService;
        private readonly SilentstormUserService _silentstormUserService;

        public SongCommandModule(ProjectService projectService, SilentstormUserService silentstormUserService)
        {
            _projectService = projectService;
            _silentstormUserService = silentstormUserService;
        }

        [SlashCommand("submit", "Submits a song to the current project")]
        public async Task SubmitSong(InteractionContext ctx, [Option("song", "Song to be submitted")] string song)
        {
            await ctx.DeferAsync(true);

            var user = await _silentstormUserService.GetSilentStormUserByUsernameAsync(ctx.User.Username);

            if (user is null)
            {
                var added = await _silentstormUserService.AddSilentStormUserAsync(new SilentstormUserDto
                {
                    Username = ctx.User.Username,
                    DiscordId = $@"{ctx.User.Id}"
                });

                if (!added)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                        .WithContent($@"Sorry, I am not able to complete your request right now. Can not register user."));
                    return;
                }

                user = await _silentstormUserService.GetSilentStormUserByUsernameAsync(ctx.User.Username);
            }

            var projects = await _projectService.GetProjectsByStatusAsync(ProjectStatus.Status.Submission);

            if (projects.Count == 0)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent($@"Sorry, there are no projects running right now."));
                return;
            }

            var project = projects.First();
            var lavalink = ctx.Client.GetLavalink();

            if (!lavalink.ConnectedNodes.Any())
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent($@"Sorry, I am not able to complete your request right now. Can not establish connection."));
                return;
            }

            var idealNode = lavalink.GetIdealNodeConnection();
            LavalinkLoadResult loadResult;

            if (Uri.TryCreate(song, UriKind.Absolute, out Uri? url))
                loadResult = await idealNode.Rest.GetTracksAsync(url);
            else
                loadResult = await idealNode.Rest.GetTracksAsync(song);

            var resultType = loadResult.LoadResultType;

            if (resultType == LavalinkLoadResultType.LoadFailed || resultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent($@"The song you submitted is not available."));
                return;
            }

            var track = loadResult.Tracks.First();
            var songTitle = track.Title;
            var songUrl = track.Uri;

            var songSubmission = new SongSubmissionDto()
            {
                Project = project,
                User = user,
                Title = songTitle,
                Uri = songUrl.ToString()
            };

            var registered = await _projectService.AddSongSubmissionAsync(songSubmission);

            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent($@"Your song ({songTitle}) was {(registered ? "registered" : "not registered. If you have already submitted a song, please contact the owner to remove the previous submission")}."));
        }
    }
}
