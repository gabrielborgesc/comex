using Comex.Menus;
using Comex.Tests;

namespace Comex;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length > 0 && string.Equals(args[0], "--teste-cliente", StringComparison.OrdinalIgnoreCase))
        {
            TesteCliente.Executar();
            return;
        }

        if (args.Length > 0 && string.Equals(args[0], "--teste-produto", StringComparison.OrdinalIgnoreCase))
        {
            TesteProduto.Executar();
            return;
        }

        new MenuPrincipal().Executar();
    }
}