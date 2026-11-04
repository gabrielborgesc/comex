using Comex.GeneradorRelatorio.Models;

namespace Comex.GeneradorRelatorio;

/// <summary>
/// Define o contrato para exportar produtos em um arquivo de relatório.
/// </summary>
public interface IGeradorRelatorio
{
    /// <summary>
    /// Exporta os produtos para o caminho informado.
    /// </summary>
    /// <param name="produtos">Produtos que serão exportados.</param>
    /// <param name="caminhoArquivo">Caminho de destino do arquivo.</param>
    /// <returns>O caminho absoluto do arquivo gerado.</returns>
    string Exportar(IEnumerable<ProdutoRelatorioDto> produtos, string caminhoArquivo);
}