using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using MusicStore.Models;
using System.Net.Http;

namespace MusicStore.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private const string PromoCode = "FREE";

        [FromServices]
        public MusicStoreContext DbContext { get; set; }

        [FromServices]
        public AppSettings AppSettings { get; set; }

        //
        // GET: /Checkout/
        public IActionResult AddressAndPayment()
        {
            return View();
        }

        //
        // POST: /Checkout/AddressAndPayment

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddressAndPayment([FromForm] Order order, CancellationToken requestAborted)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            var formCollection = await HttpContext.Request.ReadFormAsync();

            try
            {
                if (string.Equals(formCollection["PromoCode"].FirstOrDefault(), PromoCode,
                    StringComparison.OrdinalIgnoreCase) == false)
                {
                    return View(order);
                }
                else
                {
                    order.Username = HttpContext.User.GetUserName();

                    // todo : next step is to move shopping cart into another micro service
                    var cart = ShoppingCart.GetCart(DbContext, HttpContext);
                    var cartId = cart.ShoppingCartId;

                    // get the checkout service URL through service discovery
                    var serviceDiscoveryClient = new Services.ServiceDiscoveryClient(AppSettings.ServiceDiscoveryBaseUrl);
                    string checkoutServiceUrl = await serviceDiscoveryClient.GetCheckoutServiceUrlAsync();

                    // post the order to checkout service for the shopping cart id
                    var checkoutService = new Services.CheckoutServiceClient(checkoutServiceUrl);
                    order.OrderId = await checkoutService.PostOrderAsync(order, cartId);
                    
                    return RedirectToAction("Complete", new { id = order.OrderId });
                }
            }
            catch
            {
                //Invalid - redisplay with errors
                return View(order);
            }
        }

        //
        // GET: /Checkout/Complete

        public async Task<IActionResult> Complete(int id)
        {
            // Validate customer owns this order
            bool isValid = await DbContext.Orders.AnyAsync(
                o => o.OrderId == id &&
                o.Username == HttpContext.User.GetUserName());

            if (isValid)
            {
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }
    }
}