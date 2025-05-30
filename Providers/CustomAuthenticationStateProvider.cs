using CartolaLigas.Models;
using CartolaLigas.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CartolaLigas.Providers
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private readonly IJSRuntime _jSRuntime;
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        public CustomAuthenticationStateProvider(IJSRuntime jSRuntime, HttpClient httpClient, NavigationManager navigationManager)
        {
            _jSRuntime = jSRuntime;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.ligas.ehtudo.app/");
            _navigationManager = navigationManager;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //obter o authToken do localStorage
            var authToken = await _jSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            if (!string.IsNullOrEmpty(authToken))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(authToken);

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("Authorization", authToken);
                    try
                    {
                        // Obter o UserDTO a partir do id
                        var userResponse = await _httpClient.GetFromJsonAsync<User>($"api/collections/users/records/{userId}");

                        if (userResponse != null)
                        {
                            var claims = jwtToken.Claims.ToList();

                            // Adicionar a claim 'role'
                            claims.Add(new Claim(ClaimTypes.Role, userResponse.role));

                            // Opcional: Adicionar outras claims
                            //if userResponse.name not exists show userResponse.email
                            //if userResponse.name not exists show userResponse.email
                            claims.Add(new Claim(ClaimTypes.Name, userResponse.name == "" ? userResponse.email : userResponse.name));
                            var identity = new ClaimsIdentity(claims, "apiauth_type");
                            var user = new ClaimsPrincipal(identity);
                            NotifyAuthenticationStateChanged(user);
                            return new AuthenticationState(user);
                        }
                        else
                        {
                            await LogoutAndRedirect();
                            return new AuthenticationState(_anonymous);
                        }
                    }
                    catch
                    {
                        await LogoutAndRedirect();
                        return new AuthenticationState(_anonymous);
                    }
                }
                else
                {
                    await LogoutAndRedirect();
                    return new AuthenticationState(_anonymous);
                }
            }
            else
            {
                await LogoutAndRedirect();
                return new AuthenticationState(_anonymous);
            }

        }

        private async Task LogoutAndRedirect()
        {
            // Remover o authToken do localStorage
            await _jSRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");

            // Notificar que o usu�rio n�o est� autenticado
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));

            // Redirecionar para a p�gina inicial
            _navigationManager.NavigateTo("/");
        }

        public void NotifyUserAuthentication(string userName)
        {
            var identity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, userName),
        }, "apiauth_type");

            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async void NotifyUserLogout()
        {
            var user = _anonymous;
            //JSInterop remove authToken of localstorage
            await _jSRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
        private void NotifyAuthenticationStateChanged(ClaimsPrincipal user)
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}

