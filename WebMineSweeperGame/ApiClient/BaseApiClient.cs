using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebMineSweeperGame.ApiClient
{
    public class BaseApiClient
    {
        private readonly string SUBSCRIPTION_HEADER_NAME = "Ocp-Apim-Subscription-Key";
        private readonly HttpClient _client;

        public BaseApiClient(IConfiguration configuration) 
        {
            var apiBaseUrl = configuration.GetSection("ApiClient").GetValue<string>("WebAPIBaseUrl");
            _client = new HttpClient();
            _client.BaseAddress = new Uri(apiBaseUrl);
            var apimSubscripKey = configuration.GetSection("ApiClient").GetValue<string>("SubscriptionHeader");
            
            if (!String.IsNullOrEmpty(apimSubscripKey))
                _client.DefaultRequestHeaders.Add(SUBSCRIPTION_HEADER_NAME, apimSubscripKey);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string url, Object data = null) 
        {
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = httpMethod;
            httpRequestMessage.RequestUri = new Uri(url, UriKind.Relative);

            if (data != null)
            {
                var jsonData = JsonConvert.SerializeObject(data);
                HttpContent httpContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
                switch (httpMethod.Method)
                {
                    case "POST":
                    case "PUT":
                        httpRequestMessage.Content = httpContent;
                        break;
                }
            }

            return await _client.SendAsync(httpRequestMessage);
        }
    }
}
