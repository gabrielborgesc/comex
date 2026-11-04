namespace Comex.GeneradorRelatorio.Models;

/// <summary>
/// Representa os dados de um produto usados em um relatório.
/// </summary>
public sealed class ProdutoRelatorioDto
{
    /// <summary>
    /// Identificador do produto.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome do produto.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do produto.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Preço unitário do produto.
    /// </summary>
    public decimal PrecoUnitario { get; set; }

    /// <summary>
    /// Quantidade disponível do produto.
    /// </summary>
    public int Quantidade { get; set; }
}