using AlbionOnline.Models;

namespace AlbionOnline.JSON
{
    public interface IJSONAlbion
    {
        Task<PlayerModel> GetPlayerAsync(string id);

        Task<SearchModel> Search(string text);

        Task<AllianceModel> GetAlliance(string id);

        Task<GuildModel> GetGuild(string id);

        Task<IList<PlayerModel>> GetGuildMembers(string id);
    }
}
