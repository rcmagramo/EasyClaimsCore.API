using System.Net.Http;

namespace EasyClaimsCore.API.Extensions
{
    public class NewHttpClientFactory
    {
        private static readonly HttpClient _httpClient;

        static NewHttpClientFactory()
        {
            _httpClient = new HttpClient();
        }

        public static HttpClient Instance => _httpClient;

       
        public void Dispose()
        {
           
            _httpClient?.Dispose();
        }
    }
}
