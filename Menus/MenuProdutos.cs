using Comex.Models;
using Comex.Persistence;
using Comex.Utilities;

namespace Comex.Menus;

internal sealed class MenuProdutos
{
    private readonly Dictionary<int, Produto> _produtos;
    private readonly PersistenciaJson _persistencia;

    public MenuProdutos(Dictionary<int, Produto> produtos, PersistenciaJson persistencia)
    {
        _produtos = produtos;
        _persistencia = persistencia;
    }

    public void Cadastrar()
    {
        string? nome;
        do
        {
            Console.Write("Digite o nome do produto: ");
            nome = Console.ReadLine();
            if (nome is null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(nome))
            {
                Console.WriteLine("O nome do produto é obrigatório.");
            }
        }
        while (string.IsNullOrWhiteSpace(nome));

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

        int id = _produtos.Keys.DefaultIfEmpty().Max() + 1;
        Produto produto = new(id, nome, descricao, precoUnitario, quantidade);
        _produtos[id] = produto;
        _persistencia.SalvarProdutos(_produtos.Values);
        Console.WriteLine($"Produto {produto.Nome} cadastrado com sucesso.");
        ExibirProduto(produto);
    }

    public void Listar()
    {
        if (_produtos.Count == 0)
        {
            Console.WriteLine("Nenhum produto cadastrado.");
            return;
        }

        ExibirProdutos(_produtos.Values);
    }

    public void ListarPorNome()
    {
        if (_produtos.Count == 0)
        {
            Listar();
            return;
        }

        ExibirProdutos(_produtos.Values.OrderBy(produto => produto.Nome));
    }

    public void ListarPorPreco()
    {
        if (_produtos.Count == 0)
        {
            Listar();
            return;
        }

        ExibirProdutos(_produtos.Values
            .OrderBy(produto => produto.PrecoUnitario)
            .ThenBy(produto => produto.Nome));
    }

    private static void ExibirProdutos(IEnumerable<Produto> produtos)
    {
        foreach (Produto produto in produtos)
        {
            ExibirProduto(produto);
        }
    }

    public void AjustarPreco()
    {
        if (_produtos.Count == 0)
        {
            Listar();
            return;
        }

        Listar();
        Console.Write("Digite o ID do produto que deseja ajustar: ");
        string? entradaIdProduto = Console.ReadLine();
        if (entradaIdProduto is null)
        {
            return;
        }

        if (!int.TryParse(entradaIdProduto, out int idProduto)
            || !_produtos.TryGetValue(idProduto, out Produto? produto))
        {
            Console.WriteLine("Produto não cadastrado no sistema.");
            return;
        }

        Console.Write("Digite o novo preço do produto: ");
        string? precoInformado = Console.ReadLine();
        if (precoInformado is null)
        {
            return;
        }

        if (!ConversorPreco.TryParse(precoInformado, out decimal novoPreco))
        {
            Console.WriteLine("Preço inválido.");
            return;
        }

        produto.PrecoUnitario = novoPreco;
        _persistencia.SalvarProdutos(_produtos.Values);
        Console.WriteLine($"Preço do produto {produto.Nome} ajustado com sucesso.");
    }

    private static void ExibirProduto(Produto produto)
    {
        Console.WriteLine($"ID: {produto.Id} | Nome: {produto.Nome} | Descrição: {produto.Descricao} | Preço unitário: R$ {produto.PrecoUnitario:F2} | Quantidade: {produto.Quantidade}");
    }
}