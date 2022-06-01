using AlbionOnline.Services.Mongo;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace AlbionOnline.Models
{
    [BsonCollection("guild")]
    public class GuildModel : Document
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [BsonElement("guildId")]
        public string Id { get; set; }

        [BsonElement("updatedAt")] public DateTime UpdatedAt { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("name")]
        [JsonProperty("Name")]
        public string Name { get; set; }

        [BsonElement("founderId")]
        [JsonProperty("FounderId")]
        public string FounderId { get; set; }

        [BsonElement("founderName")]
        [JsonProperty("FounderName")]
        public string FounderName { get; set; }

        [BsonElement("founded")]
        [JsonProperty("Founded")]
        public string Founded { get; set; }

        [BsonElement("allianceTag")]
        [JsonProperty("AllianceTag")]
        public string AllianceTag { get; set; }

        [BsonElement("allianceId")]
        [JsonProperty("AllianceId")]
        public string AllianceId { get; set; }

        [BsonElement("allianceName")]
        [JsonProperty("AllianceName")]
        public string AllianceName { get; set; }

        [BsonElement("logo")]
        [JsonProperty("Logo")]
        public string Logo { get; set; }

        [BsonElement("killFame")]
        [JsonProperty("KillFame")]
        public long KillFame { get; set; }

        [BsonElement("deathFame")]
        [JsonProperty("DeathFame")]
        public long DeathFame { get; set; }

        [BsonElement("attacksWon")]
        [JsonProperty("AttacksWon")]
        public string? AttacksWon { get; set; }

        [BsonElement("defensesWon")]
        [JsonProperty("DefensesWon")]
        public string? DefensesWon { get; set; }

        [BsonElement("memberCount")]
        [JsonProperty("MemberCount")]
        public long MemberCount { get; set; }
    }

    public class GuildOverall
    {
        [BsonElement("kills")]
        [JsonProperty("Kills")]
        public long Kills { get; set; }

        [BsonElement("gvgKills")]
        [JsonProperty("gvgKills")]
        public long GvGKills { get; set; }
    }
}