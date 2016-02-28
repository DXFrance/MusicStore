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
            string requestUrl = string.Concat(this.BaseServiceUrl, "/checkout");
            return await base.HttpClient.GetStringAsync(requestUrl);
        }

        public async Task<string> GetProductsCatalogServiceUrlAsync()
        {
            string requestUrl = string.Concat(this.BaseServiceUrl, "/products_catalog");
            return await base.HttpClient.GetStringAsync(requestUrl);
        }
    }
}
