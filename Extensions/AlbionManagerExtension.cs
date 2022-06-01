using AlbionOnline.JSON;
using AlbionOnline.Models;
using AlbionOnline.Services.Mongo;
using Discord.Commands;
using Discord.WebSocket;
using System.Text;

namespace AlbionOnline.Extensions
{
    public static class AlbionManagerExtension
    {
        public static async Task CheckMembers(this SocketCommandContext context, DiscordGuildModel discord,
            IJSONAlbion jsonAlbion, IMongoRepository<DiscordGuildModel> discordGuildRepository)
        {
            var discordMemberUsers = context.Guild.Users.Where(x => x.Roles.Any(x => x.Id == discord.MemberRole));

            var membersToCheck = discordMemberUsers.Select(member => member.Id).ToList();

            foreach (var member in from guild in discord.Guilds
                                   select guild.Value.Members.Values
                     into members
                                   from member in members
                                   where membersToCheck.Any(x => x == member.DiscordId)
                                   select member) membersToCheck.Remove(member.DiscordId);

            var messageString = new StringBuilder();

            foreach (var member in membersToCheck)
            {
                var last = membersToCheck.Last();
                messageString.Append($"<@{member}> is not registered in any guild." +
                                     (last != member ? "\n" : ""));
            }

            if (membersToCheck.Count > 0)
                await context.Channel.SendMessageAsync(messageString.ToString());

            foreach (var guild in discord.Guilds)
            {
                await CheckGuildMembers(context, discord, guild, jsonAlbion, discordGuildRepository);
            }
        }

        public static async Task CheckGuildMembers(this SocketCommandContext context, DiscordGuildModel discord,
            KeyValuePair<string, DiscordGuildDataModel> guild, IJSONAlbion jsonService,
            IMongoRepository<DiscordGuildModel> discordGuildRepository)
        {
            var update = false;
            var discordUsers = context.Guild.Users;

            var guildMembers = guild.Value.Members.Values;

            // check discord
            var actionsMember = new Dictionary<ulong, ActionMember>();
            var socketGuildUsers = discordUsers as SocketGuildUser[] ?? discordUsers.ToArray();

            if (!context.Guild.Roles.Any(x => x.Id == guild.Value.DiscordRole))
            {
                foreach (var member in guildMembers)
                {
                    if (!socketGuildUsers.Any(x => x.Id == member.DiscordId))
                        continue;
                    var user = context.Guild.GetUser(member.DiscordId);
                    if (user == null)
                        continue;
                    if (user.Roles.Any(x => x.Id == discord.MemberRole))
                        await user.RemoveRoleAsync(discord.MemberRole);
                }
                discord.Guilds.Remove(guild.Key);
                await discordGuildRepository.ReplaceOneAsync(discord);
                await context.Channel.SendMessageAsync($"**{guild.Value.GuildName}** ``{guild.Value.Tag}`` **[REMOVED]**");
                return;
            }
            var socketGuildUsersRoles = discordUsers as SocketGuildUser[] ?? discordUsers.Where(x => x.Roles.Any(x => x.Id == guild.Value.DiscordRole)).ToArray();

            var albionGuildMembers = await jsonService.GetGuildMembers(guild.Key);
            foreach (var member in guildMembers)
            {
                if (!socketGuildUsers.Any(x => x.Id == member.DiscordId))
                {
                    actionsMember.TryAdd(member.DiscordId, new ActionMember
                    {
                        AccountName = member.AccountName,
                        AlbionId = member.AccountId,
                        DiscordId = member.DiscordId,
                        Type = ActionMemberType.IsNotInDiscord
                    });
                    continue;
                }
                if (!albionGuildMembers.Any(x => x.Id == member.AccountId))
                {
                    actionsMember.TryAdd(member.DiscordId, new ActionMember
                    {
                        AccountName = member.AccountName,
                        AlbionId = member.AccountId,
                        DiscordId = member.DiscordId,
                        Type = ActionMemberType.IsNotInGuild
                    });
                    continue;
                }
                if (!socketGuildUsersRoles.Any(x => x.Id == member.DiscordId))
                {
                    if (!socketGuildUsers.Any(x => x.Id == member.DiscordId))
                        continue;
                    actionsMember.TryAdd(member.DiscordId, new ActionMember
                    {
                        AccountName = member.AccountName,
                        AlbionId = member.AccountId,
                        DiscordId = member.DiscordId,
                        Type = ActionMemberType.WithoutRole
                    });
                    continue;
                }
            }

            var message = await context.Channel.SendMessageAsync($"**{guild.Value.GuildName}** ``{guild.Value.Tag}`` Loading...");
            if (actionsMember.Count > 0)
            {
                var messageString = new StringBuilder();
                foreach (var remove in actionsMember.ToArray())
                {
                    var last = actionsMember.Last();
                    switch (remove.Value.Type)
                    {
                        case ActionMemberType.IsNotInDiscord:
                            guild.Value.Members.Remove(remove.Value.AlbionId);
                            messageString.Append(
                                $"**{guild.Value.GuildName}>** **{remove.Value.AccountName}** ``{remove.Value.AlbionId}`` is not in discord **[REMOVED]**" +
                                (last.Key != remove.Key ? "\n" : ""));
                            update = true;
                            continue;
                        case ActionMemberType.IsNotInGuild:
                            var user = context.Guild.GetUser(remove.Value.DiscordId);
                            if (user.Roles.Any(x => x.Id == discord.MemberRole))
                                await user.RemoveRoleAsync(discord.MemberRole);
                            if (user.Roles.Any(x => x.Id == guild.Value.DiscordRole))
                                await user.RemoveRoleAsync(guild.Value.DiscordRole);
                            guild.Value.Members.Remove(remove.Value.AlbionId);
                            messageString.Append($"**{guild.Value.GuildName}** ``{guild.Value.Tag}`` <@{remove.Value.DiscordId}> is not in guild **[REMOVED]**" +
                                (last.Key != remove.Key ? "\n" : ""));
                            update = true;
                            continue;
                        case ActionMemberType.WithoutRole:
                            var userToRole = context.Guild.GetUser(remove.Value.DiscordId);
                            if (!userToRole.Roles.Any(x => x.Id == discord.MemberRole))
                                await userToRole.AddRoleAsync(discord.MemberRole);
                            if (!userToRole.Roles.Any(x => x.Id == guild.Value.DiscordRole))
                                await userToRole.AddRoleAsync(guild.Value.DiscordRole);
                            messageString.Append(
                                $"**{guild.Value.GuildName}** ``{guild.Value.Tag}`` <@{remove.Value.DiscordId}> registered to role" +
                                (last.Key != remove.Key ? "\n" : ""));
                            continue;
                    }
                }
                if (update)
                {
                    await discordGuildRepository.ReplaceOneAsync(discord);
                }
                await message.ModifyAsync(msg => msg.Content = messageString.ToString());
            }
            else
            {
                await message.ModifyAsync(msg => msg.Content = $"**{guild.Value.GuildName}** ``{guild.Value.Tag}`` All okay");
            }

        }
    }
}
