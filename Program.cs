using System.Globalization;
using System.Linq;
using System.Text.Json;
using Desafio.Models;
using Desafio.Services;

var culture = CultureInfo.GetCultureInfo("pt-BR");
var jsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    ReadCommentHandling = JsonCommentHandling.Skip
};

Console.WriteLine("Desafio Técnico Target - ERP (Console)\n");

var vendasRoot = LerArquivoJson<VendasRoot>("vendas.json") ?? new VendasRoot();
var estoqueRoot = LerArquivoJson<EstoqueRoot>("estoque.json") ?? new EstoqueRoot();

ExibirComissoes(vendasRoot.Vendas);

var gerenciadorEstoque = new GerenciadorEstoque(estoqueRoot.Estoque);
ExecutarMovimentacoesExemplo(gerenciadorEstoque);

CalcularJurosInterativo();

T? LerArquivoJson<T>(string caminho) where T : class
{
    try
    {
        if (!File.Exists(caminho))
        {
            Console.WriteLine($"Arquivo {caminho} não encontrado.");
            return null;
        }

        var json = File.ReadAllText(caminho);
        return JsonSerializer.Deserialize<T>(json, jsonOptions);
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"Erro ao interpretar {caminho}: {ex.Message}");
        return null;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao ler {caminho}: {ex.Message}");
        return null;
    }
}

void ExibirComissoes(IEnumerable<Venda> vendas)
{
    Console.WriteLine("Comissões por vendedor:");

    if (!vendas.Any())
    {
        Console.WriteLine("Nenhuma venda encontrada.");
        return;
    }

    var resumo = vendas
        .GroupBy(v => v.Vendedor)
        .Select(g => new
        {
            Vendedor = g.Key,
            TotalComissao = g.Sum(v => CalcularComissaoPorVenda(v.Valor)),
            TotalVendido = g.Sum(v => v.Valor),
            QuantidadeVendas = g.Count()
        })
        .OrderByDescending(r => r.TotalComissao);

    foreach (var item in resumo)
    {
        Console.WriteLine(
            $"- {item.Vendedor}: comissões {item.TotalComissao.ToString("C", culture)} " +
            $"em {item.QuantidadeVendas} vendas (total vendido {item.TotalVendido.ToString("C", culture)})");
    }
}

decimal CalcularComissaoPorVenda(decimal valor)
{
    return valor switch
    {
        < 100m => 0m,
        <= 500m => valor * 0.01m,
        _ => valor * 0.05m
    };

}

void ExecutarMovimentacoesExemplo(GerenciadorEstoque gerenciador)
{
    Console.WriteLine("\nMovimentações de estoque:");

    var operacoes = new[]
    {
        new { Codigo = 101, Quantidade = 25, Entrada = true, Descricao = "Reposição do fornecedor" },
        new { Codigo = 102, Quantidade = 10, Entrada = false, Descricao = "Saída para pedido 4501" },
        new { Codigo = 105, Quantidade = 5, Entrada = false, Descricao = "Saída para pedido 4502" },
        new { Codigo = 999, Quantidade = 5, Entrada = false, Descricao = "Teste de produto inexistente" }
    };

    foreach (var op in operacoes)
    {
        try
        {
            var mov = gerenciador.RegistrarMovimentacao(op.Codigo, op.Quantidade, op.Descricao, op.Entrada);
            Console.WriteLine(
                $"[{mov.Tipo}] Produto {mov.CodigoProduto} - {op.Descricao}. " +
                $"Qtde: {mov.Quantidade}. Estoque final: {mov.EstoqueFinal}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Falha na movimentação do produto {op.Codigo}: {ex.Message}");
        }
    }

    Console.WriteLine("\nEstoque consolidado:");
    foreach (var produto in gerenciador.ListarProdutos())
    {
        Console.WriteLine($"- {produto.CodigoProduto} | {produto.DescricaoProduto}: {produto.QuantidadeEstoque} unidades");
    }
}

void CalcularJurosInterativo()
{
    Console.WriteLine("\nJuros por atraso (2,5% ao dia):");
    Console.Write("Valor principal (ex: 1500,00): ");
    var valorTexto = Console.ReadLine();

    if (!decimal.TryParse(valorTexto, NumberStyles.Number, culture, out var principal))
    {
        Console.WriteLine("Valor inválido.");
        return;
    }

    Console.Write("Data de vencimento (dd/MM/yyyy): ");
    var dataTexto = Console.ReadLine();

    if (!DateTime.TryParseExact(dataTexto, "dd/MM/yyyy", culture, DateTimeStyles.None, out var vencimento))
    {
        Console.WriteLine("Data inválida.");
        return;
    }

    var hoje = DateTime.Today;
    var diasAtraso = Math.Max(0, (hoje - vencimento.Date).Days);
    var juros = CalcularJurosSimples(principal, diasAtraso, 0.025m);
    var totalAtualizado = principal + juros;

    Console.WriteLine($"Dias em atraso: {diasAtraso}");
    Console.WriteLine($"Juros: {juros.ToString("C", culture)}");
    Console.WriteLine($"Total atualizado: {totalAtualizado.ToString("C", culture)}");
}

decimal CalcularJurosSimples(decimal principal, int diasAtraso, decimal taxaDiaria)
{
    if (diasAtraso <= 0)
    {
        return 0m;
    }

    return principal * taxaDiaria * diasAtraso;
}
