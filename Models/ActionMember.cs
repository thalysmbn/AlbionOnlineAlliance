namespace AlbionOnline.Models
{
    public class ActionMember
    {
        public string AccountName { get; set; }
        public string AlbionId { get; set; }
        public ulong DiscordId { get; set; }
        public ActionMemberType Type { get; set; }
    }

    public enum ActionMemberType
    {
        IsNotRegistered,
        IsNotInDiscord,
        IsNotInGuild,
        WithoutRole
    }
}
