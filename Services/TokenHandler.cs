using EasyClaimsCore.API.Models.Responses;
using Newtonsoft.Json;

namespace EasyClaimsCore.API.Services
{
    public class TokenHandler : ITokenHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenHandler> _logger;
        private string _jwtToken;

        public TokenHandler(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<TokenHandler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> MakeApiRequestAsync(string pmcc, string certificateId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("EClaimsClient");
                var restBaseUrl = _configuration["PhilHealth:RestBaseUrl"];
                var url = $"{restBaseUrl}PHIC/Claims3.0/getToken";

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("accreditationNo", pmcc);
                httpClient.DefaultRequestHeaders.Add("softwareCertificateId", certificateId);

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content);
                    _jwtToken = tokenResponse.Result.ToString() ?? throw new InvalidOperationException("Token not found in response");
                    return _jwtToken;
                    //return tokenResponse?.token?.ToString() ?? throw new InvalidOperationException("Token not found in response");
                }

                throw new HttpRequestException($"Token request failed with status: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while requesting token for PMCC: {PMCC}", pmcc);
                throw;
            }
        }
    }
}