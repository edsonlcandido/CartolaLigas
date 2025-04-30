namespace CartolaLigas.Models.Cartola;
using System.Text.Json.Serialization;

public class Fechamento
{
    [JsonPropertyName("dia")]
    public int Dia { get; set; }

    [JsonPropertyName("mes")]
    public int Mes { get; set; }

    [JsonPropertyName("ano")]
    public int Ano { get; set; }

    [JsonPropertyName("hora")]
    public int Hora { get; set; }

    [JsonPropertyName("minuto")]
    public int Minuto { get; set; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }
}
