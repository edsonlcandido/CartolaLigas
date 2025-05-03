using System.Net.Http;
using System.Net.Http.Json;
using CartolaLigas.DTOs.Request;
using CartolaLigas.DTOs.Response;
using CartolaLigas.Models;
using CartolaLigas.Providers;
using Microsoft.JSInterop;

namespace CartolaLigas.Services
{
    public class LigasService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jSRuntime;
        private List<Liga>? _cachedLigas; // Cache em memória para as ligas
        private Liga _cachedLiga;
        private TimeService _timeService;
        private List<Models.Time> _times;

        public LigasService(HttpClient httpClient, IJSRuntime jSRuntime, TimeService timeService)
        {
            _httpClient = httpClient;
            _jSRuntime = jSRuntime;
            _timeService = timeService;
        }

        public async Task<Liga> CreateAsync(string name, string? token = null)
        {
            var authToken = token ?? await _jSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", authToken);

            CreateLeagueRequest createLeagueRequest = new CreateLeagueRequest()
            {
                name = name
            };

            var response = await _httpClient.PostAsJsonAsync<CreateLeagueRequest>("https://api.ligas.ehtudo.app/webhook/ligas/v1/liga/", createLeagueRequest);
            if (response.IsSuccessStatusCode)
            {
                //response volta um objeto com os dados da liga criada
                //var content = await response.Content.ReadAsStringAsync();

                var listLiga = response.Content.ReadFromJsonAsync<List<Liga>>();
                if (listLiga.Result.Count > 0)
                {
                    return listLiga.Result.FirstOrDefault();
                }
                else
                {
                    return null;
                }     
            }
            else
            {
                //precisava tenar criar a liga adicionando um numero no final do slug deve tentar
                return null; 
            }
        }
        public async Task<List<Liga>> ListarAsync(string? token = null)
        {
            // Verificar se as ligas já estão no cache
            if (_cachedLigas != null)
            {
                return _cachedLigas;
            }

            var authToken = token ?? await _jSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", authToken);

            var response = await _httpClient.GetFromJsonAsync<ListarLigasResponse>("https://api.ligas.ehtudo.app/webhook/ligas/v1/liga/");
            if (response == null)
            {
                Console.WriteLine("Erro ao listar ligas");
                return new List<Liga>();
            }

            _cachedLigas = response.items; // Armazenar no cache
            return _cachedLigas;

            //if (response.IsSuccessStatusCode)
            //{
            //    var content = await response.Content.ReadAsStringAsync();
            //    Console.WriteLine(content);
            //}
            //else
            //{
            //    Console.WriteLine("Erro ao listar ligas");
            //}
        }
        public void ClearCache()
        {
            _cachedLigas = null; // Limpar o cache quando necessário
        }

        public async Task<List<Liga>> DeleteAsync(Liga liga, string? token = null)
        {
            var authToken = token ?? await _jSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", authToken);

            var response = await _httpClient.DeleteAsync($"https://api.ligas.ehtudo.app/api/collections/ligas/records/{liga.id}");

            ClearCache();

            return await ListarAsync();
        }
        public async Task<Liga> LigaAsync(string id)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            var response = await _httpClient.GetFromJsonAsync<Liga>($"https://api.ligas.ehtudo.app/api/collections/ligas/records/{id}");
            _cachedLiga = response;
            return _cachedLiga;
        }
        public async Task<List<Models.Time>> TeamsOnLeague()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            throw new NotImplementedException("O método TeamsOnLeague ainda não foi implementado.");
        }


        public async Task<bool> AddTeamToLeague(Models.Cartola.Time cartolaTime, string leagueId)
        {
            // Converter Cartola.Time para Models.Time  
            var time = new Models.Time
            {
                Name = cartolaTime.Nome,
                NomeCartola = cartolaTime.NomeCartola,
                CartolaTimeId = cartolaTime.TimeId
            };

            // Adicionar o time usando TimeService  
            var addedTime = await _timeService.AddTime(time);
            if (addedTime == null)
            {
                Console.WriteLine("Erro ao adicionar o time.");
                return false;
            }

            // Vincular o time à liga  
            var payload = new
            {
                liga_id = leagueId,
                time_id = addedTime.Id
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.ligas.ehtudo.app/api/collections/ligas_times/records", payload);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Time vinculado à liga com sucesso.");
                return true;
            }

            Console.WriteLine("Erro ao vincular o time à liga.");
            return false;
        }
    }
}
