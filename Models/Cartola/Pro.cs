using System.Text.Json.Serialization;

namespace CartolaLigas.Models.Cartola
{
    public class Pro
    {
        [JsonPropertyName("criacao")]
        public int Criacao { get; set; }

        [JsonPropertyName("participacao")]
        public int Participacao { get; set; }
    }
}