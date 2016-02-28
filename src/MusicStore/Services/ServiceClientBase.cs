using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MusicStore.Services
{
    public abstract class ServiceClientBase
    {
        protected readonly string BaseServiceUrl;
        protected readonly HttpClient HttpClient;

        public ServiceClientBase(string baseServiceUrl)
        {
            this.BaseServiceUrl = baseServiceUrl;
            this.HttpClient = new HttpClient();
        }
    }
}
