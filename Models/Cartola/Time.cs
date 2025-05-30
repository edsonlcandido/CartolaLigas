﻿using System.Text.Json.Serialization;

namespace CartolaLigas.Models.Cartola
{
    public class Time
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
}
