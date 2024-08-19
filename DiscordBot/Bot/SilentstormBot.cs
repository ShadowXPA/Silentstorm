using CommonLib.Data.Databases;
using CommonLib.Data;
using CommonLib.Utils;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using DiscordBot.Configurations;
using DiscordBot.Commands;
using DiscordBot.Tasks;
using CommonLib.Services;
using CommonLib.Models;
using DSharpPlus.Lavalink;

namespace DiscordBot.Bot
{
    public class SilentstormBot : Bot
    {
        private readonly SilentstormBotConfiguration _silentStormBotConfig;
        private ServiceProvider? _serviceProvider;

        public SilentstormBot(string configFile) : this(ConfigurationFileUtils.LoadConfiguration<SilentstormBotConfiguration>(configFile)) { }

        public SilentstormBot(SilentstormBotConfiguration botConfiguration) : base(botConfiguration.DiscordConfiguration, "SilentStorm")
        {
            _silentStormBotConfig = botConfiguration;
        }

        public override async Task ConnectAsync()
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton<Bot>(this)
                .AddSingleton(_silentStormBotConfig)
                .AddSingleton(_silentStormBotConfig.DatabaseConfiguration!)
                .AddSingleton<SilentstormDatabase>()
                .AddSingleton<PropertiesService>()
                .AddSingleton<SilentstormUserService>()
                .AddSingleton<ChannelService>()
                .AddSingleton<ProjectTypeService>()
                .AddSingleton<ProjectStatusService>()
                .AddSingleton<ProjectService>()
                .AddSingleton<AnnouncementTask>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
            var commands = Client.UseSlashCommands(new SlashCommandsConfiguration()
            {
                Services = _serviceProvider
            });

            var interactivity = Client.UseInteractivity();

            commands.RegisterCommands<SongCommandModule>();
            commands.RegisterCommands<ChannelCommandModule>();

            var lavalink = Client.UseLavalink();

            Client.Ready += ClientReady;
            Client.ComponentInteractionCreated += ClientComponentInteractionCreated;

            await base.ConnectAsync();
            await lavalink.ConnectAsync(_silentStormBotConfig.LavalinkConfiguration);
        }

        private async Task ClientComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs args)
        {
            var projectService = _serviceProvider?.GetService<ProjectService>();
            var userService = _serviceProvider?.GetService<SilentstormUserService>();

            if (projectService is null || userService is null) return;

            await args.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .AsEphemeral(true));

            var username = args.User.Username;
            var discordId = args.User.Id;
            var user = await userService.GetSilentStormUserByUsernameAsync(username);
            var submissionId = uint.Parse(args.Id);

            if (user is null)
            {
                var added = await userService.AddSilentStormUserAsync(new SilentstormUserDto
                {
                    Username = username,
                    DiscordId = $@"{discordId}"
                });

                if (!added)
                {
                    await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                        .AsEphemeral(true)
                        .WithContent($@"Sorry, I am not able to complete your request right now. Can not vote."));
                    return;
                }

                user = await userService.GetSilentStormUserByUsernameAsync(username);
            }

            var submission = await projectService.GetSongSubmissionByIdAsync(submissionId, true);

            if (submission is null)
            {
                await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                        .AsEphemeral(true)
                        .WithContent($@"Sorry, I am not able to complete your request right now. Song submission was not found."));
                return;
            }

            var project = submission.Project!;

            if (project.ProjectStatus != ProjectStatus.Status.Voting)
            {
                await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                        .AsEphemeral(true)
                        .WithContent($@"Sorry, the project is not accepting votes at the moment."));
                return;
            }

            var registered = await projectService.AddSongVoteAsync(new()
            {
                Project = new() { Id = project.Id },
                SongSubmission = new() { Id = submission.Id },
                User = new() { Id = user!.Id }
            });

            await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                .AsEphemeral(true)
                .WithContent($@"Your vote towards ({submission.Title}) was {(registered
                    ? "registered" : "not registered. If you have already voted for a song, please contact the owner to remove the previous vote")}."));
        }

        private async Task ClientReady(DiscordClient sender, ReadyEventArgs args)
        {
            await Client.UpdateStatusAsync(_silentStormBotConfig.DiscordActivity);
            var announcementTask = _serviceProvider?.GetService<AnnouncementTask>();
            announcementTask?.Start();
        }
    }
}
