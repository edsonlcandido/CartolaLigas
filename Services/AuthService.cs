using System.IdentityModel.Tokens.Jwt;
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
        private UserDTO _cachedUserDTO;

        public AuthService(HttpClient httpClient, IJSRuntime jSRuntime)
        {
            _httpClient = httpClient;
            //_httpClient.BaseAddress = new Uri("https://api.ligas.ehtudo.app/");
            _jSRuntime = jSRuntime;
        }

        public async Task<bool> Login(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "https://api.ligas.ehtudo.app/api/collections/users/auth-with-password", 
                new {identity = username, password = password });
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                _cachedUserDTO = authResponse.record;
                await _jSRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", authResponse.token);
                return true;
            }
            return false;
        }
        public async Task<UserDTO?> UserView(string token, string id)
        {
            //obter o _cachedUserDTO se não for null
            if (_cachedUserDTO != null)
            {
                return _cachedUserDTO;
            }
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", token);
            var response = await _httpClient.GetFromJsonAsync<UserDTO>($"https://api.ligas.ehtudo.app/api/collections/users/records/{id}");
            if (response.id != null)
            {
                _cachedUserDTO = response;
                return _cachedUserDTO;
            }
            else
            {
                return null;
            }
        }

        public async Task<int> MaxLeagues()
        {
            if (_cachedUserDTO != null)
            {
                return _cachedUserDTO.maxLeagues;
            }

            var token = await _jSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            _cachedUserDTO = await UserView(token, userId);

            return _cachedUserDTO.maxLeagues;
        }

        public async Task<string> GetToken(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "https://api.ligas.ehtudo.app/api/collections/users/auth-with-password",
                new { identity = username, password = password });
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return authResponse.token;
            }
             return string.Empty;
        }


        public async Task Logout()
        {
            await _httpClient.PostAsync("https://api.ligas.ehtudo.app/api/auth/logout", null);
        }
    }
}
