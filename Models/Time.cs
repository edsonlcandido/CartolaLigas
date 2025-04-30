using System.Text.Json.Serialization;

namespace CartolaLigas.Models
{
    public class Time
    {
        public string CollectionId { get; set; }
        public string CollectionName { get; set; }
        public string Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        [JsonPropertyName("cartola_time_id")]
        public long CartolaTimeId { get; set; }
        [JsonPropertyName("nome_cartola")]
        public string NomeCartola { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
    }
}
