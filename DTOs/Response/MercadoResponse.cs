using System.Text.Json.Serialization;
using CartolaLigas.Models.Cartola;

public class MercadoResponse
{
    [JsonPropertyName("rodada_atual")]
    public int RodadaAtual { get; set; }

    [JsonPropertyName("status_mercado")]
    public int StatusMercado { get; set; }

    [JsonPropertyName("esquema_default_id")]
    public int EsquemaDefaultId { get; set; }

    [JsonPropertyName("cartoleta_inicial")]
    public int CartoletaInicial { get; set; }

    [JsonPropertyName("max_ligas_free")]
    public int MaxLigasFree { get; set; }

    [JsonPropertyName("max_ligas_pro")]
    public int MaxLigasPro { get; set; }

    [JsonPropertyName("max_ligas_matamata_free")]
    public int MaxLigasMatamataFree { get; set; }

    [JsonPropertyName("max_criar_ligas_matamata_free")]
    public int MaxCriarLigasMatamataFree { get; set; }

    [JsonPropertyName("max_ligas_matamata_pro")]
    public int MaxLigasMatamataPro { get; set; }

    [JsonPropertyName("max_ligas_patrocinadas_free")]
    public int MaxLigasPatrocinadasFree { get; set; }

    [JsonPropertyName("max_ligas_patrocinadas_pro_num")]
    public int MaxLigasPatrocinadasProNum { get; set; }

    [JsonPropertyName("max_atletas_favoritos_free")]
    public int MaxAtletasFavoritosFree { get; set; }

    [JsonPropertyName("max_atletas_favoritos_pro")]
    public int MaxAtletasFavoritosPro { get; set; }

    [JsonPropertyName("game_over")]
    public bool GameOver { get; set; }

    [JsonPropertyName("temporada")]
    public int Temporada { get; set; }

    [JsonPropertyName("reativar")]
    public bool Reativar { get; set; }

    [JsonPropertyName("exibe_sorteio_pro")]
    public bool ExibeSorteioPro { get; set; }

    [JsonPropertyName("fechamento")]
    public Fechamento Fechamento { get; set; }

    [JsonPropertyName("limites_competicao")]
    public LimitesCompeticao LimitesCompeticao { get; set; }

    [JsonPropertyName("times_escalados")]
    public int TimesEscalados { get; set; }

    [JsonPropertyName("mercado_pos_rodada")]
    public bool MercadoPosRodada { get; set; }

    [JsonPropertyName("novo_mes_ranking")]
    public bool NovoMesRanking { get; set; }

    [JsonPropertyName("degustacao_gatomestre")]
    public bool DegustacaoGatomestre { get; set; }

    [JsonPropertyName("nome_rodada")]
    public string NomeRodada { get; set; }

    [JsonPropertyName("limites_competicoes")]
    public LimitesCompeticoes LimitesCompeticoes { get; set; }
}
