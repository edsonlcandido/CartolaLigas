using System.Net.Http.Json;
using CartolaLigas.Providers;

namespace CartolaLigas.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(CustomHttpClientProvider httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.ligas.ehtudo.app/");
        }

        public async Task<bool> Login(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/collections/users/auth-with-password", 
                new {identity = username, password = password });
            return response.IsSuccessStatusCode;
        }

        public async Task Logout()
        {
            await _httpClient.PostAsync("api/auth/logout", null);
        }
    }
}
