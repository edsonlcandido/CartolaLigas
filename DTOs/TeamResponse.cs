using System.Text.Json.Serialization;

namespace CartolaLigas.DTOs
{
    public class TeamCartolaDTO
    {
        [JsonPropertyName("url_escudo_svg")]
        public string UrlEscudoSvg { get; set; }

        [JsonPropertyName("nome_cartola")]
        public string NomeCartola { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("url_escudo_png")]
        public string UrlEscudoPng { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("foto_perfil")]
        public string FotoPerfil { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("facebook_id")]
        public long? FacebookId { get; set; }

        [JsonPropertyName("time_id")]
        public long TimeId { get; set; }

        [JsonPropertyName("assinante")]
        public bool? Assinante { get; set; }

        [JsonPropertyName("pontos_campeonato")]
        public double PontosCampeonato { get; set; }
        [JsonPropertyName("patrimonio")]
        public double Patrimonio { get; set; }
    }

    public class TeamDTO
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
    public class TeamResponse
    {
    }
}
