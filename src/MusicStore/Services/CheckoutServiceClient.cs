using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Services
{
    public class CheckoutServiceClient : ServiceClientBase
    {
        public CheckoutServiceClient(string checkoutServiceBaseUrl)
            : base(checkoutServiceBaseUrl)
        {
        }

        public async Task<int> PostOrderAsync(Models.Order order, string shoppingCartId)
        {
            string requestUrl = string.Concat(base.BaseServiceUrl, "/", shoppingCartId);
            string orderAsJson = Newtonsoft.Json.JsonConvert.SerializeObject(order);

            var stringContent = new StringContent(orderAsJson, Encoding.UTF8, "application/json");
            var response = await base.HttpClient.PostAsync(requestUrl, stringContent);

            response.EnsureSuccessStatusCode();

            return int.Parse(await response.Content.ReadAsStringAsync());
        }
    }
}
