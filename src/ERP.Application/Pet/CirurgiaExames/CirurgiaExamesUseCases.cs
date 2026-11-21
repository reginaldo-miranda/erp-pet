using ERP.Application.Common.Behaviors;
using ERP.Application.Common.Interfaces;
using ERP.Application.Common.Models;
using ERP.Domain.Core.Entities.Pet;
using ERP.Domain.Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Pet.CirurgiaExames;

public record CirurgiaDto(
    int Id,
    string Numero,
    string NomePet,
    string NomeTutor,
    string NomeCirurgiao,
    string? AnestesistaNome,
    string? SalaCirurgica,
    string Procedimento,
    DateTime DataHoraAgendada,
    StatusCirurgiaVet Status,
    string? ProtocoloAnestesico,
    decimal ValorProcedimento);

public record TermoDto(
    int Id,
    string NomePet,
    string NomeTutor,
    TipoTermoConsentimento Tipo,
    string Titulo,
    bool Assinado,
    DateTime? DataAssinatura,
    string? AssinadoPorNome);

public record ExameDto(
    int Id,
    string NomePet,
    TipoExameVet Tipo,
    string NomeExame,
    string CodigoAmostraLis,
    StatusExameVet Status,
    DateTime DataSolicitacao,
    DateTime? DataResultado,
    string? ResultadoTexto);

// ==================== CIRURGIAS ====================

[RequerModulo(ModulosPet.VetCirurgico)]
public record GetCirurgiasQuery : IRequest<Result<List<CirurgiaDto>>>;

public class GetCirurgiasQueryHandler : IRequestHandler<GetCirurgiasQuery, Result<List<CirurgiaDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetCirurgiasQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<CirurgiaDto>>> Handle(GetCirurgiasQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.CirurgiasVeterinarias.AsNoTracking()
            .Include(c => c.Pet)
            .Include(c => c.Cliente)
            .Include(c => c.VeterinarioCirurgiao)
            .OrderByDescending(c => c.DataHoraAgendada)
            .Take(100)
            .Select(c => new CirurgiaDto(
                c.Id, c.Numero, c.Pet.Nome, c.Cliente.Nome, c.VeterinarioCirurgiao.Nome,
                c.AnestesistaNome, c.SalaCirurgica, c.Procedimento, c.DataHoraAgendada,
                c.Status, c.ProtocoloAnestesico, c.ValorProcedimento))
            .ToListAsync(cancellationToken);

        return Result<List<CirurgiaDto>>.Ok(list);
    }
}

[RequerModulo(ModulosPet.VetCirurgico)]
public record AgendarCirurgiaCommand(
    int PetId,
    int ClienteId,
    int VeterinarioCirurgiaoId,
    string? AnestesistaNome,
    string? SalaCirurgica,
    string Procedimento,
    DateTime DataHoraAgendada,
    string? ProtocoloAnestesico,
    decimal ValorProcedimento) : IRequest<Result<int>>;

public class AgendarCirurgiaCommandHandler : IRequestHandler<AgendarCirurgiaCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public AgendarCirurgiaCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(AgendarCirurgiaCommand request, CancellationToken cancellationToken)
    {
        var cir = new CirurgiaVeterinaria
        {
            EmpresaId = _empresaService.EmpresaId ?? 0,
            Numero = $"CIR-{DateTime.UtcNow:yyyyMMddHHmmss}",
            PetId = request.PetId,
            ClienteId = request.ClienteId,
            VeterinarioCirurgiaoId = request.VeterinarioCirurgiaoId,
            AnestesistaNome = request.AnestesistaNome,
            SalaCirurgica = request.SalaCirurgica,
            Procedimento = request.Procedimento.Trim(),
            DataHoraAgendada = request.DataHoraAgendada.Kind == DateTimeKind.Utc ? request.DataHoraAgendada : request.DataHoraAgendada.ToUniversalTime(),
            Status = StatusCirurgiaVet.Agendada,
            ProtocoloAnestesico = request.ProtocoloAnestesico,
            ValorProcedimento = request.ValorProcedimento
        };

        _context.CirurgiasVeterinarias.Add(cir);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(cir.Id);
    }
}

// ==================== TERMOS DE CONSENTIMENTO ====================

[RequerModulo(ModulosPet.VetCirurgico)]
public record GetTermosConsentimentoQuery : IRequest<Result<List<TermoDto>>>;

public class GetTermosConsentimentoQueryHandler : IRequestHandler<GetTermosConsentimentoQuery, Result<List<TermoDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetTermosConsentimentoQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<TermoDto>>> Handle(GetTermosConsentimentoQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.TermosConsentimentoPet.AsNoTracking()
            .Include(t => t.Pet)
            .Include(t => t.Cliente)
            .OrderByDescending(t => t.Id)
            .Take(100)
            .Select(t => new TermoDto(t.Id, t.Pet.Nome, t.Cliente.Nome, t.Tipo, t.Titulo, t.Assinado, t.DataAssinatura, t.AssinadoPorNome))
            .ToListAsync(cancellationToken);

        return Result<List<TermoDto>>.Ok(list);
    }
}

[RequerModulo(ModulosPet.VetCirurgico)]
public record CriarTermoConsentimentoCommand(int PetId, int ClienteId, TipoTermoConsentimento Tipo, string Titulo, string ConteudoTermo) : IRequest<Result<int>>;

public class CriarTermoConsentimentoCommandHandler : IRequestHandler<CriarTermoConsentimentoCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public CriarTermoConsentimentoCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(CriarTermoConsentimentoCommand request, CancellationToken cancellationToken)
    {
        var termo = new TermoConsentimentoPet
        {
            EmpresaId = _empresaService.EmpresaId ?? 0,
            PetId = request.PetId,
            ClienteId = request.ClienteId,
            Tipo = request.Tipo,
            Titulo = request.Titulo.Trim(),
            ConteudoTermo = request.ConteudoTermo
        };
        _context.TermosConsentimentoPet.Add(termo);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(termo.Id);
    }
}

public record AssinarTermoConsentimentoCommand(int TermoId, string AssinadoPorNome) : IRequest<Result<bool>>;

public class AssinarTermoConsentimentoCommandHandler : IRequestHandler<AssinarTermoConsentimentoCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    public AssinarTermoConsentimentoCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(AssinarTermoConsentimentoCommand request, CancellationToken cancellationToken)
    {
        var termo = await _context.TermosConsentimentoPet.FirstOrDefaultAsync(t => t.Id == request.TermoId, cancellationToken);
        if (termo == null) return Result<bool>.Falha("Termo não encontrado.");

        termo.Assinado = true;
        termo.DataAssinatura = DateTime.UtcNow;
        termo.AssinadoPorNome = request.AssinadoPorNome.Trim();
        termo.TokenAssinatura = Guid.NewGuid().ToString("N")[..16].ToUpperInvariant();

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}

// ==================== EXAMES & HL7/LIS ====================

[RequerModulo(ModulosPet.VetCirurgico)]
public record GetExamesVetQuery : IRequest<Result<List<ExameDto>>>;

public class GetExamesVetQueryHandler : IRequestHandler<GetExamesVetQuery, Result<List<ExameDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetExamesVetQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<ExameDto>>> Handle(GetExamesVetQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.ExamesVeterinarios.AsNoTracking()
            .Include(e => e.Pet)
            .OrderByDescending(e => e.DataSolicitacao)
            .Take(100)
            .Select(e => new ExameDto(e.Id, e.Pet.Nome, e.Tipo, e.NomeExame, e.CodigoAmostraLis, e.Status, e.DataSolicitacao, e.DataResultado, e.ResultadoTexto))
            .ToListAsync(cancellationToken);

        return Result<List<ExameDto>>.Ok(list);
    }
}

[RequerModulo(ModulosPet.VetCirurgico)]
public record SolicitarExameVetCommand(int PetId, int? VeterinarioSolicitanteId, TipoExameVet Tipo, string NomeExame, string CodigoAmostraLis) : IRequest<Result<int>>;

public class SolicitarExameVetCommandHandler : IRequestHandler<SolicitarExameVetCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public SolicitarExameVetCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(SolicitarExameVetCommand request, CancellationToken cancellationToken)
    {
        var exame = new ExameVeterinario
        {
            EmpresaId = _empresaService.EmpresaId ?? 0,
            PetId = request.PetId,
            VeterinarioSolicitanteId = request.VeterinarioSolicitanteId,
            Tipo = request.Tipo,
            NomeExame = request.NomeExame.Trim(),
            CodigoAmostraLis = string.IsNullOrWhiteSpace(request.CodigoAmostraLis) ? $"LIS-{DateTime.UtcNow:HHmmss}" : request.CodigoAmostraLis.Trim(),
            Status = StatusExameVet.Solicitado,
            DataSolicitacao = DateTime.UtcNow
        };
        _context.ExamesVeterinarios.Add(exame);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(exame.Id);
    }
}

[RequerModulo(ModulosPet.VetLabHl7)]
public record ProcessarPayloadHl7Command(string PayloadHl7) : IRequest<Result<string>>;

public class ProcessarPayloadHl7CommandHandler : IRequestHandler<ProcessarPayloadHl7Command, Result<string>>
{
    private readonly IApplicationDbContext _context;
    public ProcessarPayloadHl7CommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<string>> Handle(ProcessarPayloadHl7Command request, CancellationToken cancellationToken)
    {
        var parsed = Hl7MessageParser.Parse(request.PayloadHl7);
        if (!parsed.Valido || string.IsNullOrWhiteSpace(parsed.CodigoAmostraLis))
            return Result<string>.Falha("Payload HL7 inválido ou sem código de amostra OBR.");

        var exame = await _context.ExamesVeterinarios
            .FirstOrDefaultAsync(e => e.CodigoAmostraLis == parsed.CodigoAmostraLis, cancellationToken);

        if (exame == null)
            return Result<string>.Falha($"Nenhum exame encontrado para amostra LIS '{parsed.CodigoAmostraLis}'.");

        exame.PayloadHl7Bruto = request.PayloadHl7;
        exame.ResultadoTexto = parsed.ResumoResultados;
        exame.Status = StatusExameVet.ResultadoRecebido;
        exame.DataResultado = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<string>.Ok($"Resultado integrado ao exame '{exame.NomeExame}' (Amostra {exame.CodigoAmostraLis}).");
    }
}
