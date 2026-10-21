using Comex.Models;
using Comex.Persistence;

namespace Comex.Menus;

internal sealed class MenuCarrinho
{
    private readonly Dictionary<int, Produto> _produtos;
    private readonly Dictionary<int, Carrinho> _carrinhos;
    private readonly PersistenciaJson _persistencia;
    private readonly MenuClientes _menuClientes;
    private readonly MenuProdutos _menuProdutos;

    public MenuCarrinho(
        Dictionary<int, Produto> produtos,
        Dictionary<int, Carrinho> carrinhos,
        PersistenciaJson persistencia,
        MenuClientes menuClientes,
        MenuProdutos menuProdutos)
    {
        _produtos = produtos;
        _carrinhos = carrinhos;
        _persistencia = persistencia;
        _menuClientes = menuClientes;
        _menuProdutos = menuProdutos;
    }

    public void AdicionarProduto()
    {
        Cliente? cliente = _menuClientes.Selecionar();
        if (cliente is null)
        {
            return;
        }

        if (_produtos.Count == 0)
        {
            _menuProdutos.Listar();
            return;
        }

        _menuProdutos.Listar();
        Console.Write("Digite o ID do produto que deseja adicionar: ");
        string? entradaIdProduto = Console.ReadLine();
        if (entradaIdProduto is null)
        {
            return;
        }

        if (!int.TryParse(entradaIdProduto, out int idProduto)
            || !_produtos.TryGetValue(idProduto, out Produto? produto))
        {
            Console.WriteLine("Produto não cadastrado.");
            return;
        }

        if (!_carrinhos.TryGetValue(cliente.Id, out Carrinho? carrinho))
        {
            carrinho = new Carrinho { ClienteId = cliente.Id };
            _carrinhos[cliente.Id] = carrinho;
        }

        carrinho.ProdutoIds.Add(produto.Id);
        _persistencia.SalvarCarrinhos(_carrinhos.Values);
        Console.WriteLine($"Produto {produto.Nome} adicionado ao carrinho de {cliente.Nome} com sucesso.");
    }

    public void Listar()
    {
        Cliente? cliente = _menuClientes.Selecionar();
        if (cliente is null)
        {
            return;
        }

        ExibirCarrinho(cliente);
    }

    public void FecharCompra()
    {
        Cliente? cliente = _menuClientes.Selecionar(
            "Cliente não cadastrado.",
            "Cliente não cadastrado.");
        if (cliente is null)
        {
            return;
        }

        decimal? valorFinal = ExibirCarrinho(cliente);
        if (valorFinal is null)
        {
            return;
        }

        Console.Write("Digite o número do cartão: ");
        string? numeroCartao = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(numeroCartao))
        {
            Console.WriteLine("Número do cartão inválido.");
            return;
        }

        Console.WriteLine($"Realizando pagamento de R$ {valorFinal.Value:F2} no cartão {numeroCartao}.");
    }

    private decimal? ExibirCarrinho(Cliente cliente)
    {
        if (!_carrinhos.TryGetValue(cliente.Id, out Carrinho? carrinho) || carrinho.ProdutoIds.Count == 0)
        {
            Console.WriteLine($"O carrinho de {cliente.Nome} está vazio.");
            return null;
        }

        decimal total = 0;
        foreach (int idProduto in carrinho.ProdutoIds)
        {
            Produto produto = _produtos[idProduto];
            Console.WriteLine($"ID: {produto.Id} | Produto: {produto.Nome} | Preço unitário: R$ {produto.PrecoUnitario:F2}");
            total += produto.PrecoUnitario;
        }

        Console.WriteLine($"Total: R$ {total:F2}");
        return total;
    }
}