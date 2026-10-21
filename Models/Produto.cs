namespace Comex.Models;

public class Produto
{
    public Produto(int id, string nome, string descricao, decimal precoUnitario, int quantidade)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do produto é obrigatório.", nameof(nome));
        }

        Id = id;
        Nome = nome.Trim();
        Descricao = descricao;
        PrecoUnitario = precoUnitario;
        Quantidade = quantidade;
    }

    public int Id { get; set; }

    public string Nome { get; set; }

    public string Descricao { get; set; }

    public decimal PrecoUnitario { get; set; }

    public int Quantidade { get; set; }
}