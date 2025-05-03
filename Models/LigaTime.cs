using System.Text.Json.Serialization;

namespace CartolaLigas.Models
{
    public class LigaTime
    {
        public string CollectionId { get; set; }
        public string CollectionName { get; set; }
        public string Created { get; set; }
        public string Id { get; set; }
        [JsonPropertyName("liga_id")]
        public string LigaId { get; set; }
        [JsonPropertyName("time_id")]
        public string TimeId { get; set; }
        public string Updated { get; set; }
    }
}