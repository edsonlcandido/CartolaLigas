using System.Text.Json.Serialization;

namespace CartolaLigas.Models.Cartola
{
    public class LimitesCompeticoes
    {
        [JsonPropertyName("pontos_corridos")]
        public PontosCorridos PontosCorridos { get; set; }
    }
}