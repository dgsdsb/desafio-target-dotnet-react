using System.Text.Json.Serialization;

namespace Desafio.Models;

public class EstoqueRoot
{
    [JsonPropertyName("estoque")]
    public List<Produto> Estoque { get; set; } = new();
}
