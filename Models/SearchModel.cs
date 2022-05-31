using System.Collections.Generic;
using Newtonsoft.Json;

namespace AlbionOnline.Models
{
    public class SearchModel
    {
        [JsonProperty("Guilds")] public IList<SearchModelGuilds> Guilds { get; set; }

        [JsonProperty("Players")] public IList<SearchModelPlayers> Players { get; set; }
    }

    public class SearchModelGuilds
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string AllianceId { get; set; }

        public string AllianceName { get; set; }
    }

    public class SearchModelPlayers
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string GuildId { get; set; }

        public string GuildName { get; set; }

        public string AllianceId { get; set; }

        public string AllianceName { get; set; }
    }
}