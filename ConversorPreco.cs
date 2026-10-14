using System.Globalization;

internal static class ConversorPreco
{
    public static bool TryParse(string? entrada, out decimal preco)
    {
        string valorNormalizado = entrada?.Trim().Replace(',', '.') ?? string.Empty;
        NumberStyles estilos = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
        return decimal.TryParse(valorNormalizado, estilos, CultureInfo.InvariantCulture, out preco);
    }
}