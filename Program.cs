using Comex.Menus;

namespace Comex;

internal class Program
{
    private static async Task Main()
    {
        await new MenuPrincipal().ExecutarAsync();
    }
}