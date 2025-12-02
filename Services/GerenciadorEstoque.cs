using Desafio.Models;

namespace Desafio.Services;

public class GerenciadorEstoque
{
    private readonly Dictionary<int, Produto> _produtos;

    public GerenciadorEstoque(IEnumerable<Produto> produtos)
    {
        _produtos = produtos.ToDictionary(p => p.CodigoProduto);
    }

    public Movimentacao RegistrarMovimentacao(int codigoProduto, int quantidade, string descricao, bool isEntrada)
    {
        if (quantidade <= 0)
        {
            throw new ArgumentException("Quantidade deve ser positiva.", nameof(quantidade));
        }

        if (!_produtos.TryGetValue(codigoProduto, out var produto))
        {
            throw new KeyNotFoundException($"Produto {codigoProduto} não encontrado.");
        }

        var novoSaldo = produto.QuantidadeEstoque + (isEntrada ? quantidade : -quantidade);
        if (novoSaldo < 0)
        {
            throw new InvalidOperationException($"Estoque insuficiente para saída de {quantidade} unidades.");
        }

        produto.QuantidadeEstoque = novoSaldo;

        return new Movimentacao
        {
            CodigoProduto = codigoProduto,
            Quantidade = quantidade,
            Descricao = descricao,
            Tipo = isEntrada ? "Entrada" : "Saída",
            EstoqueFinal = novoSaldo,
            Data = DateTime.Now
        };
    }

    public IEnumerable<Produto> ListarProdutos() => _produtos.Values.OrderBy(p => p.CodigoProduto);
}
