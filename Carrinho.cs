public class Carrinho
{
    public int ClienteId { get; set; }

    public List<int> ProdutoIds { get; set; } = new();
}