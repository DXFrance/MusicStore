using Microsoft.AspNet.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStore.Components
{
    [ViewComponent(Name = "AlbumCover")]
    public class AlbumCoverComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string artist, string album)
        {
            var httpClient = new System.Net.Http.HttpClient();
            string url = string.Format("http://ws.audioscrobbler.com/2.0/?method=album.getinfo&api_key=189c353499a656b83ad16da81d3857c1&artist={0}&album={1}&format=json", artist, album);

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return View("Default", Url.Content("~/Images/placeholder.png"));
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var getCoverResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<GetCoverResponse>(jsonContent);

            if (getCoverResponse != null && getCoverResponse.album != null && getCoverResponse.album.image != null && getCoverResponse.album.image.Any(i => i.size == "large"))
            {
                return View("Default", getCoverResponse.album.image.First(i => i.size == "large").text);
            }

            return View("Default", Url.Content("~/Images/placeholder.png"));
        }
    }

    public class GetCoverResponse
    {
        public Album album { get; set; }
    }

    public class Album
    {
        public Image[] image { get; set; }
    }

    public class Image
    {
        public string text { get; set; }
        public string size { get; set; }
    }
}
