using ERP.Application.Common.Interfaces;
using ERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Infrastructure.Services;

public class ModuloService : IModuloService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ICurrentEmpresaService _empresaService;
    private HashSet<string>? _cacheModulos;
    private int? _cacheEmpresaId;

    public ModuloService(IServiceScopeFactory scopeFactory, ICurrentEmpresaService empresaService)
    {
        _scopeFactory = scopeFactory;
        _empresaService = empresaService;
    }

    public async Task<bool> ModuloAtivoAsync(string moduloCodigo, CancellationToken cancellationToken = default)
    {
        var ativos = await GetModulosAtivosAsync(cancellationToken);
        return ativos.Contains(moduloCodigo);
    }

    public async Task<HashSet<string>> GetModulosAtivosAsync(CancellationToken cancellationToken = default)
    {
        var empresaId = _empresaService.EmpresaId;

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (empresaId == null)
        {
            empresaId = await db.Empresas.AsNoTracking()
                .OrderBy(e => e.Id)
                .Select(e => (int?)e.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (empresaId == null)
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        if (_cacheModulos != null && _cacheEmpresaId == empresaId)
            return _cacheModulos;

        var agora = DateTime.UtcNow;
        var codigos = await db.EmpresaModulos
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(em => em.EmpresaId == empresaId.Value
                         && em.Ativo
                         && (em.DataExpiracao == null || em.DataExpiracao > agora))
            .Select(em => em.ModuloCodigo)
            .ToListAsync(cancellationToken);

        _cacheEmpresaId = empresaId;
        _cacheModulos = new HashSet<string>(codigos, StringComparer.OrdinalIgnoreCase);
        return _cacheModulos;
    }

    public void InvalidarCache()
    {
        _cacheModulos = null;
        _cacheEmpresaId = null;
    }
}
