namespace Desafio.Models;

public class Movimentacao
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public int CodigoProduto { get; init; }

    public string Descricao { get; init; } = string.Empty;

    public int Quantidade { get; init; }

    public string Tipo { get; init; } = string.Empty;

    public int EstoqueFinal { get; init; }

    public DateTime Data { get; init; } = DateTime.UtcNow;
}
