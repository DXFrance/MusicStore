using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStore.Services
{
    public class ServiceDiscoveryClient : ServiceClientBase
    {
        public ServiceDiscoveryClient(string serviceDiscoveryBaseApiUrl)
            : base(serviceDiscoveryBaseApiUrl)
        {
        }

        public async Task<string> GetCheckoutServiceUrlAsync()
        {
            return await Task.FromResult("http://checkout.local:5004/api/checkout");
        }

        public async Task<string> GetProductsCatalogServiceUrlAsync()
        {
            return await Task.FromResult("http://catalog:5003/api/productsCatalog");
        }
    }
}
