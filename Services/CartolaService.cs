using System.Net.Http;
using System.Net.Http.Json;
using CartolaLigas.Providers;

namespace CartolaLigas.Services
{
    public class CartolaService
    {
        private readonly CustomHttpClientProvider _httpClient;

        public CartolaService(CustomHttpClientProvider httpClient) 
        {
            _httpClient = httpClient;
        }

        public async Task<Object> GetMercado()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.BaseAddress = new Uri("https://api.cartola.globo.com/atletas/");
            var response = await _httpClient.GetFromJsonAsync<Object>("mercado/");
            return response;
        }
    }
}
