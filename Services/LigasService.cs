using System.Net.Http;
using System.Net.Http.Json;
using CartolaLigas.Providers;

namespace CartolaLigas.Services
{
    public class LigasService
    {
        private readonly HttpClient _httpClient;

        public LigasService(CustomHttpClientProvider httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.ligas.ehtudo.app/");
        }

        public async Task CreateAsync(string userId, string name, string slug)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjb2xsZWN0aW9uSWQiOiJfcGJfdXNlcnNfYXV0aF8iLCJleHAiOjE3NDQwNzA0MDgsImlkIjoiZTM1ODk0azE2azNwN2p0IiwicmVmcmVzaGFibGUiOnRydWUsInR5cGUiOiJhdXRoIn0.cpMyfPkPZy8bkA1VhzK3e2P-2vjO5RaMnF_Tm0cvj9M");

            var response = await _httpClient.PostAsJsonAsync(
            "api/collections/ligas/records",
            new { user_id = userId, name = name, slug = slug });
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
            }
            else
            {
                Console.WriteLine("Erro ao criar liga");
            }
        }
        public async Task<List<Liga>> ListarAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", token);
            var response = await _httpClient.GetFromJsonAsync<ListarLigas>("api/collections/ligas/records");
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
