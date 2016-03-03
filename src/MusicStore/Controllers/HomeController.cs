using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Caching.Memory;
using MusicStore.Models;

namespace MusicStore.Controllers
{
    public class HomeController : Controller
    {
        [FromServices]
        public AppSettings AppSettings { get; set; }
        
        [FromServices]
        public MusicStoreContext DbContext { get; set; }

        //
        // GET: /Home/
        public async Task<IActionResult> Index()
        {
            var serviceRegistry = new Services.ServiceRegistry();
            var productsCatalogClient = new Services.ProductsCatalogClient(serviceRegistry.GetProductsCatalogServiceUrl());

            var albums = await productsCatalogClient.GetTopSellingAlbumsAsync(6);
            return View(albums);
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        public IActionResult StatusCodePage()
        {
            return View("~/Views/Shared/StatusCodePage.cshtml");
        }

        public IActionResult AccessDenied()
        {
            return View("~/Views/Shared/AccessDenied.cshtml");
        }
    }
}