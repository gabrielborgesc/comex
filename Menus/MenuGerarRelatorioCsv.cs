using System.Text.Json;
using System.Text.Json.Serialization;
using Comex.GeneradorRelatorio;
using Comex.GeneradorRelatorio.Models;

namespace Comex.Menus;

internal sealed class MenuGerarRelatorioCsv : Menu
{
    private const string NomeArquivoProdutos = "produtos.json";
    private static readonly JsonSerializerOptions OpcoesJson = new(JsonSerializerDefaults.Web);

    public override void Executar()
    {
        string caminhoProdutos = Path.GetFullPath(NomeArquivoProdutos);
        if (!File.Exists(caminhoProdutos))
        {
            Console.WriteLine($"Arquivo não encontrado: {caminhoProdutos}");
            return;
        }

        try
        {
            string json = File.ReadAllText(caminhoProdutos);
            List<ProdutoApiDto>? produtos = JsonSerializer.Deserialize<List<ProdutoApiDto>>(json, OpcoesJson);

            if (produtos is null || produtos.Count == 0)
            {
                Console.WriteLine("O arquivo não contém produtos para exportar.");
                return;
            }

            List<ProdutoRelatorioDto> produtosRelatorio = produtos
                .Where(produto => produto.Preco > 0)
                .OrderByDescending(produto => produto.Preco)
                .Select(produto => new ProdutoRelatorioDto
                {
                    Id = produto.Id,
                    Nome = produto.Titulo,
                    Descricao = produto.Descricao,
                    PrecoUnitario = produto.Preco,
                    Quantidade = 0
                })
                .ToList();

            if (produtosRelatorio.Count == 0)
            {
                Console.WriteLine("Nenhum produto com preço válido para exportar.");
                return;
            }

            IGeradorRelatorio gerador = new GeradorCsv();
            string caminhoSalvo = gerador.Exportar(produtosRelatorio, "relatorio-produtos.csv");
            Console.WriteLine($"Relatório CSV gerado em: {caminhoSalvo}");
        }
        catch (IOException excecao)
        {
            Console.WriteLine($"Não foi possível ler produtos.json ou gravar o relatório: {excecao.Message}");
        }
        catch (UnauthorizedAccessException excecao)
        {
            Console.WriteLine($"Sem permissão para acessar os arquivos do relatório: {excecao.Message}");
        }
        catch (JsonException excecao)
        {
            Console.WriteLine($"Não foi possível desserializar produtos.json: {excecao.Message}");
        }
    }

    private sealed class ProdutoApiDto
    {
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Titulo { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Preco { get; set; }

        [JsonPropertyName("description")]
        public string Descricao { get; set; } = string.Empty;
    }
}