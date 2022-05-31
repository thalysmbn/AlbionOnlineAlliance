using System;
using System.Collections.Generic;
using AlbionOnline.Services.Mongo;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace AlbionOnline.Models
{
    [BsonCollection("discords")]
    public class DiscordGuildModel : Document
    {
        [BsonElement("discordId")]
        [JsonProperty("DiscordId")]
        public ulong DiscordId { get; set; }

        [BsonElement("roleId")]
        [JsonProperty("RoleId")]
        public ulong RoleId { get; set; }

        [BsonElement("memberRole")]
        [JsonProperty("MemberRole")]
        public ulong MemberRole { get; set; }

        [BsonElement("registerChat")]
        [JsonProperty("RegisterChat")]
        public ulong RegisterChat { get; set; }

        [BsonElement("commandChat")]
        [JsonProperty("CommandChat")]
        public ulong CommandChat { get; set; }

        [BsonElement("admins")]
        [JsonProperty("Admins")]
        public IList<ulong> Admins { get; set; }

        [BsonElement("managers")]
        [JsonProperty("Managers")]
        public IList<ulong> Managers { get; set; }

        [BsonElement("guilds")]
        [JsonProperty("Guilds")]
        public Dictionary<string, DiscordGuildDataModel> Guilds { get; set; }

        [BsonElement("blackList")]
        [JsonProperty("BlackList")]
        public IList<DiscordGuildBlackListDataModel> BlackList { get; set; } // DiscordId, Reason

        [BsonElement("logs")]
        [JsonProperty("Logs")]
        public IList<DiscordGuildLog> Logs { get; set; }
    }

    public class DiscordGuildBlackListDataModel
    {
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
    }

    public class DiscordGuildDataModel
    {
        public ulong DiscordRole { get; set; }
        public string GuildName { get; set; }
        public Dictionary<string, DiscordGuildMemberDataModel> Members { get; set; }
        public string Tag { get; set; }
    }

    public class DiscordGuildMemberDataModel
    {
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string DiscordName { get; set; }
        public ulong DiscordId { get; set; }
    }

    public class DiscordGuildLog
    {
        public ulong DiscordId { get; set; }
        public string LogTo { get; set; }
        public string LogText { get; set; }
        public DiscordGuildLogType LogType { get; set; }
        public DateTime Date { get; set; }
    }

    public enum DiscordGuildLogType
    {
        Blacklist,
        BlacklistRemove,
        Register,
        Member,
        Guild,
        Manager,
        None
    }
}