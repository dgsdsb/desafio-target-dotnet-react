# Desafio Dev Target ERP (.NET + React)
Console em C# (.NET 8) para cálculo de comissões, controle de estoque e juros simples.
Opcionalmente, deixei uma interface estática em React só para visualizar os resultados no navegador.

## Como rodar

Console (.NET):
Na raiz do projeto:

dotnet run

Ele usa automaticamente vendas.json e estoque.json.

Web local (estático):

npx serve .

Depois abra o endereço que aparecer no terminal — você cai direto em /web.

## O que o projeto faz?

### Comissões:

Lê vendas.json e aplica a regra:

< 100 → 0%

100 a 500 → 1%

> 500 → 5%

### Estoque:

Lê estoque.json, valida produto/quantidade, aplica entrada/saída e mostra o saldo final. Se tentar algo impossível, acusa erro.

### Juros simples:

Cálculo de 2,5% ao dia usando valor e datas informadas (console e web).

## Estrutura

Program.cs → orquestra leitura dos arquivos, cálculos e saída no console.

Models/ → Venda, Produto, EstoqueRoot, VendasRoot, Movimentacao.

Services/ → GerenciadorEstoque (valida e aplica movimentações).

vendas.json e estoque.json → cenários padrão.

web/ → versão estática com React via CDN (index.html, styles.css, vendas.json, estoque.json).

index.html (raiz) → redireciona para /web.
