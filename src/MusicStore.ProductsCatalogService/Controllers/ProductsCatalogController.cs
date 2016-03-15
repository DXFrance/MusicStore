using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Query;
using Microsoft.Extensions.Caching.Memory;
using MusicStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStore.ProductsCatalogService.Controllers
{
    [Route("api/[controller]")]
    public class ProductsCatalogController : Controller
    {
        [FromServices]
        public MusicStoreContext DbContext { get; set; }

        [FromServices]
        public IMemoryCache Cache { get; set; }

        [HttpGet("genres")]
        public async Task<IActionResult> GetGenresAsync()
        {
            var genres = await DbContext.Genres.ToListAsync();
            return Json(genres);
        }

        [HttpGet("browse/{genre}")]
        public async Task<IActionResult> BrowseByGenreAsync(string genre)
        {
            // Retrieve Genre genre and its Associated associated Albums albums from database
            var genreModel = await DbContext.Genres
                .Include(g => g.Albums)
                .Where(g => g.Name == genre)
                .FirstOrDefaultAsync();

            if (genreModel == null)
            {
                return HttpNotFound();
            }

            // load artists
            var artistItds = genreModel.Albums.Select(a => a.ArtistId).Distinct();
            var artists = await DbContext.Artists.Where(a => artistItds.Contains(a.ArtistId)).ToListAsync();

            return Json(genreModel);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var cacheKey = string.Format("album_{0}", id);
            Album album;
            if (!Cache.TryGetValue(cacheKey, out album))
            {
                album = await DbContext.Albums
                                .Where(a => a.AlbumId == id)
                                .Include(a => a.Artist)
                                .Include(a => a.Genre)
                                .FirstOrDefaultAsync();

                if (album != null)
                {
                    //Remove it from cache if not retrieved in last 10 minutes
                    Cache.Set(
                        cacheKey,
                        album,
                        new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                }
            }

            if (album == null)
            {
                return HttpNotFound();
            }

            return Json(album);
        }

        [HttpGet("top/{count}")]
        public async Task<IActionResult> GetTopSellingAlbumsAsync(int count)
        {
            var topAlbums = await DbContext.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .OrderByDescending(a => a.OrderDetails.Count())
                .Take(count)
                .ToListAsync();

            return Json(topAlbums);
        }
    }
}
