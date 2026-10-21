using Comex.Models;
using Comex.Persistence;
using Comex.Utilities;

namespace Comex.Tests;

internal static class TesteProduto
{
    public static void Executar()
    {
        PersistenciaJson persistencia = new();
        List<Produto> produtos = persistencia.CarregarProdutos();

        while (true)
        {
            Console.WriteLine("\n=== MENU DE PRODUTOS ===");
            Console.WriteLine("1 - Criar produto");
            Console.WriteLine("2 - Listar produtos");
            Console.WriteLine("0 - Sair");
            Console.Write("Escolha uma opção: ");

            string? entrada = Console.ReadLine();
            if (entrada is null)
            {
                return;
            }

            if (!int.TryParse(entrada, out int opcao))
            {
                Console.WriteLine("Opção inválida.");
                continue;
            }

            switch (opcao)
            {
                case 0:
                    return;
                case 1:
                    CriarProduto(produtos, persistencia);
                    break;
                case 2:
                    ListarProdutos(produtos);
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
    }

    private static void CriarProduto(List<Produto> produtos, PersistenciaJson persistencia)
    {
        Console.Write("Digite o nome do produto: ");
        string? nome = Console.ReadLine();
        if (nome is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(nome))
        {
            Console.WriteLine("O nome do produto é obrigatório.");
            return;
        }

        Console.Write("Digite a descrição do produto: ");
        string? descricao = Console.ReadLine();
        if (descricao is null)
        {
            return;
        }

        Console.Write("Digite o preço unitário do produto: ");
        string? precoInformado = Console.ReadLine();
        if (precoInformado is null)
        {
            return;
        }

        if (!ConversorPreco.TryParse(precoInformado, out decimal precoUnitario))
        {
            Console.WriteLine("Preço inválido.");
            return;
        }

        Console.Write("Digite a quantidade do produto: ");
        string? quantidadeInformada = Console.ReadLine();
        if (quantidadeInformada is null)
        {
            return;
        }

        if (!int.TryParse(quantidadeInformada, out int quantidade))
        {
            Console.WriteLine("Quantidade inválida.");
            return;
        }

        int id = produtos.Select(produto => produto.Id).DefaultIfEmpty().Max() + 1;
        Produto produto = new(id, nome, descricao, precoUnitario, quantidade);
        produtos.Add(produto);
        persistencia.SalvarProdutos(produtos);
        Console.WriteLine($"Produto {produto.Nome} criado com sucesso.");
    }

    private static void ListarProdutos(List<Produto> produtos)
    {
        if (produtos.Count == 0)
        {
            Console.WriteLine("Nenhum produto cadastrado.");
            return;
        }

        foreach (Produto produto in produtos)
        {
            Console.WriteLine($"ID: {produto.Id} | Nome: {produto.Nome} | Descrição: {produto.Descricao} | Preço unitário: R$ {produto.PrecoUnitario:F2} | Quantidade: {produto.Quantidade}");
        }
    }
}