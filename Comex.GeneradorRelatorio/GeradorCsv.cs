using System.Globalization;
using System.Text;
using Comex.GeneradorRelatorio.Models;

namespace Comex.GeneradorRelatorio;

/// <summary>
/// Gera relatórios de produtos no formato CSV.
/// </summary>
public sealed class GeradorCsv : IGeradorRelatorio
{
    /// <summary>
    /// Exporta os produtos para um arquivo CSV.
    /// </summary>
    /// <param name="produtos">Produtos que serão exportados.</param>
    /// <param name="caminhoArquivo">Caminho de destino do arquivo CSV.</param>
    /// <returns>O caminho absoluto do arquivo CSV gerado.</returns>
    public string Exportar(IEnumerable<ProdutoRelatorioDto> produtos, string caminhoArquivo)
    {
        ArgumentNullException.ThrowIfNull(produtos);
        ArgumentException.ThrowIfNullOrWhiteSpace(caminhoArquivo);

        string caminhoCompleto = Path.GetFullPath(caminhoArquivo);
        string? diretorio = Path.GetDirectoryName(caminhoCompleto);
        if (diretorio is not null)
        {
            Directory.CreateDirectory(diretorio);
        }

        using StreamWriter escritor = new(caminhoCompleto, false, new UTF8Encoding(true));
        escritor.WriteLine("Id,Nome,Descricao,PrecoUnitario,Quantidade");

        foreach (ProdutoRelatorioDto produto in produtos)
        {
            string[] campos =
            [
                produto.Id.ToString(CultureInfo.InvariantCulture),
                produto.Nome,
                produto.Descricao,
                produto.PrecoUnitario.ToString(CultureInfo.InvariantCulture),
                produto.Quantidade.ToString(CultureInfo.InvariantCulture)
            ];

            escritor.WriteLine(string.Join(",", campos.Select(EscaparCampo)));
        }

        return caminhoCompleto;
    }

    private static string EscaparCampo(string valor)
    {
        if (!valor.Contains(',') && !valor.Contains('"') && !valor.Contains('\r') && !valor.Contains('\n'))
        {
            return valor;
        }

        return $"\"{valor.Replace("\"", "\"\"")}\"";
    }
}