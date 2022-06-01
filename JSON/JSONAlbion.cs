using AlbionOnline.Models;
using Newtonsoft.Json;

namespace AlbionOnline.JSON
{
    public class JSONAlbion : IJSONAlbion
    {
        public async Task<PlayerModel> GetPlayerAsync(string id)
        {
            var data = await GetAsync($"https://gameinfo.albiononline.com/api/gameinfo/players/{id}");
            return JsonConvert.DeserializeObject<PlayerModel>(data);
        }

        public async Task<SearchModel> Search(string text)
        {
            var data = await GetAsync($"https://gameinfo.albiononline.com/api/gameinfo/search?q={text}");
            return JsonConvert.DeserializeObject<SearchModel>(data);
        }

        public async Task<AllianceModel> GetAlliance(string id)
        {
            var data = await GetAsync($"https://gameinfo.albiononline.com/api/gameinfo/alliances/{id}");
            return JsonConvert.DeserializeObject<AllianceModel>(data);
        }

        public async Task<GuildModel> GetGuild(string id)
        {
            var data = await GetAsync($"https://gameinfo.albiononline.com/api/gameinfo/guilds/{id}");
            return JsonConvert.DeserializeObject<GuildModel>(data);
        }

        public async Task<IList<PlayerModel>> GetGuildMembers(string id)
        {
            var data = await GetAsync($"https://gameinfo.albiononline.com/api/gameinfo/guilds/{id}/members");
            return JsonConvert.DeserializeObject<IList<PlayerModel>>(data);
        }

        private async Task<string> GetAsync(string url)
        {
            using (var test = new HttpClient())
                return await test.GetStringAsync(url);
        }
    }
}
