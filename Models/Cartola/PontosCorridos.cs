using System.Text.Json.Serialization;

namespace CartolaLigas.Models.Cartola
{
    public class PontosCorridos
    {
        [JsonPropertyName("free")]
        public Free Free { get; set; }

        [JsonPropertyName("pro")]
        public Pro Pro { get; set; }
    }
}