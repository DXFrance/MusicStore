using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Caching.Memory;
using MusicStore.Models;
using Microsoft.Extensions.Logging;

namespace MusicStore.Controllers
{
    public class StoreController : Controller
    {
        [FromServices]
        public AppSettings AppSettings { get; set; }
        
        private ILogger _logger;

        public StoreController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<StoreController>();
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
    }
}