using Comex.Menus;

namespace Comex;

internal class Program
{
    private static async Task Main()
    {
        Dictionary<int, Menu> menus = new()
        {
            [12] = new MenuGerarRelatorioCsv()
        };

        await new MenuPrincipal(menus).ExecutarAsync();
    }
}