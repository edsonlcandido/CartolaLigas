using System.Text.Json.Serialization;

namespace CartolaLigas.Models
{
    public class User
    {
        public string collectionId { get; set; }
        public string collectionName { get; set; }
        public string id { get; set; }
        public string email { get; set; }
        [JsonPropertyName("emailVisibility")]
        public bool emailVisibility { get; set; }
        public bool verified { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public string role { get; set; }
        public string created { get; set; }
        public string updated { get; set; }
        [JsonPropertyName("max_leagues")]
        public int maxLeagues { get; set; }
    }
}