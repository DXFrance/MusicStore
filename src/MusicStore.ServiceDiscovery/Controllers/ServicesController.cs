using Microsoft.AspNet.Mvc;

namespace MusicStore.ServiceDiscovery.Controllers
{
    [Route("api/[controller]")]
    public class ServicesController : Controller
    {
        // move these values into database instead of hardcoded
        private static string CheckoutServiceUrl = "http://localhost:5004";
        private static string ProductsCatalogServiceUrl = "http://localhost:5003";

        /// <summary>
        /// Gets the URL of a service by its id
        /// </summary>
        /// <param name="serviceId">The service identifier</param>
        /// <returns>The URL of the service</returns>
        [HttpGet("{serviceId}")]
        public IActionResult Get(string serviceId)
        {
            if(serviceId == "products_catalog")
            {
                return new ContentResult()
                {
                    Content = ProductsCatalogServiceUrl,
                    StatusCode = 200
                };
            }
            else if(serviceId == "checkout")
            {
                return new ContentResult()
                {
                    Content = CheckoutServiceUrl,
                    StatusCode = 200
                };
            }
            else
            {
                return new HttpStatusCodeResult(404);
            }
        }

        /// <summary>
        /// Registers the URL of a given service
        /// </summary>
        /// <param name="serviceId">The service identifier</param>
        /// <param name="value">The service URL</param>
        [HttpPut("{serviceId}")]
        public IActionResult Put(string serviceId, [FromBody]string value)
        {
            if(serviceId == "products_catalog")
            {
                ServicesController.ProductsCatalogServiceUrl = value;
            }
            else if(serviceId == "checkout")
            {
                ServicesController.CheckoutServiceUrl = value;
            }
            else
            {
                return new EmptyResult();
            }

            return new HttpOkResult();
        }
    }
}
