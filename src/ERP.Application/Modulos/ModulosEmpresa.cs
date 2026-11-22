using ERP.Application.Common.Interfaces;
using ERP.Application.Common.Models;
using ERP.Domain.Core.Entities;
using ERP.Domain.Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Modulos;

public record ModuloEmpresaDto(
    string Codigo,
    string Nome,
    string Descricao,
    string Categoria,
    bool Ativo,
    OrigemModuloEmpresa Origem,
    DateTime? DataAtivacao);

public record ConfiguracaoModulosEmpresaDto(
    int EmpresaId,
    string NomeEmpresa,
    PlanoLicenca PlanoAtual,
    List<ModuloEmpresaDto> Modulos);

public static class PlanosModulosPreset
{
    public static HashSet<string> ObterModulosDoPlano(PlanoLicenca plano) => plano switch
    {
        PlanoLicenca.Pequeno => new(StringComparer.OrdinalIgnoreCase)
        {
            ModulosPet.LojaGranel,
            ModulosPet.BanhoTosa
        },
        PlanoLicenca.Medio => new(StringComparer.OrdinalIgnoreCase)
        {
            ModulosPet.LojaGranel,
            ModulosPet.BanhoTosa,
            ModulosPet.VetAmbulatorio,
            ModulosPet.PortalTutor,
            ModulosPet.WhatsApp
        },
        PlanoLicenca.Grande => new(StringComparer.OrdinalIgnoreCase)
        {
            ModulosPet.LojaGranel,
            ModulosPet.BanhoTosa,
            ModulosPet.VetAmbulatorio,
            ModulosPet.VetInternacao,
            ModulosPet.VetCirurgico,
            ModulosPet.VetLabHl7,
            ModulosPet.PortalTutor,
            ModulosPet.WhatsApp,
            ModulosPet.ConveniosPet
        },
        _ => new(StringComparer.OrdinalIgnoreCase)
    };
}

public record GetModulosEmpresaQuery : IRequest<Result<ConfiguracaoModulosEmpresaDto>>;

public class GetModulosEmpresaQueryHandler : IRequestHandler<GetModulosEmpresaQuery, Result<ConfiguracaoModulosEmpresaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;

    public GetModulosEmpresaQueryHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<ConfiguracaoModulosEmpresaDto>> Handle(GetModulosEmpresaQuery request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaService.EmpresaId
            ?? await _context.Empresas.AsNoTracking().OrderBy(e => e.Id).Select(e => (int?)e.Id).FirstOrDefaultAsync(cancellationToken);
        if (empresaId == null)
            return Result<ConfiguracaoModulosEmpresaDto>.Falha("Nenhuma empresa encontrada.");

        var empresa = await _context.Empresas.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == empresaId.Value, cancellationToken);
        if (empresa == null)
            return Result<ConfiguracaoModulosEmpresaDto>.Falha("Empresa não encontrada.");

        var catalogo = await _context.ModulosSistema.AsNoTracking()
            .Where(m => m.Ativo)
            .OrderBy(m => m.Ordem)
            .ToListAsync(cancellationToken);

        var ativos = await _context.EmpresaModulos.AsNoTracking()
            .Where(em => em.EmpresaId == empresaId.Value)
            .ToDictionaryAsync(em => em.ModuloCodigo, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var itens = catalogo.Select(m =>
        {
            ativos.TryGetValue(m.Codigo, out var em);
            return new ModuloEmpresaDto(
                m.Codigo,
                m.Nome,
                m.Descricao,
                m.Categoria,
                em?.Ativo ?? false,
                em?.Origem ?? OrigemModuloEmpresa.Plano,
                em?.DataAtivacao);
        }).ToList();

        return Result<ConfiguracaoModulosEmpresaDto>.Ok(new ConfiguracaoModulosEmpresaDto(
            empresa.Id,
            empresa.NomeFantasia ?? empresa.RazaoSocial,
            empresa.PlanoLicenca,
            itens));
    }
}

public record AplicarPlanoEmpresaCommand(PlanoLicenca Plano) : IRequest<Result<bool>>;

public class AplicarPlanoEmpresaCommandHandler : IRequestHandler<AplicarPlanoEmpresaCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    private readonly IModuloService _moduloService;

    public AplicarPlanoEmpresaCommandHandler(
        IApplicationDbContext context,
        ICurrentEmpresaService empresaService,
        IModuloService moduloService)
    {
        _context = context;
        _empresaService = empresaService;
        _moduloService = moduloService;
    }

    public async Task<Result<bool>> Handle(AplicarPlanoEmpresaCommand request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaService.EmpresaId
            ?? await _context.Empresas.AsNoTracking().OrderBy(e => e.Id).Select(e => (int?)e.Id).FirstOrDefaultAsync(cancellationToken);
        if (empresaId == null)
            return Result<bool>.Falha("Nenhuma empresa encontrada.");

        var empresa = await _context.Empresas.FirstOrDefaultAsync(e => e.Id == empresaId.Value, cancellationToken);
        if (empresa == null)
            return Result<bool>.Falha("Empresa não encontrada.");

        empresa.PlanoLicenca = request.Plano;

        var modulosPlano = PlanosModulosPreset.ObterModulosDoPlano(request.Plano);
        var catalogo = await _context.ModulosSistema.AsNoTracking().ToListAsync(cancellationToken);
        var existentes = await _context.EmpresaModulos
            .Where(em => em.EmpresaId == empresaId.Value)
            .ToListAsync(cancellationToken);

        foreach (var mod in catalogo)
        {
            var deveAtivar = modulosPlano.Contains(mod.Codigo);
            var registro = existentes.FirstOrDefault(e => e.ModuloCodigo.Equals(mod.Codigo, StringComparison.OrdinalIgnoreCase));
            if (registro == null)
            {
                if (deveAtivar)
                {
                    _context.EmpresaModulos.Add(new EmpresaModulo
                    {
                        EmpresaId = empresaId.Value,
                        ModuloCodigo = mod.Codigo,
                        Ativo = true,
                        Origem = OrigemModuloEmpresa.Plano,
                        DataAtivacao = DateTime.UtcNow
                    });
                }
            }
            else if (registro.Origem == OrigemModuloEmpresa.Plano || !deveAtivar)
            {
                registro.Ativo = deveAtivar;
                registro.Origem = OrigemModuloEmpresa.Plano;
                if (deveAtivar)
                    registro.DataAtivacao = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        _moduloService.InvalidarCache();
        return Result<bool>.Ok(true);
    }
}

public record ToggleModuloEmpresaCommand(string ModuloCodigo, bool Ativo) : IRequest<Result<bool>>;

public class ToggleModuloEmpresaCommandHandler : IRequestHandler<ToggleModuloEmpresaCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    private readonly IModuloService _moduloService;

    public ToggleModuloEmpresaCommandHandler(
        IApplicationDbContext context,
        ICurrentEmpresaService empresaService,
        IModuloService moduloService)
    {
        _context = context;
        _empresaService = empresaService;
        _moduloService = moduloService;
    }

    public async Task<Result<bool>> Handle(ToggleModuloEmpresaCommand request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaService.EmpresaId
            ?? await _context.Empresas.AsNoTracking().OrderBy(e => e.Id).Select(e => (int?)e.Id).FirstOrDefaultAsync(cancellationToken);
        if (empresaId == null)
            return Result<bool>.Falha("Nenhuma empresa encontrada.");

        var existeCatalogo = await _context.ModulosSistema.AnyAsync(m => m.Codigo == request.ModuloCodigo, cancellationToken);
        if (!existeCatalogo)
            return Result<bool>.Falha($"Módulo '{request.ModuloCodigo}' não encontrado no catálogo.");

        var registro = await _context.EmpresaModulos
            .FirstOrDefaultAsync(em => em.EmpresaId == empresaId.Value && em.ModuloCodigo == request.ModuloCodigo, cancellationToken);

        if (registro == null)
        {
            _context.EmpresaModulos.Add(new EmpresaModulo
            {
                EmpresaId = empresaId.Value,
                ModuloCodigo = request.ModuloCodigo,
                Ativo = request.Ativo,
                Origem = OrigemModuloEmpresa.AddOn,
                DataAtivacao = DateTime.UtcNow
            });
        }
        else
        {
            registro.Ativo = request.Ativo;
            registro.Origem = OrigemModuloEmpresa.AddOn;
            if (request.Ativo)
                registro.DataAtivacao = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        _moduloService.InvalidarCache();
        return Result<bool>.Ok(true);
    }
}
