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
        }

        public async Task<bool> Login(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "https://eh-tudo-api-ligas.aiyfgd.easypanel.host/api/collections/users/auth-with-password", 
                new {identity = username, password = password });
            return response.IsSuccessStatusCode;
        }

        public async Task Logout()
        {
            await _httpClient.PostAsync("api/auth/logout", null);
        }
    }
}
