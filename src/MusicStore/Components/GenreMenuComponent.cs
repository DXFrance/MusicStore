using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using MusicStore.Models;

namespace MusicStore.Components
{
    [ViewComponent(Name = "GenreMenu")]
    public class GenreMenuComponent : ViewComponent
    {
        public GenreMenuComponent(MusicStoreContext dbContext)
        {
            DbContext = dbContext;
        }

        private MusicStoreContext DbContext { get; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var genres = await GetGenres();

            return View(genres);
        }

        private async Task<List<Genre>> GetGenres()
        {
            var serviceDiscoveryClient = new Services.ServiceDiscoveryClient("");

            string productsCatalogServiceUrl = await serviceDiscoveryClient.GetProductsCatalogServiceUrlAsync();
            var productsCatalogClient = new Services.ProductsCatalogClient(productsCatalogServiceUrl);

            var genres = await productsCatalogClient.GetGenresAsync();
            return genres.ToList();
        }
    }
}