using ERP.Application.Common.Behaviors;
using ERP.Application.Common.Interfaces;
using ERP.Application.Common.Models;
using ERP.Application.Pet.Comandas;
using ERP.Domain.Core.Entities.Pet;
using ERP.Domain.Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Pet.Ambulatorio;

public record VeterinarioDto(
    int Id,
    string Nome,
    string Crmv,
    string UfCrmv,
    string? Especialidade,
    string? Telefone,
    decimal ValorConsultaPadrao);

public record ConsultaVetDto(
    int Id,
    string Numero,
    int PetId,
    string NomePet,
    int ClienteId,
    string NomeTutor,
    int VeterinarioId,
    string NomeVeterinario,
    string Crmv,
    DateTime DataHora,
    TipoConsultaVet Tipo,
    StatusConsultaVet Status,
    string? QueixaPrincipal,
    decimal? PesoKg,
    decimal? TemperaturaC,
    int? FrequenciaCardiacaBpm,
    int? FrequenciaRespiratoriaMpm,
    string? Mucosas,
    string? SoapSubjetivo,
    string? SoapObjetivo,
    string? SoapAvaliacao,
    string? SoapPlano,
    decimal ValorConsulta);

public record ReceitaItemInput(
    string Medicamento,
    string? PrincipioAtivo,
    string? Concentracao,
    string Posologia,
    string? ViaAdministracao,
    int? DuracaoDias,
    string? Quantidade);

public record ReceitaVetDto(
    int Id,
    int PetId,
    string NomePet,
    string NomeVeterinario,
    string Crmv,
    TipoReceitaVet Tipo,
    DateTime DataEmissao,
    string? Observacoes,
    List<ReceitaItemInput> Itens);

public record VacinaVermifugoDto(
    int Id,
    int PetId,
    string NomePet,
    string NomeTutor,
    string? CelularTutor,
    TipoPreventivoPet Tipo,
    string NomeProduto,
    string? Fabricante,
    string? NumeroLote,
    string? Dose,
    DateTime DataAplicacao,
    DateTime? ProximaAplicacao,
    string? NomeVeterinario);

// ==================== VETERINÁRIOS ====================

[RequerModulo(ModulosPet.VetAmbulatorio)]
public record GetVeterinariosQuery : IRequest<Result<List<VeterinarioDto>>>;

public class GetVeterinariosQueryHandler : IRequestHandler<GetVeterinariosQuery, Result<List<VeterinarioDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetVeterinariosQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<VeterinarioDto>>> Handle(GetVeterinariosQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.Veterinarios.AsNoTracking()
            .OrderBy(v => v.Nome)
            .Select(v => new VeterinarioDto(v.Id, v.Nome, v.Crmv, v.UfCrmv, v.Especialidade, v.Telefone, v.ValorConsultaPadrao))
            .ToListAsync(cancellationToken);
        return Result<List<VeterinarioDto>>.Ok(list);
    }
}

[RequerModulo(ModulosPet.VetAmbulatorio)]
public record CreateVeterinarioCommand(
    string Nome,
    string Crmv,
    string UfCrmv,
    string? Especialidade,
    string? Telefone,
    string? Email,
    decimal ValorConsultaPadrao) : IRequest<Result<int>>;

public class CreateVeterinarioCommandHandler : IRequestHandler<CreateVeterinarioCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public CreateVeterinarioCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(CreateVeterinarioCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Crmv))
            return Result<int>.Falha("Nome e CRMV do veterinário são obrigatórios.");

        var vet = new Veterinario
        {
            EmpresaId = _empresaService.EmpresaId ?? 0,
            Nome = request.Nome.Trim(),
            Crmv = request.Crmv.Trim(),
            UfCrmv = request.UfCrmv.Trim().ToUpperInvariant(),
            Especialidade = request.Especialidade,
            Telefone = request.Telefone,
            Email = request.Email,
            ValorConsultaPadrao = request.ValorConsultaPadrao
        };
        _context.Veterinarios.Add(vet);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(vet.Id);
    }
}

// ==================== CONSULTAS, TRIAGEM & PRONTUÁRIO SOAP ====================

[RequerModulo(ModulosPet.VetAmbulatorio)]
public record GetConsultasVetQuery(int? PetId = null) : IRequest<Result<List<ConsultaVetDto>>>;

public class GetConsultasVetQueryHandler : IRequestHandler<GetConsultasVetQuery, Result<List<ConsultaVetDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetConsultasVetQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<ConsultaVetDto>>> Handle(GetConsultasVetQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ConsultasVeterinarias.AsNoTracking()
            .Include(c => c.Pet)
            .Include(c => c.Cliente)
            .Include(c => c.Veterinario)
            .AsQueryable();

        if (request.PetId.HasValue)
            query = query.Where(c => c.PetId == request.PetId.Value);

        var list = await query.OrderByDescending(c => c.DataHora).Take(200)
            .Select(c => new ConsultaVetDto(
                c.Id, c.Numero, c.PetId, c.Pet.Nome, c.ClienteId, c.Cliente.Nome,
                c.VeterinarioId, c.Veterinario.Nome, $"{c.Veterinario.Crmv}/{c.Veterinario.UfCrmv}",
                c.DataHora, c.Tipo, c.Status, c.QueixaPrincipal, c.PesoKg, c.TemperaturaC,
                c.FrequenciaCardiacaBpm, c.FrequenciaRespiratoriaMpm, c.Mucosas,
                c.SoapSubjetivo, c.SoapObjetivo, c.SoapAvaliacao, c.SoapPlano, c.ValorConsulta))
            .ToListAsync(cancellationToken);

        return Result<List<ConsultaVetDto>>.Ok(list);
    }
}

[RequerModulo(ModulosPet.VetAmbulatorio)]
public record AgendarConsultaVetCommand(
    int PetId,
    int ClienteId,
    int VeterinarioId,
    int? ComandaAtendimentoId,
    DateTime DataHora,
    TipoConsultaVet Tipo,
    string? QueixaPrincipal,
    decimal? ValorConsultaOverride = null) : IRequest<Result<int>>;

public class AgendarConsultaVetCommandHandler : IRequestHandler<AgendarConsultaVetCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public AgendarConsultaVetCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(AgendarConsultaVetCommand request, CancellationToken cancellationToken)
    {
        var vet = await _context.Veterinarios.AsNoTracking().FirstOrDefaultAsync(v => v.Id == request.VeterinarioId, cancellationToken);
        if (vet == null) return Result<int>.Falha("Veterinário não encontrado.");

        var consulta = new ConsultaVeterinaria
        {
            EmpresaId = _empresaService.EmpresaId ?? 0,
            Numero = $"VET-{DateTime.UtcNow:yyyyMMddHHmmss}",
            PetId = request.PetId,
            ClienteId = request.ClienteId,
            VeterinarioId = request.VeterinarioId,
            ComandaAtendimentoId = request.ComandaAtendimentoId,
            DataHora = request.DataHora.Kind == DateTimeKind.Utc ? request.DataHora : request.DataHora.ToUniversalTime(),
            Tipo = request.Tipo,
            Status = StatusConsultaVet.Agendada,
            QueixaPrincipal = request.QueixaPrincipal,
            ValorConsulta = request.ValorConsultaOverride ?? (request.Tipo == TipoConsultaVet.Retorno ? 0m : vet.ValorConsultaPadrao)
        };

        _context.ConsultasVeterinarias.Add(consulta);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(consulta.Id);
    }
}

[RequerModulo(ModulosPet.VetAmbulatorio)]
public record RegistrarTriagemVetCommand(
    int ConsultaId,
    string? QueixaPrincipal,
    decimal? PesoKg,
    decimal? TemperaturaC,
    int? FrequenciaCardiacaBpm,
    int? FrequenciaRespiratoriaMpm,
    string? Mucosas,
    int? TpcSegundos) : IRequest<Result<bool>>;

public class RegistrarTriagemVetCommandHandler : IRequestHandler<RegistrarTriagemVetCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    public RegistrarTriagemVetCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(RegistrarTriagemVetCommand request, CancellationToken cancellationToken)
    {
        var consulta = await _context.ConsultasVeterinarias
            .Include(c => c.Pet)
            .FirstOrDefaultAsync(c => c.Id == request.ConsultaId, cancellationToken);
        if (consulta == null) return Result<bool>.Falha("Consulta não encontrada.");

        consulta.QueixaPrincipal = request.QueixaPrincipal ?? consulta.QueixaPrincipal;
        consulta.PesoKg = request.PesoKg;
        consulta.TemperaturaC = request.TemperaturaC;
        consulta.FrequenciaCardiacaBpm = request.FrequenciaCardiacaBpm;
        consulta.FrequenciaRespiratoriaMpm = request.FrequenciaRespiratoriaMpm;
        consulta.Mucosas = request.Mucosas;
        consulta.TpcSegundos = request.TpcSegundos;
        consulta.Status = StatusConsultaVet.EmAtendimento;

        if (request.PesoKg.HasValue && request.PesoKg.Value > 0)
            consulta.Pet.PesoAtualKg = request.PesoKg.Value;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}

[RequerModulo(ModulosPet.VetAmbulatorio)]
public record FinalizarProntuarioSoapCommand(
    int ConsultaId,
    string? SoapSubjetivo,
    string? SoapObjetivo,
    string? SoapAvaliacao,
    string? SoapPlano) : IRequest<Result<bool>>;

public class FinalizarProntuarioSoapCommandHandler : IRequestHandler<FinalizarProntuarioSoapCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    public FinalizarProntuarioSoapCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(FinalizarProntuarioSoapCommand request, CancellationToken cancellationToken)
    {
        var consulta = await _context.ConsultasVeterinarias
            .FirstOrDefaultAsync(c => c.Id == request.ConsultaId, cancellationToken);
        if (consulta == null) return Result<bool>.Falha("Consulta não encontrada.");

        consulta.SoapSubjetivo = request.SoapSubjetivo;
        consulta.SoapObjetivo = request.SoapObjetivo;
        consulta.SoapAvaliacao = request.SoapAvaliacao;
        consulta.SoapPlano = request.SoapPlano;
        consulta.Status = StatusConsultaVet.Finalizada;
        consulta.FinalizadaEm = DateTime.UtcNow;

        if (consulta.ComandaAtendimentoId.HasValue && consulta.ValorConsulta > 0)
        {
            var comanda = await _context.ComandasAtendimento
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.Id == consulta.ComandaAtendimentoId.Value, cancellationToken);

            if (comanda != null && comanda.Status == StatusComanda.Aberta &&
                !comanda.Itens.Any(i => i.Origem == OrigemComandaItem.HospitalAmbulatorio && i.Descricao.Contains(consulta.Numero)))
            {
                comanda.Itens.Add(new ComandaItem
                {
                    EmpresaId = comanda.EmpresaId,
                    ComandaAtendimentoId = comanda.Id,
                    Origem = OrigemComandaItem.HospitalAmbulatorio,
                    TipoItem = TipoComandaItem.Servico,
                    Descricao = $"Consulta Veterinária {consulta.Numero}",
                    Quantidade = 1,
                    ValorUnitario = consulta.ValorConsulta,
                    ValorTotal = consulta.ValorConsulta,
                    DocumentoFiscalDestino = DocumentoFiscalDestino.NFSe
                });
                AdicionarItemComandaCommandHandler.RecalcularTotais(comanda);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}

// ==================== RECEITUÁRIO ====================

[RequerModulo(ModulosPet.VetAmbulatorio)]
public record GetReceitasPetQuery(int? PetId = null) : IRequest<Result<List<ReceitaVetDto>>>;

public class GetReceitasPetQueryHandler : IRequestHandler<GetReceitasPetQuery, Result<List<ReceitaVetDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetReceitasPetQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<ReceitaVetDto>>> Handle(GetReceitasPetQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ReceitasVeterinarias.AsNoTracking()
            .Include(r => r.Pet)
            .Include(r => r.Veterinario)
            .Include(r => r.Itens)
            .AsQueryable();

        if (request.PetId.HasValue)
            query = query.Where(r => r.PetId == request.PetId.Value);

        var list = await query.OrderByDescending(r => r.DataEmissao).Take(100)
            .Select(r => new ReceitaVetDto(
                r.Id, r.PetId, r.Pet.Nome, r.Veterinario.Nome, $"{r.Veterinario.Crmv}/{r.Veterinario.UfCrmv}",
                r.Tipo, r.DataEmissao, r.Observacoes,
                r.Itens.Select(i => new ReceitaItemInput(
                    i.Medicamento, i.PrincipioAtivo, i.Concentracao, i.Posologia,
                    i.ViaAdministracao, i.DuracaoDias, i.Quantidade)).ToList()))
            .ToListAsync(cancellationToken);

        return Result<List<ReceitaVetDto>>.Ok(list);
    }
}

[RequerModulo(ModulosPet.VetAmbulatorio)]
public record EmitirReceitaVetCommand(
    int? ConsultaVeterinariaId,
    int PetId,
    int VeterinarioId,
    TipoReceitaVet Tipo,
    string? Observacoes,
    List<ReceitaItemInput> Itens) : IRequest<Result<int>>;

public class EmitirReceitaVetCommandHandler : IRequestHandler<EmitirReceitaVetCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public EmitirReceitaVetCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(EmitirReceitaVetCommand request, CancellationToken cancellationToken)
    {
        if (request.Itens.Count == 0)
            return Result<int>.Falha("Adicione ao menos um medicamento à receita.");

        var empresaId = _empresaService.EmpresaId ?? 0;
        var receita = new ReceitaVeterinaria
        {
            EmpresaId = empresaId,
            ConsultaVeterinariaId = request.ConsultaVeterinariaId,
            PetId = request.PetId,
            VeterinarioId = request.VeterinarioId,
            Tipo = request.Tipo,
            DataEmissao = DateTime.UtcNow,
            Observacoes = request.Observacoes
        };

        foreach (var item in request.Itens)
        {
            receita.Itens.Add(new ReceitaVeterinariaItem
            {
                EmpresaId = empresaId,
                Medicamento = item.Medicamento.Trim(),
                PrincipioAtivo = item.PrincipioAtivo,
                Concentracao = item.Concentracao,
                Posologia = item.Posologia.Trim(),
                ViaAdministracao = item.ViaAdministracao,
                DuracaoDias = item.DuracaoDias,
                Quantidade = item.Quantidade
            });
        }

        _context.ReceitasVeterinarias.Add(receita);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(receita.Id);
    }
}

// ==================== CARTEIRA DE VACINAS & ALERTAS ====================

[RequerModulo(ModulosPet.VetAmbulatorio)]
public record GetVacinasVermifugosQuery(int? PetId = null, bool ApenasAlertasReforco = false) : IRequest<Result<List<VacinaVermifugoDto>>>;

public class GetVacinasVermifugosQueryHandler : IRequestHandler<GetVacinasVermifugosQuery, Result<List<VacinaVermifugoDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetVacinasVermifugosQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<VacinaVermifugoDto>>> Handle(GetVacinasVermifugosQuery request, CancellationToken cancellationToken)
    {
        var query = _context.VacinacoesVermifugosPet.AsNoTracking()
            .Include(v => v.Pet)
                .ThenInclude(p => p.Tutores)
                    .ThenInclude(t => t.Cliente)
            .Include(v => v.Veterinario)
            .AsQueryable();

        if (request.PetId.HasValue)
            query = query.Where(v => v.PetId == request.PetId.Value);

        if (request.ApenasAlertasReforco)
        {
            var limite = DateTime.UtcNow.AddDays(30);
            query = query.Where(v => v.ProximaAplicacao != null && v.ProximaAplicacao <= limite);
        }

        var list = await query
            .OrderByDescending(v => v.DataAplicacao)
            .Take(200)
            .ToListAsync(cancellationToken);

        var dtos = list.Select(v =>
        {
            var tutorPrincipal = v.Pet.Tutores.FirstOrDefault(t => t.TipoVinculo == TipoVinculoTutor.Principal)
                                 ?? v.Pet.Tutores.FirstOrDefault();
            return new VacinaVermifugoDto(
                v.Id,
                v.PetId,
                v.Pet.Nome,
                tutorPrincipal?.Cliente.Nome ?? "-",
                tutorPrincipal?.Cliente.Celular ?? tutorPrincipal?.Cliente.Telefone,
                v.Tipo,
                v.NomeProduto,
                v.Fabricante,
                v.NumeroLote,
                v.Dose,
                v.DataAplicacao,
                v.ProximaAplicacao,
                v.Veterinario?.Nome);
        }).ToList();

        return Result<List<VacinaVermifugoDto>>.Ok(dtos);
    }
}

[RequerModulo(ModulosPet.VetAmbulatorio)]
public record RegistrarVacinaVermifugoCommand(
    int PetId,
    int? VeterinarioId,
    TipoPreventivoPet Tipo,
    string NomeProduto,
    string? Fabricante,
    string? NumeroLote,
    DateTime? ValidadeLote,
    string? Dose,
    DateTime? ProximaAplicacao,
    string? Observacoes) : IRequest<Result<int>>;

public class RegistrarVacinaVermifugoCommandHandler : IRequestHandler<RegistrarVacinaVermifugoCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public RegistrarVacinaVermifugoCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(RegistrarVacinaVermifugoCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NomeProduto))
            return Result<int>.Falha("Informe o nome da vacina/vermífugo.");

        var reg = new VacinacaoVermifugoPet
        {
            EmpresaId = _empresaService.EmpresaId ?? 0,
            PetId = request.PetId,
            VeterinarioId = request.VeterinarioId,
            Tipo = request.Tipo,
            NomeProduto = request.NomeProduto.Trim(),
            Fabricante = request.Fabricante,
            NumeroLote = request.NumeroLote,
            ValidadeLote = request.ValidadeLote?.ToUniversalTime(),
            Dose = request.Dose,
            DataAplicacao = DateTime.UtcNow,
            ProximaAplicacao = request.ProximaAplicacao?.ToUniversalTime(),
            Observacoes = request.Observacoes
        };

        _context.VacinacoesVermifugosPet.Add(reg);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(reg.Id);
    }
}
