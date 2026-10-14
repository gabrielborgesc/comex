internal static class TesteCliente
{
    public static void Executar()
    {
        Endereco endereco = new(1, "Centro", "São Paulo", "Apto 42", "SP", "Rua Exemplo", 123);
        Cliente cliente = new(1, "Ana Souza", "12345678900", "ana@example.com", "Analista", "11987654321", endereco);

        Console.WriteLine($"Cliente: {cliente.Nome}");
        Console.WriteLine($"CPF: {cliente.Cpf}");
        Console.WriteLine($"E-mail: {cliente.Email}");
        Console.WriteLine($"Profissão: {cliente.Profissao}");
        Console.WriteLine($"Telefone: {cliente.Telefone}");
        Console.WriteLine($"Endereço: Rua: {cliente.Endereco.Rua}, {cliente.Endereco.Numero}, {cliente.Endereco.Complemento}, {cliente.Endereco.Bairro}, {cliente.Endereco.Cidade} - {cliente.Endereco.Estado}");
    }
}