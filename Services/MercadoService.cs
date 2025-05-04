using CartolaLigas.Providers;
using System.Net.Http;
using System.Net.Http.Json;

namespace CartolaLigas.Services
{
    public class MercadoService
    {
        private readonly HttpClient _httpClient;
        private MercadoResponse mercadoResponse;
        public MercadoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<MercadoResponse> GetMercadoStatus()
        {
            mercadoResponse = await _httpClient.GetFromJsonAsync<MercadoResponse>("https://bypass.ehtudo.app/https://api.cartola.globo.com/mercado/status");
            return mercadoResponse;
        }

        public bool IsEmManutencao()
        {
            //return true;
            return mercadoResponse?.StatusMercado == 4;
        }
        
        public int GetRodadaAtual()
        {
            return mercadoResponse?.RodadaAtual ?? 0;
        }
    }
}
