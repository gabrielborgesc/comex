using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Comex.Menus;

internal sealed class MenuConsultaExterna
{
    private const string UriProdutos = "https://fakestoreapi.com/products";
    private static readonly HttpClient ClienteHttp = new();
    private static readonly JsonSerializerOptions OpcoesJson = new(JsonSerializerDefaults.Web);

    public async Task BuscarProdutosAsync()
    {
        Console.WriteLine("Consultando produtos na Fake Store API...");

        try
        {
            using HttpResponseMessage resposta = await ClienteHttp.GetAsync(UriProdutos);
            resposta.EnsureSuccessStatusCode();

            await using Stream conteudo = await resposta.Content.ReadAsStreamAsync();
            List<ProdutoExterno>? produtos = await JsonSerializer.DeserializeAsync<List<ProdutoExterno>>(
                conteudo,
                OpcoesJson);

            if (produtos is null || produtos.Count == 0)
            {
                Console.WriteLine("A API não retornou produtos.");
                return;
            }

            foreach (ProdutoExterno produto in produtos)
            {
                Console.WriteLine($"ID: {produto.Id} | {produto.Titulo} | Categoria: {produto.Categoria} | Preço: ${produto.Preco:F2}");
                Console.WriteLine($"Descrição: {produto.Descricao}");
            }

            Console.WriteLine($"Consulta concluída: {produtos.Count} produto(s) recebido(s).");
        }
        catch (HttpRequestException excecao)
        {
            Console.WriteLine($"Falha na consulta à API: {excecao.Message}");
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("A consulta à API excedeu o tempo limite.");
        }
        catch (JsonException excecao)
        {
            Console.WriteLine($"Não foi possível interpretar a resposta da API: {excecao.Message}");
        }
    }

    private sealed class ProdutoExterno
    {
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Titulo { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Preco { get; set; }

        [JsonPropertyName("description")]
        public string Descricao { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Categoria { get; set; } = string.Empty;
    }
}