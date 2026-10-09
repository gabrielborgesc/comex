internal class Program
{
    private static void Main()
    {
        ExibirMenuDeOpcoes();
    }

    private static void ExibirMenuDeOpcoes()
    {
        Dictionary<string, string> clientes = new();

        while (true)
        {
            Console.WriteLine("\n=== COMEX ===");
            Console.WriteLine("1 - Cadastrar cliente");
            Console.WriteLine("2 - Listar clientes");
            Console.WriteLine("-1 - Sair");
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

            Console.WriteLine($"Opção escolhida: {opcao}");

            if (opcao == -1)
            {
                Console.WriteLine("COMEX FINALIZADO!");
                return;
            }

            if (opcao == 1)
            {
                CadastraCliente(clientes);
            }
            else if (opcao == 2)
            {
                ListaClientes(clientes);
            }
        }
    }

    private static void ListaClientes(Dictionary<string, string> clientes)
    {
        if (clientes.Count == 0)
        {
            Console.WriteLine("Nenhum cliente cadastrado.");
            return;
        }

        foreach (KeyValuePair<string, string> cliente in clientes)
        {
            Console.WriteLine($"CPF: {cliente.Key} | Nome: {cliente.Value}");
        }
    }

    private static void CadastraCliente(Dictionary<string, string> clientes)
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

        if (clientes.ContainsKey(cpf))
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

        clientes[cpf] = nome;
        Console.WriteLine($"Usuário {nome} ({cpf}) cadastrado com sucesso.");
    }
}