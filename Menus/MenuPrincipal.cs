using System.ComponentModel;
using System.Reflection;
using Comex.Models;
using Comex.Persistence;

namespace Comex.Menus;

internal sealed class MenuPrincipal
{
    private enum OpcaoMenu
    {
        [Description("Sair")]
        Sair = -1,

        [Description("Cadastrar Cliente")]
        CadastrarCliente = 1,

        [Description("Listar Clientes")]
        ListarClientes = 2,

        [Description("Cadastrar Produto")]
        CadastrarProduto = 3,

        [Description("Listar produtos")]
        ListarProdutos = 4,

        [Description("Ajustar preço de produto")]
        AjustarPrecoDeProduto = 5,

        [Description("Adicionar produto no carrinho")]
        AdicionarProdutoNoCarrinho = 6,

        [Description("Listar carrinho")]
        ListarCarrinho = 7,

        [Description("Fechar compra")]
        FecharCompra = 8
    }

    private readonly MenuClientes _menuClientes;
    private readonly MenuProdutos _menuProdutos;
    private readonly MenuCarrinho _menuCarrinho;

    public MenuPrincipal()
    {
        PersistenciaJson persistencia = new();
        Dictionary<int, Produto> produtos = persistencia.CarregarProdutos().ToDictionary(produto => produto.Id);
        Dictionary<int, Endereco> enderecos = persistencia.CarregarEnderecos().ToDictionary(endereco => endereco.Id);
        Dictionary<int, Cliente> clientes = persistencia.CarregarClientes(enderecos);
        Dictionary<int, Carrinho> carrinhos = persistencia.CarregarCarrinhos().ToDictionary(carrinho => carrinho.ClienteId);

        _menuClientes = new MenuClientes(clientes, enderecos, persistencia);
        _menuProdutos = new MenuProdutos(produtos, persistencia);
        _menuCarrinho = new MenuCarrinho(produtos, carrinhos, persistencia, _menuClientes, _menuProdutos);
    }

    public void Executar()
    {
        while (true)
        {
            ExibirOpcoes();
            string? entrada = Console.ReadLine();
            if (entrada is null)
            {
                return;
            }

            if (!int.TryParse(entrada, out int opcaoInformada))
            {
                Console.WriteLine("Opção inválida.");
                continue;
            }

            OpcaoMenu opcao = (OpcaoMenu)opcaoInformada;
            ExibirOpcaoEscolhida(opcaoInformada, opcao);

            switch (opcao)
            {
                case OpcaoMenu.Sair:
                    Console.WriteLine("COMEX FINALIZADO!");
                    return;
                case OpcaoMenu.CadastrarCliente:
                    _menuClientes.Cadastrar();
                    break;
                case OpcaoMenu.ListarClientes:
                    _menuClientes.Listar();
                    break;
                case OpcaoMenu.CadastrarProduto:
                    _menuProdutos.Cadastrar();
                    break;
                case OpcaoMenu.AjustarPrecoDeProduto:
                    _menuProdutos.AjustarPreco();
                    break;
                case OpcaoMenu.ListarProdutos:
                    _menuProdutos.Listar();
                    break;
                case OpcaoMenu.AdicionarProdutoNoCarrinho:
                    _menuCarrinho.AdicionarProduto();
                    break;
                case OpcaoMenu.ListarCarrinho:
                    _menuCarrinho.Listar();
                    break;
                case OpcaoMenu.FecharCompra:
                    _menuCarrinho.FecharCompra();
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
    }

    private static void ExibirOpcoes()
    {
        Console.WriteLine("\n=== COMEX ===");
        foreach (OpcaoMenu opcaoMenu in Enum.GetValues<OpcaoMenu>()
            .OrderBy(opcaoMenu => opcaoMenu == OpcaoMenu.Sair)
            .ThenBy(opcaoMenu => (int)opcaoMenu))
        {
            Console.WriteLine($"{(int)opcaoMenu} - {ObterDescricao(opcaoMenu)}");
        }

        Console.Write("Escolha uma opção: ");
    }

    private static void ExibirOpcaoEscolhida(int opcaoInformada, OpcaoMenu opcao)
    {
        if (Enum.IsDefined(typeof(OpcaoMenu), opcao))
        {
            Console.WriteLine($"Opção escolhida: {opcaoInformada} - {ObterDescricao(opcao)}");
        }
        else
        {
            Console.WriteLine($"Opção escolhida: {opcaoInformada}");
        }
    }

    private static string ObterDescricao(OpcaoMenu opcao)
    {
        return typeof(OpcaoMenu)
            .GetField(opcao.ToString())?
            .GetCustomAttribute<DescriptionAttribute>()?
            .Description ?? opcao.ToString();
    }
}