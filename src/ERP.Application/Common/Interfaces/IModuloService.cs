namespace ERP.Application.Common.Interfaces;

/// <summary>
/// Códigos padronizados dos módulos do ERP Pet.
/// </summary>
public static class ModulosPet
{
    public const string LojaGranel = "LojaGranel";
    public const string BanhoTosa = "BanhoTosa";
    public const string VetAmbulatorio = "VetAmbulatorio";
    public const string VetInternacao = "VetInternacao";
    public const string VetCirurgico = "VetCirurgico";
    public const string VetLabHl7 = "VetLabHl7";
    public const string PortalTutor = "PortalTutor";
    public const string WhatsApp = "WhatsApp";
    public const string ConveniosPet = "ConveniosPet";
}

/// <summary>
/// Serviço para verificação de módulos ativos da empresa atual (Feature Flags SaaS).
/// </summary>
public interface IModuloService
{
    Task<bool> ModuloAtivoAsync(string moduloCodigo, CancellationToken cancellationToken = default);
    Task<HashSet<string>> GetModulosAtivosAsync(CancellationToken cancellationToken = default);
    void InvalidarCache();
}
