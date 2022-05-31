using AlbionOnline.Extensions;
using AlbionOnline.JSON;
using AlbionOnline.Models;
using AlbionOnline.Services.Mongo;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbionOnline
{
    public class DiscordCommandModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;
        private readonly IMongoRepository<DiscordGuildModel> _discordGuildRepository;
        private readonly IJSONAlbion _jsonAlbion;

        public DiscordCommandModule(CommandService commandService,
            IMongoRepository<DiscordGuildModel> discordGuildRepository,
            IJSONAlbion jsonAlbion)
        {
            _commandService = commandService;
            _discordGuildRepository = discordGuildRepository;
            _jsonAlbion = jsonAlbion;
        }

        [Command("help")]
        public async Task Help([Remainder] string command = null)
        {
            var discord = await _discordGuildRepository.FindOneAsync(x => x.DiscordId == Context.Guild.Id);
            if (discord == null)
                return;
            await ReplyAsync("Here's a list of commands and their description: ", false,
                _commandService.GetDefaultHelpEmbed(command));
        }

        #region albion
        [Command("search")]
        public async Task SearchAsync([Remainder] string text)
        {
            var discord = await _discordGuildRepository.FindOneAsync(x => x.DiscordId == Context.Guild.Id);
            if (discord == null)
                return;
            if (Context.Message.Channel.Id != discord.CommandChat)
            {
                await Context.Channel.SendMessageAsync("Invalid command chat.");
            }
            else if (!discord.Managers.Contains(Context.User.Id))
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> you are not a manager.");
            }
            else
            {
                var message = await Context.Channel.SendMessageAsync($"Looking for ``{text}``...");
                var seachModel = await _jsonAlbion.Search(text);
                var stringBuilder = new StringBuilder();
                if (seachModel.Guilds.Count > 0) stringBuilder.Append("**Guilds**:\n");
                foreach (var guild in seachModel.Guilds) stringBuilder.Append($"{guild.Name} ``{guild.Id}``\n");
                if (seachModel.Players.Count > 0) stringBuilder.Append("**Players**:\n");
                foreach (var player in seachModel.Players)
                {
                    var guildBuilder = new StringBuilder(player.GuildName);
                    stringBuilder.Append($"{player.Name}  ");
                    if (player.AllianceId != "")
                        stringBuilder.Append($" **[{player.AllianceName}]**");
                    if (player.GuildId != "")
                        stringBuilder.Append($" **{player.GuildName}**");
                    if (discord.BlackList.Any(x => x.AccountId == player.Id))
                        stringBuilder.Append("**BLACKLISTED**");
                    else
                        stringBuilder.Append($"    ``{player.Id}``");
                    stringBuilder.Append("\n");
                }
                if (stringBuilder.Length > 0)
                    await message.ModifyAsync(msg => msg.Content = stringBuilder.ToString());
                else
                    await message.ModifyAsync(msg => msg.Content = "Nothing found.");
            }
        }

        [Command("check")]
        public async Task CheckAsync(IRole role = null)
        {
            var discord = await _discordGuildRepository.FindOneAsync(x => x.DiscordId == Context.Guild.Id);
            if (discord == null)
                return;
            if (Context.Message.Channel.Id != discord.CommandChat)
            {
                await Context.Channel.SendMessageAsync("Invalid command chat.");
            }
            else if (!discord.Managers.Contains(Context.User.Id))
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> you are not a manager.");
            }
            else
            {
                if (role == null)
                {
                    await Context.CheckMembers(discord, _jsonAlbion, _discordGuildRepository);
                }
                else
                {
                    var guild = discord.Guilds.FirstOrDefault(x => x.Value.DiscordRole == role.Id);
                    await Context.CheckGuildMembers(discord, guild, _jsonAlbion, _discordGuildRepository);
                }
            }
        }

        [Command("members")]
        public async Task MembersAsync(IRole role = null)
        {
            var discord = await _discordGuildRepository.FindOneAsync(x => x.DiscordId == Context.Guild.Id);
            if (discord == null)
                return;
            if (Context.Message.Channel.Id != discord.CommandChat)
            {
                await Context.Channel.SendMessageAsync("Invalid command chat.");
            }
            else if (!discord.Managers.Contains(Context.User.Id))
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> you are not a manager.");
            }
            else if (role != null)
            {
                var (key, value) = discord.Guilds.FirstOrDefault(x => x.Value.DiscordRole == role.Id);
                if (value != null)
                {
                    var message =
                        await Context.Channel.SendMessageAsync(
                            $"**{value.GuildName}** ( <@&{value.DiscordRole}> )");
                    var embed = new EmbedBuilder();
                    var nameBuilder = new StringBuilder();
                    var discordBuilder = new StringBuilder();
                    embed.Title = value.GuildName;
                    embed.Description = $"<@&{value.DiscordRole}>";

                    var guildMembers = await _jsonAlbion.GetGuildMembers(key);
                    foreach (var guildMember in guildMembers)
                    {
                        nameBuilder.Append($"{guildMember.Name} \n");
                        discordBuilder.Append(value.Members.TryGetValue(guildMember.Id, out var member)
                            ? $"<@{member.DiscordId}>\n"
                            : "-\n");
                    }

                    embed.AddField("Name", nameBuilder.ToString(), true);
                    embed.AddField("Discord", discordBuilder.ToString(), true);
                    await message.ModifyAsync(mg => mg.Embed = embed.Build());
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"<@&{role.Id}> not found.");
                }
            }
            else
            {
                var nameBuilder = new StringBuilder();
                foreach (var guild in discord.Guilds) nameBuilder.Append($"<@&{guild.Value.DiscordRole}>\n");
                await Context.Channel.SendMessageAsync(nameBuilder.ToString());
            }
        }

        [Command("guild")]
        public async Task GuildAsync(string guild, string tag)
        {
            await Context.Channel.SendMessageAsync($"Discord ID: ``{Context.Guild.Id}``");
            var discord = await _discordGuildRepository.FindOneAsync(x => x.DiscordId == Context.Guild.Id);
            if (discord == null)
                return;
            if (Context.Message.Channel.Id != discord.CommandChat)
            {
                await Context.Channel.SendMessageAsync("Invalid command chat.");
            }
            else if (!discord.Admins.Contains(Context.User.Id))
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> you are not a admin.");
            }
            else
            {
                if (tag == null)
                    return;
                var message = await Context.Channel.SendMessageAsync($"Adding ``{guild}``...");
                var data = await _jsonAlbion.GetGuild(guild);
                if (discord.Guilds.TryGetValue(guild, out var guildModel))
                {
                    var guildData = discord.Guilds.FirstOrDefault(x => x.Key == data.Id);
                    await Context.Channel.SendMessageAsync(
                        $"**{data.Name}** has already been added: <@&{guildData.Value.DiscordRole}>");
                }
                else
                {
                    var role = await Context.Guild.CreateRoleAsync(data.Name, color: Color.Default,
                        isMentionable: true);
                    discord.Logs.Add(new DiscordGuildLog
                    {
                        DiscordId = Context.User.Id,
                        LogTo = Context.User.Username,
                        LogText = $"<@{Context.User.Id}> added <@&{role.Id}>",
                        LogType = DiscordGuildLogType.Guild,
                        Date = DateTime.Now
                    });
                    discord.Guilds.Add(guild, new DiscordGuildDataModel
                    {
                        DiscordRole = role.Id,
                        GuildName = data.Name,
                        Tag = tag,
                        Members = new Dictionary<string, DiscordGuildMemberDataModel>()
                    });
                    await _discordGuildRepository.ReplaceOneAsync(discord);
                    await message.ModifyAsync(msg => msg.Content = $"Guild added: <@&{role.Id}>");
                }
            }
        }

        [Command("guilds")]
        public async Task GuildsAsync()
        {
            var discord = await _discordGuildRepository.FindOneAsync(x => x.DiscordId == Context.Guild.Id);
            if (discord == null)
                return;
            if (Context.Message.Channel.Id != discord.CommandChat)
            {
                await Context.Channel.SendMessageAsync("Invalid command chat.");
            }
            else if (!discord.Managers.Contains(Context.User.Id))
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> you are not a manager.");
            }
            else
            {
                var guildsBuilder = new StringBuilder();
                foreach (var guild in discord.Guilds)
                    guildsBuilder.Append($"**{guild.Value.GuildName}** ``{guild.Value.Tag}``\n");
                await Context.Channel.SendMessageAsync(guildsBuilder.ToString());
            }
        }
        #endregion

        #region staff
        [Command("admins")]
        public async Task AdminsAsync()
        {
            var discord = await _discordGuildRepository.FindOneAsync(x => x.DiscordId == Context.Guild.Id);
            if (discord == null)
                return;
            if (discord.Admins.Contains(Context.User.Id) || discord.Managers.Contains(Context.User.Id))
            {
                var message = new StringBuilder();
                foreach (var manager in discord.Admins) message.Append($" <@{manager}> ");
                await Context.Channel.SendMessageAsync($"Admins: {message}");
            }
        }

        [Command("manager")]
        public async Task RegisterAsync(IGuildUser user)
        {
            var userInfo = Context.Message.Author;
            var discord = await _discordGuildRepository.FindOneAsync(x => x.DiscordId == Context.Guild.Id);
            if (discord == null)
            {
                var armsRole = await Context.Guild.CreateRoleAsync("Arms", color: Color.Teal, isMentionable: false);
                var memberRole =
                    await Context.Guild.CreateRoleAsync("Member", color: Color.Green, isMentionable: false);
                var register = await Context.Guild.CreateTextChannelAsync("Register");
                var command = await Context.Guild.CreateTextChannelAsync("Command");
                await user.AddRoleAsync(armsRole);
                await user.AddRoleAsync(memberRole);
                await _discordGuildRepository.InsertOneAsync(new DiscordGuildModel
                {
                    RoleId = armsRole.Id,
                    MemberRole = memberRole.Id,
                    RegisterChat = register.Id,
                    CommandChat = command.Id,
                    DiscordId = Context.Guild.Id,
                    Admins = new List<ulong> { user.Id },
                    Managers = new List<ulong> { user.Id },
                    Guilds = new Dictionary<string, DiscordGuildDataModel>(),
                    BlackList = new List<DiscordGuildBlackListDataModel>(),
                    Logs = new List<DiscordGuildLog>()
                });
                await Context.Channel.SendMessageAsync("Server has been configured.");
            }
            else
            {
                if (Context.Message.Channel.Id != discord.CommandChat)
                {
                    await Context.Channel.SendMessageAsync("Invalid command chat.");
                }
                else if (discord.Managers.Contains(user.Id))
                {
                    await Context.Channel.SendMessageAsync($"<@{user.Id}> is already a manager.");
                }
                else if (!discord.Admins.Contains(Context.User.Id))
                {
                    await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> you are not a admin.");
                }
                else if (discord.Managers.Contains(Context.User.Id) || !discord.Managers.Contains(user.Id))
                {
                    discord.Logs.Add(new DiscordGuildLog
                    {
                        DiscordId = Context.User.Id,
                        LogTo = Context.User.Username,
                        LogText = $"<@{Context.User.Id}> added <@{user.Id}> as <@&{discord.RoleId}>",
                        LogType = DiscordGuildLogType.Manager,
                        Date = DateTime.Now
                    });
                    discord.Managers.Add(user.Id);
                    await _discordGuildRepository.ReplaceOneAsync(discord);
                    await user.AddRoleAsync(discord.RoleId);
                    await Context.Channel.SendMessageAsync(
                        $"<@{Context.User.Id}> added <@{user.Id}> to <@&{discord.RoleId}>.");
                }
            }
        }

        [Command("managers")]
        public async Task ManagersAsync()
        {
            var discord = await _discordGuildRepository.FindOneAsync(x => x.DiscordId == Context.Guild.Id);
            if (discord == null)
                return;
            if (discord.Admins.Contains(Context.User.Id) || discord.Managers.Contains(Context.User.Id))
            {
                var toRemove = new List<ulong>();
                var message = new StringBuilder();
                foreach (var manager in discord.Managers)
                {

                    if (!Context.Guild.Users.Any(x => x.Id == manager))
                    {
                        toRemove.Add(manager);
                        continue;
                    }
                    message.Append($" <@{manager}> ");
                }
                if (toRemove.Count > 0)
                {
                    foreach (var remove in toRemove)
                        discord.Managers.Remove(remove);
                    await _discordGuildRepository.ReplaceOneAsync(discord);
                }
                await Context.Channel.SendMessageAsync($"Managers: {message}");
            }
        }
        #endregion

        #region black
        [Command("blackremove")]
        public async Task BlackRemoveAsync(string member, [Remainder] string reason)
        {
            var discord = await _discordGuildRepository.FindOneAsync(x => x.DiscordId == Context.Guild.Id);
            if (discord == null)
                return;
            if (Context.Message.Channel.Id != discord.CommandChat)
                await Context.Channel.SendMessageAsync("Invalid command chat.");
            else if (!discord.Admins.Contains(Context.User.Id))
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> you are not a admin.");
            else
            {
                var blackList = discord.BlackList.FirstOrDefault(x => x.AccountId == member);
                if (blackList != null)
                {
                    discord.BlackList.Remove(blackList);
                    discord.Logs.Add(new DiscordGuildLog
                    {
                        DiscordId = Context.User.Id,
                        LogTo = blackList.AccountName,
                        LogText = $"<@{Context.User.Id}> removed **{blackList.AccountName}** ``( {member} )`` from blacklist. **Reason**: {reason}",
                        LogType = DiscordGuildLogType.BlacklistRemove,
                        Date = DateTime.Now
                    });
                    await _discordGuildRepository.ReplaceOneAsync(discord);
                    await Context.Channel.SendMessageAsync($"**{blackList.AccountName}** ``( {member} )`` has been removed from blacklist");
                }
            }
        }

        [Command("black")]
        public async Task BlackAsync(string member, [Remainder] string reason)
        {
            var discord = await _discordGuildRepository.FindOneAsync(x => x.DiscordId == Context.Guild.Id);
            if (discord == null)
                return;
            if (Context.Message.Channel.Id != discord.CommandChat)
                await Context.Channel.SendMessageAsync("Invalid command chat.");
            else if (!discord.Admins.Contains(Context.User.Id))
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> you are not a admin.");
            else
            {
                var data = await _jsonAlbion.GetPlayerAsync(member);
                if (data == null)
                {
                    await Context.Channel.SendMessageAsync($"``{member}`` not found.");
                }
                else if (discord.BlackList.Any(x => x.AccountId == data.Id))
                {
                    await Context.Channel.SendMessageAsync($"**{data.Name}** blacklisted!");
                }
                else
                {
                    if (discord.BlackList.All(x => x.AccountId != member))
                    {
                        discord.BlackList.Add(new DiscordGuildBlackListDataModel
                        {
                            AccountId = data.Id,
                            AccountName = data.Name,
                            Reason = reason,
                            Date = DateTime.Now
                        });
                        foreach (var guild in discord.Guilds.Values)
                            if (guild.Members.Any(x => x.Key == data.Id))
                            {
                                var guildMember = guild.Members.FirstOrDefault(x => x.Key == data.Id).Value;
                                var discordUser =
                                    Context.Guild.Users.FirstOrDefault(x => x.Id == guildMember.DiscordId);
                                if (discordUser != null)
                                    if (discordUser.Roles.Any(x => x.Id == guild.DiscordRole))
                                        await discordUser.RemoveRoleAsync(guild.DiscordRole);
                                if (discordUser.Roles.Any(x => x.Id == discord.MemberRole))
                                    await discordUser.RemoveRoleAsync(discord.MemberRole);
                                guild.Members.Remove(guildMember.AccountId);
                            }
                        discord.Logs.Add(new DiscordGuildLog
                        {
                            DiscordId = Context.User.Id,
                            LogTo = data.Name,
                            LogText = $"<@{Context.User.Id}> black listed **{data.Name}** ``( {member} )``. **Reason**: {reason}",
                            LogType = DiscordGuildLogType.Blacklist,
                            Date = DateTime.Now
                        });
                        await _discordGuildRepository.ReplaceOneAsync(discord);
                        await Context.Channel.SendMessageAsync($"**{data.Name}** ``( {member} )`` blacklisted");
                    }
                }
            }
        }

        [Command("blacklist")]
        public async Task BlackListAsync()
        {
            var discord = await _discordGuildRepository.FindOneAsync(x => x.DiscordId == Context.Guild.Id);
            if (discord == null)
                return;
            if (Context.Message.Channel.Id != discord.CommandChat)
            {
                await Context.Channel.SendMessageAsync("Invalid command chat.");
            }
            else if (!discord.Managers.Contains(Context.User.Id))
            {
                await Context.Channel.SendMessageAsync($"<@{Context.User.Id}> you are not a manager.");
            }
            else
            {
                var blackListBuilder = new StringBuilder();
                foreach (var blackList in discord.BlackList)
                    blackListBuilder.Append($"``{blackList.Date}`` **{blackList.AccountName}** - {blackList.Reason}\n");
                await Context.Channel.SendMessageAsync(blackListBuilder.ToString());
            }
        }
        #endregion

        #region logs
        [Command("logm")]
        public async Task Log([Remainder] SocketGuildUser user = null)
        {
            var discord = await _discordGuildRepository.FindOneAsync(x => x.DiscordId == Context.Guild.Id);
            if (discord == null)
                return;

            var textBuilder = new StringBuilder();

            var logs = discord.Logs.Where(x => x.DiscordId == user.Id).OrderByDescending(x => x.Date);
            foreach (var log in logs.Take(10)) textBuilder.Append($"``{log.Date}`` {log.LogText}\n");
            await Context.Channel.SendMessageAsync(textBuilder.ToString());
        }

        [Command("logs")]
        public async Task Logs(string key = null)
        {
            var discord = await _discordGuildRepository.FindOneAsync(x => x.DiscordId == Context.Guild.Id);
            if (discord == null)
                return;

            var textBuilder = new StringBuilder();
            DiscordGuildLogType logTo = key == "blacklist" ? DiscordGuildLogType.Blacklist : key == "member" ? DiscordGuildLogType.Member : key == "register" ? DiscordGuildLogType.Register : key == "manager" ? DiscordGuildLogType.Manager : key == "guild" ? DiscordGuildLogType.Guild : key == "blacklistremove" ? DiscordGuildLogType.BlacklistRemove : DiscordGuildLogType.None;
            var logs = discord.Logs.Where(a => key == null || a.LogType == logTo || a.LogTo == key).OrderByDescending(x => x.Date);

            foreach (var log in logs.Take(10)) textBuilder.Append($"``{log.Date}`` {log.LogText}\n");
            await Context.Channel.SendMessageAsync(textBuilder.ToString());
        }
        #endregion
    }
}
