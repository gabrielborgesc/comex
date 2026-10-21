namespace Comex.Models;

public class Cliente
{
    public Cliente(int id, string nome, string cpf, string email, string profissao, string telefone, Endereco endereco)
    {
        Id = id;
        Nome = nome;
        Cpf = cpf;
        Email = email;
        Profissao = profissao;
        Telefone = telefone;
        Endereco = endereco;
    }

    public int Id { get; set; }

    public string Nome { get; set; }

    public string Cpf { get; set; }

    public string Email { get; set; }

    public string Profissao { get; set; }

    public string Telefone { get; set; }

    public Endereco Endereco { get; set; }
}