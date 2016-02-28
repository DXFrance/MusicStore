using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MusicStore.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MusicStore.CheckoutService.Controllers
{
    [Route("api/[controller]")]
    public class CheckoutController : Controller
    {
        [FromServices]
        public MusicStoreContext DbContext { get; set; }

        [HttpPost("{shoppingCartId}")]
        public async Task<IActionResult> AddressAndPayment(string shoppingCartId, [FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(400);
            }

            // add the order to the db context
            DbContext.Add(order);

            // update the date to now
            order.OrderDate = DateTime.Now;

            // get the shopping cart from its id and create the order
            var shoppingCart = ShoppingCart.GetCart(DbContext, shoppingCartId);
            await shoppingCart.CreateOrder(order);

            // save the changes
            await DbContext.SaveChangesAsync();

            // return the order id
            return Json(order.OrderId);
        }
    }
}
