using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using CartolaLigas.DTOs;
using CartolaLigas.Providers;
using Microsoft.JSInterop;

namespace CartolaLigas.Services
{
    public class TimeService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jSRuntime;

        public TimeService(CustomHttpClientProvider httpClient, IJSRuntime jSRuntime)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.ligas.ehtudo.app/");
            _jSRuntime = jSRuntime;
        }
        public async Task<TeamDTO> Time()
        {
            //obter o authToken do localStorage
            var authToken = await _jSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            if (authToken != null)
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", authToken);
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(authToken);

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                var response = await _httpClient.GetFromJsonAsync<TimeResponse>($"webhook/ligas/v1/time?userId={userId}");

                if (response.items.Count != 0)
                {
                    return response.items[0];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public class TimeResponse
        {
            public List<TeamDTO>? items { get; set; }
            public int page { get; set; }
            public int perPage { get; set; }
            public int totalItems { get; set; }
            public int totalPages { get; set; }
        }
    }
}
