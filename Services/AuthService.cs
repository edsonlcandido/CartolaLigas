using System.Net.Http.Json;
using CartolaLigas.DTOs;
using CartolaLigas.Providers;
using Microsoft.JSInterop;

namespace CartolaLigas.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jSRuntime;

        public AuthService(CustomHttpClientProvider httpClient, IJSRuntime jSRuntime)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.ligas.ehtudo.app/");
            _jSRuntime = jSRuntime;
        }

        public async Task<bool> Login(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/collections/users/auth-with-password", 
                new {identity = username, password = password });
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                await _jSRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", authResponse.token);
                return true;
            }
            return false;
        }
        public async Task<UserDTO?> UserView(string token, string id)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", token);
            var response = await _httpClient.GetFromJsonAsync<UserDTO>($"api/collections/users/records/{id}");
            if (response.id != null)
            {
                return response;
            }
            else
            {
                return null;
            }
        }


        public async Task Logout()
        {
            await _httpClient.PostAsync("api/auth/logout", null);
        }
    }
}
