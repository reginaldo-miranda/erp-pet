namespace ERP.Application.Pet.CirurgiaExames;

public record Hl7ParsedResult(bool Valido, string? CodigoAmostraLis, string? ResumoResultados);

/// <summary>
/// Parser para mensagens HL7 v2 (segmentos OBR e OBX) recebidas de analisadores/laboratórios LIS.
/// </summary>
public static class Hl7MessageParser
{
    public static Hl7ParsedResult Parse(string? rawHl7)
    {
        if (string.IsNullOrWhiteSpace(rawHl7))
            return new Hl7ParsedResult(false, null, null);

        var linhas = rawHl7.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string? amostra = null;
        var resultados = new List<string>();

        foreach (var linha in linhas)
        {
            var campos = linha.Split('|');
            if (campos.Length == 0) continue;

            if (campos[0] == "OBR" && campos.Length > 2 && string.IsNullOrEmpty(amostra))
            {
                amostra = !string.IsNullOrWhiteSpace(campos[2]) ? campos[2].Trim() :
                          (campos.Length > 3 ? campos[3].Trim() : null);
            }
            else if (campos[0] == "OBX" && campos.Length > 5)
            {
                var parametro = campos[3].Split('^').LastOrDefault()?.Trim() ?? campos[3];
                var valor = campos[5].Trim();
                var unidade = campos.Length > 6 ? campos[6].Trim() : "";
                var refRange = campos.Length > 7 ? campos[7].Trim() : "";
                resultados.Add($"{parametro}: {valor} {unidade} (Ref: {refRange})".Trim());
            }
        }

        if (string.IsNullOrWhiteSpace(amostra) && resultados.Count == 0)
            return new Hl7ParsedResult(false, null, null);

        return new Hl7ParsedResult(true, amostra, string.Join("\n", resultados));
    }
}
