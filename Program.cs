using System.ComponentModel;
using System.Reflection;

internal class Program
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

    private sealed record Cliente(int Id, string Cpf, string Nome);

    private sealed record Produto(int Id, string Nome, decimal Preco);

    private static void Main()
    {
        ExibirMenuDeOpcoes();
    }

    private static void ExibirMenuDeOpcoes()
    {
        Dictionary<int, Cliente> clientes = new();
        Dictionary<int, Produto> produtos = new();
        Dictionary<int, List<int>> carrinhos = new();

        while (true)
        {
            Console.WriteLine("\n=== COMEX ===");
            foreach (OpcaoMenu opcaoMenu in Enum.GetValues<OpcaoMenu>()
                .OrderBy(opcaoMenu => opcaoMenu == OpcaoMenu.Sair)
                .ThenBy(opcaoMenu => (int)opcaoMenu))
            {
                Console.WriteLine($"{(int)opcaoMenu} - {ObterDescricao(opcaoMenu)}");
            }

            Console.Write("Escolha uma opção: ");

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
            if (Enum.IsDefined(typeof(OpcaoMenu), opcao))
            {
                Console.WriteLine($"Opção escolhida: {opcaoInformada} - {ObterDescricao(opcao)}");
            }
            else
            {
                Console.WriteLine($"Opção escolhida: {opcaoInformada}");
            }

            switch (opcao)
            {
                case OpcaoMenu.Sair:
                    Console.WriteLine("COMEX FINALIZADO!");
                    return;
                case OpcaoMenu.CadastrarCliente:
                    CadastraCliente(clientes);
                    break;
                case OpcaoMenu.ListarClientes:
                    ListaClientes(clientes);
                    break;
                case OpcaoMenu.CadastrarProduto:
                    CadastraProduto(produtos);
                    break;
                case OpcaoMenu.AjustarPrecoDeProduto:
                    AjustarPrecoDeProduto(produtos);
                    break;
                case OpcaoMenu.ListarProdutos:
                    ListaProdutos(produtos);
                    break;
                case OpcaoMenu.AdicionarProdutoNoCarrinho:
                    AdicionaProdutoNoCarrinho(clientes, produtos, carrinhos);
                    break;
                case OpcaoMenu.ListarCarrinho:
                    ListaCarrinho(clientes, produtos, carrinhos);
                    break;
                case OpcaoMenu.FecharCompra:
                    FechaCompra(clientes, produtos, carrinhos);
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
    }

    private static string ObterDescricao(OpcaoMenu opcao)
    {
        return typeof(OpcaoMenu)
            .GetField(opcao.ToString())?
            .GetCustomAttribute<DescriptionAttribute>()?
            .Description ?? opcao.ToString();
    }

    private static void AdicionaProdutoNoCarrinho(
        Dictionary<int, Cliente> clientes,
        Dictionary<int, Produto> produtos,
        Dictionary<int, List<int>> carrinhos)
    {
        Cliente? cliente = SelecionaCliente(clientes);
        if (cliente is null)
        {
            return;
        }

        if (produtos.Count == 0)
        {
            ListaProdutos(produtos);
            return;
        }

        ListaProdutos(produtos);
        Console.Write("Digite o ID do produto que deseja adicionar: ");
        string? entradaIdProduto = Console.ReadLine();
        if (entradaIdProduto is null)
        {
            return;
        }

        if (!int.TryParse(entradaIdProduto, out int idProduto)
            || !produtos.TryGetValue(idProduto, out Produto? produto))
        {
            Console.WriteLine("Produto não cadastrado.");
            return;
        }

        if (!carrinhos.TryGetValue(cliente.Id, out List<int>? carrinho))
        {
            carrinho = new List<int>();
            carrinhos[cliente.Id] = carrinho;
        }

        carrinho.Add(produto.Id);
        Console.WriteLine($"Produto {produto.Nome} adicionado ao carrinho de {cliente.Nome} com sucesso.");
    }

    private static Cliente? SelecionaCliente(
        Dictionary<int, Cliente> clientes,
        string mensagemNaoCadastrado = "Usuário não cadastrado.",
        string mensagemListaVazia = "Nenhum cliente cadastrado.")
    {
        if (clientes.Count == 0)
        {
            Console.WriteLine(mensagemListaVazia);
            return null;
        }

        ListaClientes(clientes);
        Console.Write("Digite o ID ou CPF do cliente: ");
        string? identificador = Console.ReadLine();
        if (identificador is null)
        {
            return null;
        }

        string cpf = new(identificador.Where(char.IsAsciiDigit).ToArray());
        Cliente? cliente = clientes.Values.FirstOrDefault(item => item.Cpf == cpf);

        if (cliente is null
            && int.TryParse(identificador, out int idCliente)
            && clientes.TryGetValue(idCliente, out Cliente? clientePorId))
        {
            cliente = clientePorId;
        }

        if (cliente is null)
        {
            Console.WriteLine(mensagemNaoCadastrado);
        }

        return cliente;
    }

    private static void ListaCarrinho(
        Dictionary<int, Cliente> clientes,
        Dictionary<int, Produto> produtos,
        Dictionary<int, List<int>> carrinhos)
    {
        Cliente? cliente = SelecionaCliente(clientes);
        if (cliente is null)
        {
            return;
        }

        ExibeCarrinho(cliente, produtos, carrinhos);
    }

    private static decimal? ExibeCarrinho(
        Cliente cliente,
        Dictionary<int, Produto> produtos,
        Dictionary<int, List<int>> carrinhos)
    {
        if (!carrinhos.TryGetValue(cliente.Id, out List<int>? carrinho) || carrinho.Count == 0)
        {
            Console.WriteLine($"O carrinho de {cliente.Nome} está vazio.");
            return null;
        }

        decimal total = 0;
        foreach (int idProduto in carrinho)
        {
            Produto produto = produtos[idProduto];
            Console.WriteLine($"ID: {produto.Id} | Produto: {produto.Nome} | Preço: {produto.Preco}");
            total += produto.Preco;
        }

        Console.WriteLine($"Total: R$ {total:F2}");
        return total;
    }

    private static void FechaCompra(
        Dictionary<int, Cliente> clientes,
        Dictionary<int, Produto> produtos,
        Dictionary<int, List<int>> carrinhos)
    {
        Cliente? cliente = SelecionaCliente(
            clientes,
            "Cliente não cadastrado.",
            "Cliente não cadastrado.");
        if (cliente is null)
        {
            return;
        }

        decimal? valorFinal = ExibeCarrinho(cliente, produtos, carrinhos);
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

    private static void AjustarPrecoDeProduto(Dictionary<int, Produto> produtos)
    {
        if (produtos.Count == 0)
        {
            ListaProdutos(produtos);
            return;
        }

        ListaProdutos(produtos);
        Console.Write("Digite o ID do produto que deseja ajustar: ");
        string? entradaIdProduto = Console.ReadLine();
        if (entradaIdProduto is null)
        {
            return;
        }

        if (!int.TryParse(entradaIdProduto, out int idProduto)
            || !produtos.TryGetValue(idProduto, out Produto? produto))
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

        if (!decimal.TryParse(precoInformado, out decimal novoPreco))
        {
            Console.WriteLine("Preço inválido.");
            return;
        }

        produtos[idProduto] = produto with { Preco = novoPreco };
        Console.WriteLine($"Preço do produto {produto.Nome} ajustado com sucesso.");
    }

    private static void CadastraProduto(Dictionary<int, Produto> produtos)
    {
        Console.Write("Digite o nome do produto: ");
        string? nome = Console.ReadLine();
        if (nome is null)
        {
            return;
        }

        Console.Write("Digite o preço do produto: ");
        string? precoInformado = Console.ReadLine();
        if (precoInformado is null)
        {
            return;
        }

        if (!decimal.TryParse(precoInformado, out decimal preco))
        {
            Console.WriteLine("Preço inválido.");
            return;
        }

        int id = produtos.Count + 1;
        produtos[id] = new Produto(id, nome, preco);
        Console.WriteLine($"Produto {nome} cadastrado com sucesso. ID: {id}.");
    }

    private static void ListaProdutos(Dictionary<int, Produto> produtos)
    {
        if (produtos.Count == 0)
        {
            Console.WriteLine("Nenhum produto cadastrado.");
            return;
        }

        foreach (Produto produto in produtos.Values)
        {
            Console.WriteLine($"ID: {produto.Id} | Produto: {produto.Nome} | Preço: {produto.Preco}");
        }
    }

    private static void ListaClientes(Dictionary<int, Cliente> clientes)
    {
        if (clientes.Count == 0)
        {
            Console.WriteLine("Nenhum cliente cadastrado.");
            return;
        }

        foreach (Cliente cliente in clientes.Values)
        {
            Console.WriteLine($"ID: {cliente.Id} | CPF: {cliente.Cpf} | Nome: {cliente.Nome}");
        }
    }

    private static void CadastraCliente(Dictionary<int, Cliente> clientes)
    {
        Console.Write("Digite o CPF do cliente: ");
        string? cpf = Console.ReadLine();
        if (cpf is null)
        {
            return;
        }

        cpf = new string(cpf.Where(char.IsAsciiDigit).ToArray());
        if (cpf.Length == 0)
        {
            Console.WriteLine("O CPF deve conter números.");
            return;
        }

        if (clientes.Values.Any(cliente => cliente.Cpf == cpf))
        {
            Console.WriteLine("Já existe um cliente cadastrado com esse CPF.");
            return;
        }

        Console.Write("Digite o nome do cliente: ");
        string? nome = Console.ReadLine();
        if (nome is null)
        {
            return;
        }

        int id = clientes.Count + 1;
        clientes[id] = new Cliente(id, cpf, nome);
        Console.WriteLine($"Usuário {nome} ({cpf}) cadastrado com sucesso. ID: {id}.");
    }
}