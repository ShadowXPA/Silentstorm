using DSharpPlus.Entities;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.SlashCommands;
using CommonLib.Services;
using CommonLib.Models;

namespace DiscordBot.Commands
{
    public class ChannelCommandModule : ApplicationCommandModule
    {
        private readonly ChannelService _channelService;

        public ChannelCommandModule(ChannelService channelService)
        {
            _channelService = channelService;
        }

        [SlashRequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        [SlashCommand("register", "Registers this channel to receive notifications")]
        public async Task RegisterChannelAsync(InteractionContext ctx)
        {
            await ctx.DeferAsync(true);

            var added = await _channelService.AddChannelAsync(new ChannelDto
            {
                ChannelId = $@"{ctx.Channel.Id}",
                GuildId = $@"{ctx.Channel.GuildId}",
                ChannelName = $@"{ctx.Channel.Name} ({ctx.Channel.Guild.Name})"
            });

            if (!added)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent($@"Could not register channel! Perhaps it has already been registered"));
                return;
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent($@"Channel has been registered to receive notifications!"));
        }

        [SlashRequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        [SlashCommand("unregister", "Unregisters this channel from receiving notifications")]
        public async Task UnregisterChannelAsync(InteractionContext ctx)
        {
            await ctx.DeferAsync(true);

            var unregistered = await _channelService.RemoveChannelAsync($@"{ctx.Channel.Id}");

            if (!unregistered)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent($@"Could not unregister channel!"));
                return;
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent($@"Channel {(unregistered ? "will no longer be receiving notifications" : "is not registered")}!"));
        }
    }
}
