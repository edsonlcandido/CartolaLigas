using System.Text.Json.Serialization;

namespace CartolaLigas.Models.Cartola
{
    public class LimitesCompeticao
    {
        [JsonPropertyName("total_confronto_pro")]
        public int TotalConfrontoPro { get; set; }

        [JsonPropertyName("total_confronto_free")]
        public int TotalConfrontoFree { get; set; }

        [JsonPropertyName("criacao_confronto_pro")]
        public int CriacaoConfrontoPro { get; set; }

        [JsonPropertyName("criacao_confronto_free")]
        public int CriacaoConfrontoFree { get; set; }
    }
}