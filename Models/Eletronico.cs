namespace Comex.Models;

public class Eletronico : Produto
{
    public Eletronico(
        int id,
        string nome,
        string descricao,
        decimal precoUnitario,
        int quantidade,
        int voltagem,
        int potencia)
        : base(id, nome, descricao, precoUnitario, quantidade)
    {
        Voltagem = voltagem;
        Potencia = potencia;
    }

    public int Voltagem { get; set; }

    public int Potencia { get; set; }
}