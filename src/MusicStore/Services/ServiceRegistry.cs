using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStore.Services
{
    public class ServiceRegistry
    {
        public string GetCheckoutServiceUrl()
        {
            return "http://checkout:5004/api/checkout";
        }

        public string GetProductsCatalogServiceUrl()
        {
            return "http://catalog:5003/api/productsCatalog";
        }
    }
}
