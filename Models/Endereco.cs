namespace Comex.Models;

public class Endereco
{
    public Endereco(int id, string bairro, string cidade, string complemento, string estado, string rua, int numero)
    {
        Id = id;
        Bairro = bairro;
        Cidade = cidade;
        Complemento = complemento;
        Estado = estado;
        Rua = rua;
        Numero = numero;
    }

    public int Id { get; set; }

    public string Bairro { get; set; }

    public string Cidade { get; set; }

    public string Complemento { get; set; }

    public string Estado { get; set; }

    public string Rua { get; set; }

    public int Numero { get; set; }
}