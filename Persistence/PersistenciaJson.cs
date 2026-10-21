using System.Text.Json;
using Comex.Models;

namespace Comex.Persistence;

internal sealed class PersistenciaJson
{
    private readonly string _diretorioDb;
    private readonly JsonSerializerOptions _opcoesJson = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public PersistenciaJson()
    {
        _diretorioDb = Path.Combine(EncontrarRaizDoProjeto(), "db");
        Directory.CreateDirectory(_diretorioDb);
    }

    public List<Produto> CarregarProdutos() => CarregarLista<Produto>("produtos.txt");

    public List<Endereco> CarregarEnderecos() => CarregarLista<Endereco>("enderecos.txt");

    public List<Carrinho> CarregarCarrinhos() => CarregarLista<Carrinho>("carrinhos.txt");

    public Dictionary<int, Cliente> CarregarClientes(IReadOnlyDictionary<int, Endereco> enderecos)
    {
        List<ClientePersistido> clientesPersistidos = CarregarLista<ClientePersistido>("clientes.txt");
        Dictionary<int, Cliente> clientes = new();

        foreach (ClientePersistido clientePersistido in clientesPersistidos)
        {
            if (!enderecos.TryGetValue(clientePersistido.EnderecoId, out Endereco? endereco))
            {
                throw new InvalidDataException($"O endereço {clientePersistido.EnderecoId} do cliente {clientePersistido.Id} não foi encontrado.");
            }

            Cliente cliente = new(
                clientePersistido.Id,
                clientePersistido.Nome,
                clientePersistido.Cpf,
                clientePersistido.Email,
                clientePersistido.Profissao,
                clientePersistido.Telefone,
                endereco);
            clientes.Add(cliente.Id, cliente);
        }

        return clientes;
    }

    public void SalvarProdutos(IEnumerable<Produto> produtos) => SalvarLista("produtos.txt", produtos);

    public void SalvarEnderecos(IEnumerable<Endereco> enderecos) => SalvarLista("enderecos.txt", enderecos);

    public void SalvarCarrinhos(IEnumerable<Carrinho> carrinhos) => SalvarLista("carrinhos.txt", carrinhos);

    public void SalvarClientes(IEnumerable<Cliente> clientes)
    {
        List<ClientePersistido> clientesPersistidos = clientes
            .Select(cliente => new ClientePersistido(
                cliente.Id,
                cliente.Nome,
                cliente.Cpf,
                cliente.Email,
                cliente.Profissao,
                cliente.Telefone,
                cliente.Endereco.Id))
            .ToList();

        SalvarLista("clientes.txt", clientesPersistidos);
    }

    private List<T> CarregarLista<T>(string nomeArquivo)
    {
        string caminho = Path.Combine(_diretorioDb, nomeArquivo);
        if (!File.Exists(caminho))
        {
            return new List<T>();
        }

        string json = File.ReadAllText(caminho);
        return JsonSerializer.Deserialize<List<T>>(json, _opcoesJson) ?? new List<T>();
    }

    private void SalvarLista<T>(string nomeArquivo, IEnumerable<T> dados)
    {
        string caminho = Path.Combine(_diretorioDb, nomeArquivo);
        string caminhoTemporario = $"{caminho}.tmp";
        string json = JsonSerializer.Serialize(dados.ToList(), _opcoesJson);
        File.WriteAllText(caminhoTemporario, json);
        File.Move(caminhoTemporario, caminho, true);
    }

    private static string EncontrarRaizDoProjeto()
    {
        foreach (string diretorioInicial in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
        {
            DirectoryInfo? diretorio = new(diretorioInicial);
            while (diretorio is not null)
            {
                if (File.Exists(Path.Combine(diretorio.FullName, "Comex.csproj")))
                {
                    return diretorio.FullName;
                }

                diretorio = diretorio.Parent;
            }
        }

        return Directory.GetCurrentDirectory();
    }

    private sealed record ClientePersistido(
        int Id,
        string Nome,
        string Cpf,
        string Email,
        string Profissao,
        string Telefone,
        int EnderecoId);
}