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
        private readonly MercadoService _mercadoService;

        public TimeService(HttpClient httpClient, IJSRuntime jSRuntime, MercadoService mercadoService)
        {
            _httpClient = httpClient;
            _jSRuntime = jSRuntime;
            _mercadoService = mercadoService;
        }
        public async Task<List<TeamCartolaDTO>> SearchTeams(string query, bool details = false)
        {
            var teams = await _httpClient.GetFromJsonAsync<List<TeamCartolaDTO>>($"https://api.ligas.ehtudo.app/webhook/cartola/v1/busca?query={query}");
            if (details)
            {
                if (teams != null && teams[0].Nome != null)
                {
                    await EnrichTeamsWithDetails(teams);
                }
            }

            return teams;
        }

        private async Task EnrichTeamsWithDetails(List<TeamCartolaDTO> teams)
        {
            var mercadoStatus = await _mercadoService.GetMercadoStatus();
            if (!_mercadoService.IsEmManutencao())
            {
                var enrichmentTasks = teams.Select(async team =>
                {
                    try
                    {
                        var teamDetails = await _httpClient.GetFromJsonAsync<TeamCartolaDTO>($"https://bypass.ehtudo.app/https://api.cartola.globo.com/time/id/{team.TimeId}");
                        if (teamDetails != null)
                        {
                            team.PontosCampeonato = teamDetails.PontosCampeonato;
                            team.Patrimonio = teamDetails.Patrimonio;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao buscar detalhes do time {team.TimeId}: {ex.Message}");
                    }
                });

                await Task.WhenAll(enrichmentTasks);
            }
        }

        public async Task<TeamDTO> AddTeam(TeamDTO teamDTO)
        {
            //obter o authToken do localStorage
            var authToken = await _jSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            if (authToken != null)
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", authToken);
                var handler = new JwtSecurityTokenHandler();

                var response = await _httpClient.PostAsJsonAsync("webhook/ligas/v1/time/addOwnTime", teamDTO);
                if (response.IsSuccessStatusCode)
                {
                    var team = await response.Content.ReadFromJsonAsync<TeamDTO>();
                    return team;
                }
            }
            return null;
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

                var response = await _httpClient.GetFromJsonAsync<TimeResponse>($"webhook/ligas/v1/time");

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
