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

        [Description("Adicionar produto no carrinho")]
        AdicionarProdutoNoCarrinho = 6,
        
        [Description("Ajustar preço de produto")]
        AjustarPrecoDeProduto = 5,


        [Description("Listar carrinho")]
        ListarCarrinho = 7,

        [Description("Fechar compra")]
        FecharCompra = 8
    }

    private static void Main(string[] args)
    {
        if (args.Length > 0 && string.Equals(args[0], "--teste-cliente", StringComparison.OrdinalIgnoreCase))
        {
            TesteCliente.Executar();
            return;
        }

        if (args.Length > 0 && string.Equals(args[0], "--teste-produto", StringComparison.OrdinalIgnoreCase))
        {
            TesteProduto.Executar();
            return;
        }

        ExibirMenuDeOpcoes();
    }

    private static void ExibirMenuDeOpcoes()
    {
        PersistenciaJson persistencia = new();
        Dictionary<int, Produto> produtos = persistencia.CarregarProdutos().ToDictionary(produto => produto.Id);
        Dictionary<int, Endereco> enderecos = persistencia.CarregarEnderecos().ToDictionary(endereco => endereco.Id);
        Dictionary<int, Cliente> clientes = persistencia.CarregarClientes(enderecos);
        Dictionary<int, Carrinho> carrinhos = persistencia.CarregarCarrinhos().ToDictionary(carrinho => carrinho.ClienteId);

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
                    CadastraCliente(clientes, enderecos, persistencia);
                    break;
                case OpcaoMenu.ListarClientes:
                    ListaClientes(clientes);
                    break;
                case OpcaoMenu.CadastrarProduto:
                    CadastraProduto(produtos, persistencia);
                    break;
                case OpcaoMenu.AjustarPrecoDeProduto:
                    AjustarPrecoDeProduto(produtos, persistencia);
                    break;
                case OpcaoMenu.ListarProdutos:
                    ListaProdutos(produtos);
                    break;
                case OpcaoMenu.AdicionarProdutoNoCarrinho:
                    AdicionaProdutoNoCarrinho(clientes, produtos, carrinhos, persistencia);
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
        Dictionary<int, Carrinho> carrinhos,
        PersistenciaJson persistencia)
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

        if (!carrinhos.TryGetValue(cliente.Id, out Carrinho? carrinho))
        {
            carrinho = new Carrinho { ClienteId = cliente.Id };
            carrinhos[cliente.Id] = carrinho;
        }

        carrinho.ProdutoIds.Add(produto.Id);
        persistencia.SalvarCarrinhos(carrinhos.Values);
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
        Dictionary<int, Carrinho> carrinhos)
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
        Dictionary<int, Carrinho> carrinhos)
    {
        if (!carrinhos.TryGetValue(cliente.Id, out Carrinho? carrinho) || carrinho.ProdutoIds.Count == 0)
        {
            Console.WriteLine($"O carrinho de {cliente.Nome} está vazio.");
            return null;
        }

        decimal total = 0;
        foreach (int idProduto in carrinho.ProdutoIds)
        {
            Produto produto = produtos[idProduto];
            Console.WriteLine($"ID: {produto.Id} | Produto: {produto.Nome} | Preço unitário: R$ {produto.PrecoUnitario:F2}");
            total += produto.PrecoUnitario;
        }

        Console.WriteLine($"Total: R$ {total:F2}");
        return total;
    }

    private static void FechaCompra(
        Dictionary<int, Cliente> clientes,
        Dictionary<int, Produto> produtos,
        Dictionary<int, Carrinho> carrinhos)
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

    private static void AjustarPrecoDeProduto(Dictionary<int, Produto> produtos, PersistenciaJson persistencia)
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

        if (!ConversorPreco.TryParse(precoInformado, out decimal novoPreco))
        {
            Console.WriteLine("Preço inválido.");
            return;
        }

        produto.PrecoUnitario = novoPreco;
        persistencia.SalvarProdutos(produtos.Values);
        Console.WriteLine($"Preço do produto {produto.Nome} ajustado com sucesso.");
    }

    private static void CadastraProduto(Dictionary<int, Produto> produtos, PersistenciaJson persistencia)
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

        int id = produtos.Keys.DefaultIfEmpty().Max() + 1;
        Produto produto = new(id, nome, descricao, precoUnitario, quantidade);
        produtos[id] = produto;
        persistencia.SalvarProdutos(produtos.Values);
        Console.WriteLine($"Produto {produto.Nome} cadastrado com sucesso.");
        Console.WriteLine($"ID: {produto.Id} | Nome: {produto.Nome} | Descrição: {produto.Descricao} | Preço unitário: R$ {produto.PrecoUnitario:F2} | Quantidade: {produto.Quantidade}");
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
            Console.WriteLine($"ID: {produto.Id} | Nome: {produto.Nome} | Descrição: {produto.Descricao} | Preço unitário: R$ {produto.PrecoUnitario:F2} | Quantidade: {produto.Quantidade}");
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
            Console.WriteLine($"ID: {cliente.Id} | CPF: {cliente.Cpf} | Nome: {cliente.Nome} | E-mail: {cliente.Email} | Profissão: {cliente.Profissao} | Telefone: {cliente.Telefone}");
            Console.WriteLine($"Endereço: Rua: {cliente.Endereco.Rua}, {cliente.Endereco.Numero} | {cliente.Endereco.Complemento} | {cliente.Endereco.Bairro} | {cliente.Endereco.Cidade} - {cliente.Endereco.Estado}");
        }
    }

    private static void CadastraCliente(
        Dictionary<int, Cliente> clientes,
        Dictionary<int, Endereco> enderecos,
        PersistenciaJson persistencia)
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

        Console.Write("Digite o e-mail do cliente: ");
        string? email = Console.ReadLine();
        if (email is null)
        {
            return;
        }

        Console.Write("Digite a profissão do cliente: ");
        string? profissao = Console.ReadLine();
        if (profissao is null)
        {
            return;
        }

        Console.Write("Digite o telefone do cliente: ");
        string? telefone = Console.ReadLine();
        if (telefone is null)
        {
            return;
        }

        Console.Write("Digite a rua do cliente: ");
        string? rua = Console.ReadLine();
        if (rua is null)
        {
            return;
        }

        Console.Write("Digite o número do endereço: ");
        string? numeroInformado = Console.ReadLine();
        if (numeroInformado is null)
        {
            return;
        }

        if (!int.TryParse(numeroInformado, out int numero))
        {
            Console.WriteLine("Número do endereço inválido.");
            return;
        }

        Console.Write("Digite o complemento do endereço: ");
        string? complemento = Console.ReadLine();
        if (complemento is null)
        {
            return;
        }

        Console.Write("Digite o bairro do cliente: ");
        string? bairro = Console.ReadLine();
        if (bairro is null)
        {
            return;
        }

        Console.Write("Digite a cidade do cliente: ");
        string? cidade = Console.ReadLine();
        if (cidade is null)
        {
            return;
        }

        Console.Write("Digite o estado do cliente: ");
        string? estado = Console.ReadLine();
        if (estado is null)
        {
            return;
        }

        int id = clientes.Keys.DefaultIfEmpty().Max() + 1;
        int idEndereco = enderecos.Keys.DefaultIfEmpty().Max() + 1;
        Endereco endereco = new(idEndereco, bairro, cidade, complemento, estado, rua, numero);
        Cliente cliente = new(id, nome, cpf, email, profissao, telefone, endereco);
        clientes[id] = cliente;
        enderecos[idEndereco] = endereco;
        persistencia.SalvarEnderecos(enderecos.Values);
        persistencia.SalvarClientes(clientes.Values);
        Console.WriteLine($"Usuário {nome} ({cpf}) cadastrado com sucesso. ID: {id}.");
    }
}