using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicStore.Services
{
    public class ProductsCatalogClient : ServiceClientBase
    {
        public ProductsCatalogClient(string productsCatalogServiceBaseUrl)
            : base(productsCatalogServiceBaseUrl)
        {
        }

        public async Task<IList<Models.Genre>> GetGenresAsync()
        {
            string requestUrl = string.Concat(this.BaseServiceUrl, "/genres");
            var response = await this.HttpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var genresAsJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Models.Genre>>(genresAsJson);
        }

        public async Task<Models.Genre> BrowseByGenreAsync(string genre)
        {
            string requestUrl = string.Concat(this.BaseServiceUrl, "/browse/", genre);
            var response = await this.HttpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var genreWithAlbumbsAsJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Models.Genre>(genreWithAlbumbsAsJson);
        }

        public async Task<Models.Album> GetAlbumDetails(int id)
        {
            string requestUrl = string.Concat(this.BaseServiceUrl, "/details/", id.ToString());
            var response = await this.HttpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var albumAsJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Models.Album>(albumAsJson);
        }

        public async Task<List<Models.Album>> GetTopSellingAlbumsAsync(int count)
        {
            string requestUrl = string.Concat(this.BaseServiceUrl, "/top/", count.ToString());
            var response = await this.HttpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var topAlbumsAsJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Models.Album>>(topAlbumsAsJson);
        }
    }
}
