using System;
using System.Collections.Generic;
using AlbionOnline.Services.Mongo;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace AlbionOnline.Models
{
    [BsonCollection("alliance")]
    public class AllianceModel : Document
    {
        [BsonElement("allianceId")]
        [JsonProperty("AllianceId")]
        public string AllianceId { get; set; }

        [BsonElement("AllianceName")]
        [JsonProperty("AllianceName")]
        public string AllianceName { get; set; }

        [BsonElement("AllianceTag")]
        [JsonProperty("AllianceTag")]
        public string AllianceTag { get; set; }

        [BsonElement("FounderId")]
        [JsonProperty("FounderId")]
        public string FounderId { get; set; }

        [BsonElement("FounderName")]
        [JsonProperty("FounderName")]
        public string FounderName { get; set; }

        [BsonElement("Founded")]
        [JsonProperty("Founded")]
        public string Founded { get; set; }

        [BsonElement("Guilds")]
        [JsonProperty("Guilds")]
        public IList<AllianceGuild> Guilds { get; set; }

        [BsonElement("NumPlayers")]
        [JsonProperty("NumPlayers")]
        public int NumPlayers { get; set; }

        [BsonElement("UpdatedAt")] public DateTime UpdatedAt { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime CreatedAt { get; set; }
    }

    public class AllianceGuild
    {
        [BsonElement("Id")]
        [JsonProperty("Id")]
        public string Id { get; set; }

        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }
    }
}