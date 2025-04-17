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

        public LigasService(HttpClient httpClient, IJSRuntime jSRuntime)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.ligas.ehtudo.app/");
            _jSRuntime = jSRuntime;
        }

        public async Task CreateAsync(string name, string? token = null)
        {
            var authToken = token ?? await _jSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", authToken);

            CreateLeagueRequest createLeagueRequest = new CreateLeagueRequest()
            {
                name = name
            };

            var response = await _httpClient.PostAsJsonAsync<CreateLeagueRequest>("webhook/ligas/v1/liga/", createLeagueRequest);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
            }
            else
            {
                //precisava tenar criar a liga adicionando um numero no final do slug deve tentar
                Console.WriteLine("Erro ao criar liga");
            }
        }
        public async Task<List<Liga>> ListarAsync()
        {
            var authToken = await _jSRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", authToken);

            var response = await _httpClient.GetFromJsonAsync<ListarLigas>("api/collections/ligas/records");
            if (response == null)
            {
                Console.WriteLine("Erro ao listar ligas");
                return new List<Liga>();
            }
            return response.items;
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
