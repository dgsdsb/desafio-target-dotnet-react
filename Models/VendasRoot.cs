using System.Text.Json.Serialization;

namespace Desafio.Models;

public class VendasRoot
{
    [JsonPropertyName("vendas")]
    public List<Venda> Vendas { get; set; } = new();
}
