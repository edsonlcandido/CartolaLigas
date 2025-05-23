﻿using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using CartolaLigas.Models.Cartola;
using CartolaLigas.Providers;
using Microsoft.JSInterop;

namespace CartolaLigas.Services
{
    public class TimeService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jSRuntime;
        private readonly MercadoService _mercadoService;
        private Models.Time? _cachedTeam; // Cache em memória para o time


        public TimeService(HttpClient httpClient, IJSRuntime jSRuntime, MercadoService mercadoService)
        {
            _httpClient = httpClient;
            _jSRuntime = jSRuntime;
            _mercadoService = mercadoService;
        }
        public async Task<List<Models.Cartola.Time>> SearchTeams(string query, bool details = false)
        {
            var teams = await _httpClient.GetFromJsonAsync<List<Models.Cartola.Time>>($"https://api.ligas.ehtudo.app/webhook/cartola/v1/busca?query={query}");
            if (details)
            {
                if (teams != null && teams[0].Nome != null)
                {
                    await EnrichTeamsWithDetails(teams);
                }
            }

            return teams;
        }

        private async Task EnrichTeamsWithDetails(List<Models.Cartola.Time> teams)
        {
            var mercadoStatus = await _mercadoService.GetMercadoStatus();
            if (!_mercadoService.IsEmManutencao())
            {
                var enrichmentTasks = teams.Select(async team =>
                {
                    try
                    {
                        var teamDetails = await _httpClient.GetFromJsonAsync<Models.Cartola.Time>($"https://bypass.ehtudo.app/https://api.cartola.globo.com/time/id/{team.TimeId}");
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

        public async Task<Models.Time> AddTime(Models.Time time)
        {
            // Verificar se o time já existe na base de dados
            var existingTeamResponse = await _httpClient.GetFromJsonAsync<TimeResponse>($"https://api.ligas.ehtudo.app/api/collections/times/records?filter=(cartola_time_id={time.CartolaTimeId})");
            if (existingTeamResponse != null && existingTeamResponse.items != null && existingTeamResponse.items.Count > 0)
            {
                // Retornar o time existente
                return existingTeamResponse.items[0];
            }

            _httpClient.DefaultRequestHeaders.Clear();
            var response = await _httpClient.PostAsJsonAsync("https://api.ligas.ehtudo.app/api/collections/times/records", time);
            if (response.IsSuccessStatusCode)
            {
                var team = await response.Content.ReadFromJsonAsync<Models.Time>();
                List<Object> timeRodadaQueue = new List<Object>();
                await _mercadoService.GetMercadoStatus();
                for (int i = 1; i < _mercadoService.GetRodadaAtual(); i++)
                {
                    var timeRodada = new { rodada = i, time_id = team.Id, status = "aguardando" };
                    await _httpClient.PostAsJsonAsync($"https://api.ligas.ehtudo.app/api/collections/job_queue/records", timeRodada);
                }
                return team;
            }
            return null;
        }
        public async Task<Models.Time> AddOwnTeam(Models.Time time, string? token = null)
        {
            //obter o authToken do localStorage
            var authToken = token ?? await _jSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            var addedTeam = await AddTime(time);
            if (authToken != null || addedTeam != null)
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", authToken);
                var handler = new JwtSecurityTokenHandler();

                var response = await _httpClient.PostAsJsonAsync("https://api.ligas.ehtudo.app/webhook/ligas/v1/time/addOwnTime", addedTeam);
                if (response.IsSuccessStatusCode)
                {
                    var team = await response.Content.ReadFromJsonAsync<Models.Time>();
                    return team;
                }
            }
            return null;
        }
        public async Task RemoveOwnTeam(Models.Time time, string? token = null)
        {
            //obter o authToken do localStorage
            var authToken = token ?? await _jSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            if (authToken != null)
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", authToken);
                var response = await _httpClient.PostAsJsonAsync($"https://api.ligas.ehtudo.app/webhook/ligas/v1/time/removeOwnTime",time);
                if (response.IsSuccessStatusCode)
                {
                    var team = await response.Content.ReadFromJsonAsync<Models.Time>();
                    ClearCache();
                }
                else
                {
                    //return a mensagem de erro
                    var errorResponse = "Não foi possivel desvincular o time para esse usuario";
                }
            }
        }

        public async Task<Models.Time> Time(string? token = null)
        {
            // Verificar se o time já está no cache
            if (_cachedTeam != null)
            {
                return _cachedTeam;
            }

            //obter o authToken do localStorage
            var authToken = token ?? await _jSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            if (authToken != null)
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", authToken);
                var handler = new JwtSecurityTokenHandler();

                var response = await _httpClient.GetFromJsonAsync<TimeResponse>($"https://api.ligas.ehtudo.app/webhook/ligas/v1/time");

                if (response.items.Count != 0)
                {
                    _cachedTeam = response.items[0]; // Armazenar no cache
                    return _cachedTeam;
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
        public void ClearCache()
        {
            _cachedTeam = null; // Limpar o cache quando necessário
        }
        public async Task<Models.Time> GetTime(string timeId)
        {
            try
            {
                var time = await _httpClient.GetFromJsonAsync<Models.Time>($"https://api.ligas.ehtudo.app/api/collections/times/records/{timeId}");
                return time;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar o time com ID {timeId}: {ex.Message}");
                return null;
            }
        }



        private async Task adicionarPontuacaoTime()
        {
            var response = await _httpClient.GetFromJsonAsync<Object>($"https://api.ligas.ehtudo.app/webhook/ligas/v1/time");
        }

        public class ErrorResponse
        {
            public ErrorData Data { get; set; }
            public string Message { get; set; }
            public int Status { get; set; }
        }

        public class ErrorData
        {
            public ErrorDetail CartolaTimeId { get; set; }
        }

        public class ErrorDetail
        {
            public string Code { get; set; }
            public string Message { get; set; }
        }

        public class TimeResponse
        {
            public List<Models.Time>? items { get; set; }
            public int page { get; set; }
            public int perPage { get; set; }
            public int totalItems { get; set; }
            public int totalPages { get; set; }
        }
    }
}
