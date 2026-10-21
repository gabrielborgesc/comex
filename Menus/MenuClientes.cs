using Comex.Models;
using Comex.Persistence;

namespace Comex.Menus;

internal sealed class MenuClientes
{
    private readonly Dictionary<int, Cliente> _clientes;
    private readonly Dictionary<int, Endereco> _enderecos;
    private readonly PersistenciaJson _persistencia;

    public MenuClientes(
        Dictionary<int, Cliente> clientes,
        Dictionary<int, Endereco> enderecos,
        PersistenciaJson persistencia)
    {
        _clientes = clientes;
        _enderecos = enderecos;
        _persistencia = persistencia;
    }

    public void Cadastrar()
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

        if (_clientes.Values.Any(cliente => cliente.Cpf == cpf))
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

        int id = _clientes.Keys.DefaultIfEmpty().Max() + 1;
        int idEndereco = _enderecos.Keys.DefaultIfEmpty().Max() + 1;
        Endereco endereco = new(idEndereco, bairro, cidade, complemento, estado, rua, numero);
        Cliente cliente = new(id, nome, cpf, email, profissao, telefone, endereco);
        _clientes[id] = cliente;
        _enderecos[idEndereco] = endereco;
        _persistencia.SalvarEnderecos(_enderecos.Values);
        _persistencia.SalvarClientes(_clientes.Values);
        Console.WriteLine($"Usuário {nome} ({cpf}) cadastrado com sucesso. ID: {id}.");
    }

    public void Listar()
    {
        if (_clientes.Count == 0)
        {
            Console.WriteLine("Nenhum cliente cadastrado.");
            return;
        }

        foreach (Cliente cliente in _clientes.Values)
        {
            Console.WriteLine($"ID: {cliente.Id} | CPF: {cliente.Cpf} | Nome: {cliente.Nome} | E-mail: {cliente.Email} | Profissão: {cliente.Profissao} | Telefone: {cliente.Telefone}");
            Console.WriteLine($"Endereço: Rua: {cliente.Endereco.Rua}, {cliente.Endereco.Numero} | {cliente.Endereco.Complemento} | {cliente.Endereco.Bairro} | {cliente.Endereco.Cidade} - {cliente.Endereco.Estado}");
        }
    }

    public Cliente? Selecionar(
        string mensagemNaoCadastrado = "Usuário não cadastrado.",
        string mensagemListaVazia = "Nenhum cliente cadastrado.")
    {
        if (_clientes.Count == 0)
        {
            Console.WriteLine(mensagemListaVazia);
            return null;
        }

        Listar();
        Console.Write("Digite o ID ou CPF do cliente: ");
        string? identificador = Console.ReadLine();
        if (identificador is null)
        {
            return null;
        }

        string cpf = new(identificador.Where(char.IsAsciiDigit).ToArray());
        Cliente? cliente = _clientes.Values.FirstOrDefault(item => item.Cpf == cpf);

        if (cliente is null
            && int.TryParse(identificador, out int idCliente)
            && _clientes.TryGetValue(idCliente, out Cliente? clientePorId))
        {
            cliente = clientePorId;
        }

        if (cliente is null)
        {
            Console.WriteLine(mensagemNaoCadastrado);
        }

        return cliente;
    }
}