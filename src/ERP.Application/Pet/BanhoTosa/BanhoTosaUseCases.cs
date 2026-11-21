using ERP.Application.Common.Behaviors;
using ERP.Application.Common.Interfaces;
using ERP.Application.Common.Models;
using ERP.Application.Pet.Comandas;
using ERP.Domain.Core.Entities.Pet;
using ERP.Domain.Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Pet.BanhoTosa;

public record TabelaPrecoBanhoDto(
    int Id,
    string NomeServico,
    PortePet Porte,
    TipoPelagemPet TipoPelagem,
    decimal Valor,
    int DuracaoMinutos,
    decimal ComissaoBanhoPercentual,
    decimal ComissaoTosaPercentual);

public record RecursoBanhoDto(int Id, string Nome, TipoRecursoBanho Tipo);

public record PacoteBanhoDto(
    int Id,
    int ClienteId,
    string NomeTutor,
    int? PetId,
    string? NomePet,
    string NomePacote,
    int TotalSessoes,
    int SessoesUtilizadas,
    int SessoesDisponiveis,
    decimal ValorTotal,
    DateTime DataInicio,
    DateTime DataValidade);

public record AtendimentoBanhoDto(
    int Id,
    string Numero,
    int PetId,
    string NomePet,
    PortePet PortePet,
    TipoPelagemPet PelagemPet,
    int ClienteId,
    string NomeTutor,
    string? NomeRecurso,
    string? BanhistaNome,
    string? TosadorNome,
    DateTime DataAgendada,
    int DuracaoMinutos,
    StatusAtendimentoBanho Status,
    decimal ValorTotal,
    bool CobertoPorPacote,
    DateTime? CheckInEm,
    bool TemNos,
    bool TemPulgasCarrapatos,
    bool TemLesoesPele,
    bool TemOtite,
    string? Pertences,
    string? ObservacoesCheckIn,
    string? FotoEntradaUrl,
    string? FotoSaidaUrl);

public record ComissaoBanhoDto(
    int Id,
    string NumeroAtendimento,
    string NomePet,
    string ProfissionalNome,
    EtapaComissaoBanho Etapa,
    decimal BaseCalculo,
    decimal Percentual,
    decimal ValorComissao,
    bool Paga);

// ==================== TABELA DE PREÇOS ====================

[RequerModulo(ModulosPet.BanhoTosa)]
public record GetTabelasPrecoBanhoQuery : IRequest<Result<List<TabelaPrecoBanhoDto>>>;

public class GetTabelasPrecoBanhoQueryHandler : IRequestHandler<GetTabelasPrecoBanhoQuery, Result<List<TabelaPrecoBanhoDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetTabelasPrecoBanhoQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<TabelaPrecoBanhoDto>>> Handle(GetTabelasPrecoBanhoQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.TabelasPrecoBanho.AsNoTracking()
            .OrderBy(t => t.NomeServico).ThenBy(t => t.Porte).ThenBy(t => t.TipoPelagem)
            .Select(t => new TabelaPrecoBanhoDto(
                t.Id, t.NomeServico, t.Porte, t.TipoPelagem, t.Valor,
                t.DuracaoMinutos, t.ComissaoBanhoPercentual, t.ComissaoTosaPercentual))
            .ToListAsync(cancellationToken);
        return Result<List<TabelaPrecoBanhoDto>>.Ok(list);
    }
}

[RequerModulo(ModulosPet.BanhoTosa)]
public record CreateTabelaPrecoBanhoCommand(
    string NomeServico,
    PortePet Porte,
    TipoPelagemPet TipoPelagem,
    decimal Valor,
    int DuracaoMinutos,
    decimal ComissaoBanhoPercentual,
    decimal ComissaoTosaPercentual) : IRequest<Result<int>>;

public class CreateTabelaPrecoBanhoCommandHandler : IRequestHandler<CreateTabelaPrecoBanhoCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public CreateTabelaPrecoBanhoCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(CreateTabelaPrecoBanhoCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NomeServico))
            return Result<int>.Falha("Nome do serviço é obrigatório.");

        var item = new TabelaPrecoBanho
        {
            EmpresaId = _empresaService.EmpresaId ?? 0,
            NomeServico = request.NomeServico.Trim(),
            Porte = request.Porte,
            TipoPelagem = request.TipoPelagem,
            Valor = request.Valor,
            DuracaoMinutos = request.DuracaoMinutos > 0 ? request.DuracaoMinutos : 60,
            ComissaoBanhoPercentual = request.ComissaoBanhoPercentual,
            ComissaoTosaPercentual = request.ComissaoTosaPercentual
        };

        _context.TabelasPrecoBanho.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(item.Id);
    }
}

// ==================== RECURSOS / BOXES ====================

[RequerModulo(ModulosPet.BanhoTosa)]
public record GetRecursosBanhoQuery : IRequest<Result<List<RecursoBanhoDto>>>;

public class GetRecursosBanhoQueryHandler : IRequestHandler<GetRecursosBanhoQuery, Result<List<RecursoBanhoDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetRecursosBanhoQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<RecursoBanhoDto>>> Handle(GetRecursosBanhoQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.RecursosBanhoTosa.AsNoTracking()
            .OrderBy(r => r.Nome)
            .Select(r => new RecursoBanhoDto(r.Id, r.Nome, r.Tipo))
            .ToListAsync(cancellationToken);
        return Result<List<RecursoBanhoDto>>.Ok(list);
    }
}

[RequerModulo(ModulosPet.BanhoTosa)]
public record CreateRecursoBanhoCommand(string Nome, TipoRecursoBanho Tipo) : IRequest<Result<int>>;

public class CreateRecursoBanhoCommandHandler : IRequestHandler<CreateRecursoBanhoCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public CreateRecursoBanhoCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(CreateRecursoBanhoCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return Result<int>.Falha("Nome do recurso/box é obrigatório.");

        var rec = new RecursoBanhoTosa
        {
            EmpresaId = _empresaService.EmpresaId ?? 0,
            Nome = request.Nome.Trim(),
            Tipo = request.Tipo
        };
        _context.RecursosBanhoTosa.Add(rec);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(rec.Id);
    }
}

// ==================== PACOTES ====================

[RequerModulo(ModulosPet.BanhoTosa)]
public record GetPacotesBanhoQuery(int? ClienteId = null) : IRequest<Result<List<PacoteBanhoDto>>>;

public class GetPacotesBanhoQueryHandler : IRequestHandler<GetPacotesBanhoQuery, Result<List<PacoteBanhoDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetPacotesBanhoQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<PacoteBanhoDto>>> Handle(GetPacotesBanhoQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PacotesBanhoPet.AsNoTracking()
            .Include(p => p.Cliente)
            .Include(p => p.Pet)
            .AsQueryable();

        if (request.ClienteId.HasValue)
            query = query.Where(p => p.ClienteId == request.ClienteId.Value);

        var list = await query.OrderByDescending(p => p.DataInicio)
            .Select(p => new PacoteBanhoDto(
                p.Id, p.ClienteId, p.Cliente.Nome, p.PetId, p.Pet != null ? p.Pet.Nome : null,
                p.NomePacote, p.TotalSessoes, p.SessoesUtilizadas,
                Math.Max(0, p.TotalSessoes - p.SessoesUtilizadas),
                p.ValorTotal, p.DataInicio, p.DataValidade))
            .ToListAsync(cancellationToken);

        return Result<List<PacoteBanhoDto>>.Ok(list);
    }
}

[RequerModulo(ModulosPet.BanhoTosa)]
public record CreatePacoteBanhoCommand(
    int ClienteId,
    int? PetId,
    string NomePacote,
    int TotalSessoes,
    decimal ValorTotal,
    int DiasValidade,
    string? Observacoes) : IRequest<Result<int>>;

public class CreatePacoteBanhoCommandHandler : IRequestHandler<CreatePacoteBanhoCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public CreatePacoteBanhoCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(CreatePacoteBanhoCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NomePacote) || request.TotalSessoes <= 0)
            return Result<int>.Falha("Informe o nome do pacote e a quantidade de sessões.");

        var pacote = new PacoteBanhoPet
        {
            EmpresaId = _empresaService.EmpresaId ?? 0,
            ClienteId = request.ClienteId,
            PetId = request.PetId,
            NomePacote = request.NomePacote.Trim(),
            TotalSessoes = request.TotalSessoes,
            SessoesUtilizadas = 0,
            ValorTotal = request.ValorTotal,
            DataInicio = DateTime.UtcNow,
            DataValidade = DateTime.UtcNow.AddDays(request.DiasValidade > 0 ? request.DiasValidade : 30),
            Observacoes = request.Observacoes
        };

        _context.PacotesBanhoPet.Add(pacote);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(pacote.Id);
    }
}

// ==================== AGENDA, CHECK-IN & FILA EM TEMPO REAL ====================

[RequerModulo(ModulosPet.BanhoTosa)]
public record GetAtendimentosBanhoQuery(DateTime? Data = null) : IRequest<Result<List<AtendimentoBanhoDto>>>;

public class GetAtendimentosBanhoQueryHandler : IRequestHandler<GetAtendimentosBanhoQuery, Result<List<AtendimentoBanhoDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetAtendimentosBanhoQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<AtendimentoBanhoDto>>> Handle(GetAtendimentosBanhoQuery request, CancellationToken cancellationToken)
    {
        var query = _context.AtendimentosBanhoTosa.AsNoTracking()
            .Include(a => a.Pet)
            .Include(a => a.Cliente)
            .Include(a => a.RecursoBanhoTosa)
            .AsQueryable();

        if (request.Data.HasValue)
        {
            var ini = request.Data.Value.Date.ToUniversalTime();
            var fim = ini.AddDays(1);
            query = query.Where(a => a.DataAgendada >= ini && a.DataAgendada < fim);
        }

        var list = await query
            .OrderBy(a => a.DataAgendada)
            .Take(200)
            .Select(a => new AtendimentoBanhoDto(
                a.Id, a.Numero, a.PetId, a.Pet.Nome, a.Pet.Porte, a.Pet.TipoPelagem,
                a.ClienteId, a.Cliente.Nome,
                a.RecursoBanhoTosa != null ? a.RecursoBanhoTosa.Nome : null,
                a.BanhistaNome, a.TosadorNome, a.DataAgendada, a.DuracaoMinutos,
                a.Status, a.ValorTotal, a.CobertoPorPacote, a.CheckInEm,
                a.TemNos, a.TemPulgasCarrapatos, a.TemLesoesPele, a.TemOtite,
                a.Pertences, a.ObservacoesCheckIn, a.FotoEntradaUrl, a.FotoSaidaUrl))
            .ToListAsync(cancellationToken);

        return Result<List<AtendimentoBanhoDto>>.Ok(list);
    }
}

[RequerModulo(ModulosPet.BanhoTosa)]
public record AgendarBanhoCommand(
    int PetId,
    int ClienteId,
    int? TabelaPrecoBanhoId,
    int? PacoteBanhoPetId,
    int? RecursoBanhoTosaId,
    int? ComandaAtendimentoId,
    string? BanhistaNome,
    string? TosadorNome,
    DateTime DataAgendada,
    decimal? ValorServicoOverride = null,
    decimal ValorAdicionais = 0m) : IRequest<Result<int>>;

public class AgendarBanhoCommandHandler : IRequestHandler<AgendarBanhoCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;

    public AgendarBanhoCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(AgendarBanhoCommand request, CancellationToken cancellationToken)
    {
        var pet = await _context.Pets.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.PetId, cancellationToken);
        if (pet == null)
            return Result<int>.Falha("Pet não encontrado.");

        TabelaPrecoBanho? tabela = null;
        if (request.TabelaPrecoBanhoId.HasValue)
        {
            tabela = await _context.TabelasPrecoBanho.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == request.TabelaPrecoBanhoId.Value, cancellationToken);
        }
        else
        {
            tabela = await _context.TabelasPrecoBanho.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Porte == pet.Porte && t.TipoPelagem == pet.TipoPelagem, cancellationToken);
        }

        var dataUtc = request.DataAgendada.Kind == DateTimeKind.Utc
            ? request.DataAgendada
            : request.DataAgendada.ToUniversalTime();
        var duracao = tabela?.DuracaoMinutos ?? 60;

        if (request.RecursoBanhoTosaId.HasValue)
        {
            var fimNovo = dataUtc.AddMinutes(duracao);
            var conflito = await _context.AtendimentosBanhoTosa.AnyAsync(a =>
                a.RecursoBanhoTosaId == request.RecursoBanhoTosaId.Value &&
                a.Status != StatusAtendimentoBanho.Cancelado &&
                a.Status != StatusAtendimentoBanho.Entregue &&
                a.DataAgendada < fimNovo &&
                a.DataAgendada.AddMinutes(a.DuracaoMinutos) > dataUtc, cancellationToken);

            if (conflito)
                return Result<int>.Falha("Já existe um agendamento conflitante neste box/banheira para o horário informado.");
        }

        var cobertoPorPacote = false;
        if (request.PacoteBanhoPetId.HasValue)
        {
            var pacote = await _context.PacotesBanhoPet.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PacoteBanhoPetId.Value, cancellationToken);
            if (pacote == null || pacote.SessoesDisponiveis <= 0 || pacote.DataValidade < DateTime.UtcNow)
                return Result<int>.Falha("Pacote inválido, expirado ou sem saldo de sessões.");
            cobertoPorPacote = true;
        }

        var valorServico = cobertoPorPacote ? 0m : (request.ValorServicoOverride ?? tabela?.Valor ?? 0m);
        var valorTotal = valorServico + request.ValorAdicionais;

        var atendimento = new AtendimentoBanhoTosa
        {
            EmpresaId = _empresaService.EmpresaId ?? 0,
            Numero = $"BT-{DateTime.UtcNow:yyyyMMddHHmmss}",
            PetId = request.PetId,
            ClienteId = request.ClienteId,
            TabelaPrecoBanhoId = tabela?.Id,
            PacoteBanhoPetId = request.PacoteBanhoPetId,
            RecursoBanhoTosaId = request.RecursoBanhoTosaId,
            ComandaAtendimentoId = request.ComandaAtendimentoId,
            BanhistaNome = request.BanhistaNome,
            TosadorNome = request.TosadorNome,
            DataAgendada = dataUtc,
            DuracaoMinutos = duracao,
            Status = StatusAtendimentoBanho.Agendado,
            ValorServico = valorServico,
            ValorAdicionais = request.ValorAdicionais,
            ValorTotal = valorTotal,
            CobertoPorPacote = cobertoPorPacote
        };

        _context.AtendimentosBanhoTosa.Add(atendimento);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(atendimento.Id);
    }
}

[RequerModulo(ModulosPet.BanhoTosa)]
public record RealizarCheckInBanhoCommand(
    int AtendimentoId,
    bool TemNos,
    bool TemPulgasCarrapatos,
    bool TemLesoesPele,
    bool TemOtite,
    string? Pertences,
    string? ObservacoesCheckIn,
    string? FotoEntradaUrl) : IRequest<Result<bool>>;

public class RealizarCheckInBanhoCommandHandler : IRequestHandler<RealizarCheckInBanhoCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    public RealizarCheckInBanhoCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(RealizarCheckInBanhoCommand request, CancellationToken cancellationToken)
    {
        var at = await _context.AtendimentosBanhoTosa.FirstOrDefaultAsync(a => a.Id == request.AtendimentoId, cancellationToken);
        if (at == null)
            return Result<bool>.Falha("Atendimento de Banho & Tosa não encontrado.");

        at.CheckInEm = DateTime.UtcNow;
        at.TemNos = request.TemNos;
        at.TemPulgasCarrapatos = request.TemPulgasCarrapatos;
        at.TemLesoesPele = request.TemLesoesPele;
        at.TemOtite = request.TemOtite;
        at.Pertences = request.Pertences;
        at.ObservacoesCheckIn = request.ObservacoesCheckIn;
        at.FotoEntradaUrl = request.FotoEntradaUrl;
        if (at.Status == StatusAtendimentoBanho.Agendado)
            at.Status = StatusAtendimentoBanho.AguardandoCheckIn;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}

[RequerModulo(ModulosPet.BanhoTosa)]
public record AvancarStatusBanhoCommand(
    int AtendimentoId,
    StatusAtendimentoBanho NovoStatus,
    string? FotoSaidaUrl = null) : IRequest<Result<bool>>;

public class AvancarStatusBanhoCommandHandler : IRequestHandler<AvancarStatusBanhoCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    public AvancarStatusBanhoCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(AvancarStatusBanhoCommand request, CancellationToken cancellationToken)
    {
        var at = await _context.AtendimentosBanhoTosa
            .Include(a => a.TabelaPrecoBanho)
            .Include(a => a.PacoteBanhoPet)
            .Include(a => a.Comissoes)
            .FirstOrDefaultAsync(a => a.Id == request.AtendimentoId, cancellationToken);

        if (at == null)
            return Result<bool>.Falha("Atendimento não encontrado.");

        at.Status = request.NovoStatus;
        if (!string.IsNullOrWhiteSpace(request.FotoSaidaUrl))
            at.FotoSaidaUrl = request.FotoSaidaUrl;

        if (request.NovoStatus == StatusAtendimentoBanho.ProntoRetirada)
        {
            at.ProntoEm = DateTime.UtcNow;

            // Consumir sessão de pacote uma única vez
            if (at.CobertoPorPacote && at.PacoteBanhoPet != null && at.PacoteBanhoPet.SessoesUtilizadas < at.PacoteBanhoPet.TotalSessoes)
            {
                at.PacoteBanhoPet.SessoesUtilizadas += 1;
            }

            // Gerar comissões por etapa (Banhista / Tosador) se ainda não geradas
            if (at.Comissoes.Count == 0)
            {
                var baseCalc = at.CobertoPorPacote && at.PacoteBanhoPet != null
                    ? at.PacoteBanhoPet.ValorProporcionalSessao + at.ValorAdicionais
                    : at.ValorTotal;

                var pctBanho = at.TabelaPrecoBanho?.ComissaoBanhoPercentual ?? 20m;
                var pctTosa = at.TabelaPrecoBanho?.ComissaoTosaPercentual ?? 30m;

                if (!string.IsNullOrWhiteSpace(at.BanhistaNome))
                {
                    at.Comissoes.Add(new ComissaoBanhoTosa
                    {
                        EmpresaId = at.EmpresaId,
                        AtendimentoBanhoTosaId = at.Id,
                        ProfissionalNome = at.BanhistaNome,
                        Etapa = EtapaComissaoBanho.Banho,
                        BaseCalculo = baseCalc,
                        Percentual = pctBanho,
                        ValorComissao = Math.Round(baseCalc * (pctBanho / 100m), 2)
                    });
                }

                if (!string.IsNullOrWhiteSpace(at.TosadorNome))
                {
                    at.Comissoes.Add(new ComissaoBanhoTosa
                    {
                        EmpresaId = at.EmpresaId,
                        AtendimentoBanhoTosaId = at.Id,
                        ProfissionalNome = at.TosadorNome,
                        Etapa = EtapaComissaoBanho.Tosa,
                        BaseCalculo = baseCalc,
                        Percentual = pctTosa,
                        ValorComissao = Math.Round(baseCalc * (pctTosa / 100m), 2)
                    });
                }
            }

            // Lançar automaticamente na Comanda Unificada vinculada (se houver)
            if (at.ComandaAtendimentoId.HasValue)
            {
                var comanda = await _context.ComandasAtendimento
                    .Include(c => c.Itens)
                    .FirstOrDefaultAsync(c => c.Id == at.ComandaAtendimentoId.Value, cancellationToken);

                if (comanda != null && comanda.Status == StatusComanda.Aberta &&
                    !comanda.Itens.Any(i => i.Origem == OrigemComandaItem.BanhoTosa && i.Descricao.Contains(at.Numero)))
                {
                    comanda.Itens.Add(new ComandaItem
                    {
                        EmpresaId = comanda.EmpresaId,
                        ComandaAtendimentoId = comanda.Id,
                        Origem = OrigemComandaItem.BanhoTosa,
                        TipoItem = TipoComandaItem.Servico,
                        Descricao = $"Banho & Tosa {at.Numero}",
                        Quantidade = 1,
                        ValorUnitario = at.ValorTotal,
                        ValorTotal = at.ValorTotal,
                        CobertoPorPacote = at.CobertoPorPacote,
                        DocumentoFiscalDestino = DocumentoFiscalDestino.NFSe
                    });
                    AdicionarItemComandaCommandHandler.RecalcularTotais(comanda);
                }
            }
        }
        else if (request.NovoStatus == StatusAtendimentoBanho.Entregue)
        {
            at.EntregueEm = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}

[RequerModulo(ModulosPet.BanhoTosa)]
public record GetComissoesBanhoQuery : IRequest<Result<List<ComissaoBanhoDto>>>;

public class GetComissoesBanhoQueryHandler : IRequestHandler<GetComissoesBanhoQuery, Result<List<ComissaoBanhoDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetComissoesBanhoQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<ComissaoBanhoDto>>> Handle(GetComissoesBanhoQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.ComissoesBanhoTosa.AsNoTracking()
            .Include(c => c.AtendimentoBanhoTosa)
                .ThenInclude(a => a.Pet)
            .OrderByDescending(c => c.Id)
            .Take(200)
            .Select(c => new ComissaoBanhoDto(
                c.Id,
                c.AtendimentoBanhoTosa.Numero,
                c.AtendimentoBanhoTosa.Pet.Nome,
                c.ProfissionalNome,
                c.Etapa,
                c.BaseCalculo,
                c.Percentual,
                c.ValorComissao,
                c.Paga))
            .ToListAsync(cancellationToken);

        return Result<List<ComissaoBanhoDto>>.Ok(list);
    }
}
