using System.Net.Http;
using System.Net.Http.Json;
using CartolaLigas.Extensions;
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

        public LigasService(HttpClient httpClient, IJSRuntime jSRuntime)
        {
            _httpClient = httpClient;
            _jSRuntime = jSRuntime;
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

            var response = await _httpClient.GetFromJsonAsync<ListarLigas>("https://api.ligas.ehtudo.app/webhook/ligas/v1/liga/");
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
    }

        

    internal class CreateLeagueRequest
    {
        public string name { get; set; }

        public string slug { get
            {
                return name.ToSlug();
            }
        }
    }

    //"page": 1,
    //"perPage": 30,
    //"totalPages": 1,
    //"totalItems": 2,
    //"items": [
    //  {
    //    "collectionId": "pbc_4026191094",
    //    "collectionName": "ligas",
    //    "id": "test",
    //    "user_id": "RELATION_RECORD_ID",
    //    "name": "test",
    //    "slug": "test",
    //    "created": "2022-01-01 10:00:00.123Z",
    //    "updated": "2022-01-01 10:00:00.123Z"
    //  },
    //  {
    //  "collectionId": "pbc_4026191094",
    //    "collectionName": "ligas",
    //    "id": "[object Object]2",
    //    "user_id": "RELATION_RECORD_ID",
    //    "name": "test",
    //    "slug": "test",
    //    "created": "2022-01-01 10:00:00.123Z",
    //    "updated": "2022-01-01 10:00:00.123Z"
    //  }
    //]
    public class ListarLigas
    {
        public int page { get; set; }
        public int perPage { get; set; }
        public int totalPages { get; set; }
        public int totalItems { get; set; }
        public List<Liga> items { get; set; }
    }
    public class Liga
    {
        public string collectionId { get; set; }
        public string collectionName { get; set; }
        public string id { get; set; }
        public string user_id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string created { get; set; }
        public string updated { get; set; }
    }
}
