using CartolaLigas.Providers;
using System.Net.Http;
using System.Net.Http.Json;

namespace CartolaLigas.Services
{
    public class MercadoService
    {
        private readonly CustomHttpClientProvider _httpClient;
        private MercadoResponse mercadoResponse;
        public MercadoService(CustomHttpClientProvider httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://bypass.ehtudo.app/");
        }

        public async Task<int> Status()
        {
            var MercadoResponse = await _httpClient.GetFromJsonAsync<MercadoResponse>("https://api.cartola.globo.com/mercado/status");
            if (mercadoResponse != null)
            {
                
                if (MercadoResponse != null)
                {
                    mercadoResponse = MercadoResponse;
                    return MercadoResponse.StatusMercado;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

    }
}
