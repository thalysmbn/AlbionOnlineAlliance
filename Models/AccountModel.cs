using AlbionOnline.Services.Mongo;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace AlbionOnline.Models
{
    [BsonCollection("accounts")]
    public class AccountModel : Document
    {
        [BsonElement("discordId")]
        [JsonProperty("DiscordId")]
        public string DiscordId { get; set; }

        [BsonElement("createdAt")]
        [JsonProperty("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("name")]
        [JsonProperty("Name")]
        public string Name { get; set; }

        [BsonElement("avatar")]
        [JsonProperty("Avatar")]
        public string Avatar { get; set; }
    }
}