using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Caching.Memory;
using MusicStore.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.PlatformAbstractions;

namespace MusicStore.Controllers
{
    public class StoreController : Controller
    {
        [FromServices]
        public AppSettings AppSettings { get; set; }
        
        private ILogger _logger;
        private IApplicationEnvironment _appEnvironment;

        public StoreController(ILoggerFactory loggerFactory, IApplicationEnvironment appEnvironment)
        {
            _logger = loggerFactory.CreateLogger<StoreController>();
            _appEnvironment = appEnvironment;
        }

        //
        // GET: /Store/
        public async Task<IActionResult> Index()
        {
            var serviceRegistry = new Services.ServiceRegistry();

            string productsCatalogServiceUrl = serviceRegistry.GetProductsCatalogServiceUrl();
            var productsCatalogClient = new Services.ProductsCatalogClient(productsCatalogServiceUrl);

            var genres = await productsCatalogClient.GetGenresAsync();
            return View(genres);
        }

        //
        // GET: /Store/Browse?genre=Disco
        public async Task<IActionResult> Browse(string genre)
        {
            var serviceRegistry = new Services.ServiceRegistry();

            string productsCatalogServiceUrl = serviceRegistry.GetProductsCatalogServiceUrl();
            var productsCatalogClient = new Services.ProductsCatalogClient(productsCatalogServiceUrl);

            var genreModel = await productsCatalogClient.BrowseByGenreAsync(genre);

            return View(genreModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var serviceRegistry = new Services.ServiceRegistry();

            string productsCatalogServiceUrl = serviceRegistry.GetProductsCatalogServiceUrl();
            var productsCatalogClient = new Services.ProductsCatalogClient(productsCatalogServiceUrl);

            _logger.LogInformation("Service URL : {0}", productsCatalogServiceUrl);

            var album = await productsCatalogClient.GetAlbumDetails(id);

            return View(album);
        }

        public async Task<IActionResult> GetAlbumArt(string artist, string album)
        {
            var httpClient = new System.Net.Http.HttpClient();
            string url = string.Format("http://ws.audioscrobbler.com/2.0/?method=album.getinfo&api_key=189c353499a656b83ad16da81d3857c1&artist={0}&album={1}&format=json", System.Uri.EscapeUriString(artist), System.Uri.EscapeUriString(album));

            var response = await httpClient.GetAsync(url);

            string albumUrl = string.Empty;

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var getCoverResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<GetCoverResponse>(jsonContent);

                if (getCoverResponse != null && getCoverResponse.album != null && getCoverResponse.album.image != null && getCoverResponse.album.image.Any(i => i.size == "large" && !string.IsNullOrEmpty(i.text)))
                {
                    albumUrl = getCoverResponse.album.image.First(i => i.size == "large").text;
                }
            }

            if (string.IsNullOrEmpty(albumUrl))
            {
                string defaultPath = System.IO.Path.Combine(_appEnvironment.ApplicationBasePath, "wwwroot/Images/placeholder.png");
                byte[] defaultArt = System.IO.File.ReadAllBytes(defaultPath);

                return File(defaultPath, "image/png");
            }

            var artContent = await httpClient.GetByteArrayAsync(albumUrl);
            return File(artContent, "image/jpeg");
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
            [Newtonsoft.Json.JsonProperty("#text")]
            public string text { get; set; }
            public string size { get; set; }
        }
    }
}