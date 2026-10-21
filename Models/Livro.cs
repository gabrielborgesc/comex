namespace Comex.Models;

public class Livro : Produto
{
    public Livro(
        int id,
        string nome,
        string descricao,
        decimal precoUnitario,
        int quantidade,
        string isbn,
        int totalPaginas)
        : base(id, nome, descricao, precoUnitario, quantidade)
    {
        Isbn = isbn;
        TotalPaginas = totalPaginas;
    }

    public string Isbn { get; set; }

    public int TotalPaginas { get; set; }
}