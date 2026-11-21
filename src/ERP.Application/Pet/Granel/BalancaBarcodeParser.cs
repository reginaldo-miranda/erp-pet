namespace ERP.Application.Pet.Granel;

public record BalancaBarcodeResult(
    bool EhEtiquetaBalanca,
    string? CodigoProduto,
    decimal? PesoKg,
    decimal? ValorTotal);

/// <summary>
/// Parser para etiquetas EAN-13 de balança (Toledo/Filizola padrão Brasil, iniciadas por '2').
/// Formatos comuns (13 dígitos):
/// - 2 + CCCC (4 dígitos código) + PPPPP (5 dígitos peso em gramas) + D (verificador) -> 2 + 6 dígitos?
/// Padrão mais comum 13 dígitos: 2 + CCCCCC (6 dígitos código) + PPPPP (5 dígitos peso/preço) + DV (1 dígito)
/// ou 2 + CCCC (4 dígitos) + 00 + PPPPP (5 dígitos) + DV.
/// </summary>
public static class BalancaBarcodeParser
{
    public static BalancaBarcodeResult Parse(string? codigoBarras, bool valorEhPeso = true)
    {
        if (string.IsNullOrWhiteSpace(codigoBarras))
            return new BalancaBarcodeResult(false, null, null, null);

        var limpo = codigoBarras.Trim();
        if (limpo.Length != 13 || !limpo.StartsWith('2') || !limpo.All(char.IsDigit))
            return new BalancaBarcodeResult(false, null, null, null);

        // Padrão 2 + CCCCCC (pos 1..6) + VVVVV (pos 7..11) + DV (pos 12)
        var codigoProdutoRaw = limpo.Substring(1, 6).TrimStart('0');
        if (string.IsNullOrEmpty(codigoProdutoRaw))
            codigoProdutoRaw = "0";

        var valorRaw = limpo.Substring(7, 5);
        if (!decimal.TryParse(valorRaw, out var valorInt))
            return new BalancaBarcodeResult(false, null, null, null);

        if (valorEhPeso)
        {
            // Peso em gramas -> kg (dividido por 1000)
            return new BalancaBarcodeResult(true, codigoProdutoRaw, valorInt / 1000m, null);
        }

        // Preço em centavos -> R$ (dividido por 100)
        return new BalancaBarcodeResult(true, codigoProdutoRaw, null, valorInt / 100m);
    }
}
