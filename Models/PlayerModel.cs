using AlbionOnline.Services.Mongo;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace AlbionOnline.Models
{
    [BsonCollection("player")]
    public class PlayerModel : Document
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string GuildName { get; set; }

        public string GuildId { get; set; }

        public string AllianceName { get; set; }

        public string AllianceId { get; set; }

        public string AllianceTag { get; set; }

        public string Avatar { get; set; }

        public string AvatarRing { get; set; }

        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? AverageItemPower { get; set; } = null;

        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? DamageDone { get; set; } = null;

        public long DeathFame { get; set; }

        public long KillFame { get; set; }

        public double FameRatio { get; set; }

        public DateTime UpdatedAt { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public long LastEventId { get; set; } = 0;

        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime CreatedAt { get; set; }

        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerEquipment? Equipment { get; set; } = null;

        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerLifetimeStatistic? LifetimeStatistics { get; set; }

        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? SupportHealingDone { get; set; } = null;
    }

    public class PlayerEquipment
    {
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerEquipmentData? Armor { get; set; }

        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerEquipmentData? Bag { get; set; }

        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerEquipmentData? Cape { get; set; }

        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerEquipmentData? Food { get; set; }

        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerEquipmentData? Head { get; set; }

        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerEquipmentData? MainHand { get; set; }

        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerEquipmentData? Mount { get; set; }

        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerEquipmentData? OffHand { get; set; }

        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerEquipmentData? Potion { get; set; }

        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerEquipmentData? Shoes { get; set; }
    }

    public class PlayerEquipmentData
    {
        [JsonProperty("Count")] public int Count { get; set; }

        [JsonProperty("Quality")] public int Quality { get; set; }

        [JsonProperty("Type")] public string Type { get; set; }
    }

    public class PlayerLifetimeStatistic
    {
        [JsonProperty("CrystalLeague")] public long CrystalLeague { get; set; }

        [JsonProperty("FarmingFame")] public long FarmingFame { get; set; }

        [JsonProperty("FishingFame")] public long FishingFame { get; set; }

        [JsonProperty("Timestamp")] public string Timestamp { get; set; }

        [JsonProperty("PvE")] public PlayerLifetimeStatisticPvE PvE { get; set; }

        [JsonProperty("Gathering")] public PlayerLifetimeStatisticGathering Gathering { get; set; }

        [JsonProperty("Crafting")] public PlayerLifetimeStatisticCrafting Crafting { get; set; }
    }

    public class PlayerLifetimeStatisticGathering
    {
        [JsonProperty("All")] public PlayerLifetimeStatisticGatheringData All { get; set; }

        [JsonProperty("Fiber")] public PlayerLifetimeStatisticGatheringData Fiber { get; set; }

        [JsonProperty("Hide")] public PlayerLifetimeStatisticGatheringData Hide { get; set; }

        [JsonProperty("Ore")] public PlayerLifetimeStatisticGatheringData Ore { get; set; }

        [JsonProperty("Rock")] public PlayerLifetimeStatisticGatheringData Rock { get; set; }

        [JsonProperty("Wood")] public PlayerLifetimeStatisticGatheringData Wood { get; set; }
    }

    public class PlayerLifetimeStatisticGatheringData
    {
        [JsonProperty("Avalon")] public long Avalon { get; set; }

        [JsonProperty("Outlands")] public long Outlands { get; set; }

        [JsonProperty("Royal")] public long Royal { get; set; }

        [JsonProperty("Total")] public long Total { get; set; }
    }

    public class PlayerLifetimeStatisticCrafting
    {
        [JsonProperty("Avalon")] public long Avalon { get; set; }

        [JsonProperty("Outlands")] public long Outlands { get; set; }

        [JsonProperty("Royal")] public long Royal { get; set; }

        [JsonProperty("Total")] public long Total { get; set; }
    }

    public class PlayerLifetimeStatisticPvE
    {
        [JsonProperty("Total")] public long Total { get; set; }

        [JsonProperty("Royal")] public long Royal { get; set; }

        [JsonProperty("Outlands")] public long Outlands { get; set; }

        [JsonProperty("Avalon")] public long Avalon { get; set; }

        [JsonProperty("Hellgate")] public long Hellgate { get; set; }

        [JsonProperty("CorruptedDungeon")] public long CorruptedDungeon { get; set; }
    }
}