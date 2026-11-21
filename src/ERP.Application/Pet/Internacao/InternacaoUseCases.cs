using ERP.Application.Common.Behaviors;
using ERP.Application.Common.Interfaces;
using ERP.Application.Common.Models;
using ERP.Application.Pet.Comandas;
using ERP.Domain.Core.Entities.Estoque;
using ERP.Domain.Core.Entities.Pet;
using ERP.Domain.Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Pet.Internacao;

public record BaiaInternacaoDto(int Id, string Codigo, string Nome, SetorInternacao Setor, StatusBaiaInternacao Status, decimal ValorDiaria);

public record ChecagemEnfermagemDto(
    int Id,
    string DescricaoMedicamento,
    string Dose,
    DateTime DataHoraPrevista,
    DateTime? DataHoraAplicada,
    StatusChecagemEnfermagem Status,
    string? ResponsavelEnfermagem);

public record EvolucaoInternacaoDto(
    int Id,
    DateTime DataHora,
    string Turno,
    string ProfissionalNome,
    decimal? TemperaturaC,
    int? FrequenciaCardiacaBpm,
    string? EstadoGeral,
    string DescricaoEvolucao,
    bool VisivelBoletimTutor);

public record InternacaoDto(
    int Id,
    string Numero,
    int PetId,
    string NomePet,
    string NomeTutor,
    string NomeVeterinario,
    int BaiaId,
    string CodigoBaia,
    SetorInternacao SetorBaia,
    DateTime DataEntrada,
    string MotivoInternacao,
    StatusInternacaoPet Status,
    decimal ValorDiaria,
    List<ChecagemEnfermagemDto> ChecagensPendentes,
    List<EvolucaoInternacaoDto> Evolucoes);

// ==================== BAIAS ====================

[RequerModulo(ModulosPet.VetInternacao)]
public record GetBaiasInternacaoQuery : IRequest<Result<List<BaiaInternacaoDto>>>;

public class GetBaiasInternacaoQueryHandler : IRequestHandler<GetBaiasInternacaoQuery, Result<List<BaiaInternacaoDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetBaiasInternacaoQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<BaiaInternacaoDto>>> Handle(GetBaiasInternacaoQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.BaiasInternacao.AsNoTracking()
            .OrderBy(b => b.Setor).ThenBy(b => b.Codigo)
            .Select(b => new BaiaInternacaoDto(b.Id, b.Codigo, b.Nome, b.Setor, b.Status, b.ValorDiaria))
            .ToListAsync(cancellationToken);
        return Result<List<BaiaInternacaoDto>>.Ok(list);
    }
}

[RequerModulo(ModulosPet.VetInternacao)]
public record CreateBaiaInternacaoCommand(string Codigo, string Nome, SetorInternacao Setor, decimal ValorDiaria) : IRequest<Result<int>>;

public class CreateBaiaInternacaoCommandHandler : IRequestHandler<CreateBaiaInternacaoCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public CreateBaiaInternacaoCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(CreateBaiaInternacaoCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Codigo))
            return Result<int>.Falha("Código da baia é obrigatório.");

        var baia = new BaiaInternacao
        {
            EmpresaId = _empresaService.EmpresaId ?? 0,
            Codigo = request.Codigo.Trim().ToUpperInvariant(),
            Nome = string.IsNullOrWhiteSpace(request.Nome) ? request.Codigo.Trim() : request.Nome.Trim(),
            Setor = request.Setor,
            Status = StatusBaiaInternacao.Livre,
            ValorDiaria = request.ValorDiaria
        };
        _context.BaiasInternacao.Add(baia);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(baia.Id);
    }
}

// ==================== INTERNAÇÃO, PRESCRIÇÃO & CHECAGEM ====================

[RequerModulo(ModulosPet.VetInternacao)]
public record GetInternacoesAtivasQuery : IRequest<Result<List<InternacaoDto>>>;

public class GetInternacoesAtivasQueryHandler : IRequestHandler<GetInternacoesAtivasQuery, Result<List<InternacaoDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetInternacoesAtivasQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<InternacaoDto>>> Handle(GetInternacoesAtivasQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.InternacoesPet.AsNoTracking()
            .Include(i => i.Pet)
            .Include(i => i.Cliente)
            .Include(i => i.Veterinario)
            .Include(i => i.BaiaInternacao)
            .Include(i => i.Prescricoes).ThenInclude(p => p.Checagens)
            .Include(i => i.Evolucoes)
            .OrderByDescending(i => i.DataEntrada)
            .Take(100)
            .ToListAsync(cancellationToken);

        var dtos = list.Select(i => new InternacaoDto(
            i.Id, i.Numero, i.PetId, i.Pet.Nome, i.Cliente.Nome, i.Veterinario.Nome,
            i.BaiaInternacaoId, i.BaiaInternacao.Codigo, i.BaiaInternacao.Setor,
            i.DataEntrada, i.MotivoInternacao, i.Status, i.ValorDiaria,
            i.Prescricoes.SelectMany(p => p.Checagens.Select(c => new ChecagemEnfermagemDto(
                c.Id, p.DescricaoMedicamento, p.Dose, c.DataHoraPrevista, c.DataHoraAplicada,
                c.Status, c.ResponsavelEnfermagem)))
                .OrderBy(c => c.DataHoraPrevista).ToList(),
            i.Evolucoes.OrderByDescending(e => e.DataHora).Select(e => new EvolucaoInternacaoDto(
                e.Id, e.DataHora, e.Turno, e.ProfissionalNome, e.TemperaturaC,
                e.FrequenciaCardiacaBpm, e.EstadoGeral, e.DescricaoEvolucao, e.VisivelBoletimTutor)).ToList()
        )).ToList();

        return Result<List<InternacaoDto>>.Ok(dtos);
    }
}

[RequerModulo(ModulosPet.VetInternacao)]
public record AdmitirInternacaoCommand(
    int PetId,
    int ClienteId,
    int VeterinarioId,
    int BaiaInternacaoId,
    int? ComandaAtendimentoId,
    string MotivoInternacao,
    decimal? ValorDiariaOverride = null) : IRequest<Result<int>>;

public class AdmitirInternacaoCommandHandler : IRequestHandler<AdmitirInternacaoCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public AdmitirInternacaoCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(AdmitirInternacaoCommand request, CancellationToken cancellationToken)
    {
        var baia = await _context.BaiasInternacao.FirstOrDefaultAsync(b => b.Id == request.BaiaInternacaoId, cancellationToken);
        if (baia == null) return Result<int>.Falha("Baia não encontrada.");
        if (baia.Status != StatusBaiaInternacao.Livre)
            return Result<int>.Falha($"Baia {baia.Codigo} não está livre ({baia.Status}).");

        baia.Status = StatusBaiaInternacao.Ocupada;

        var internacao = new InternacaoPet
        {
            EmpresaId = _empresaService.EmpresaId ?? 0,
            Numero = $"INT-{DateTime.UtcNow:yyyyMMddHHmmss}",
            PetId = request.PetId,
            ClienteId = request.ClienteId,
            VeterinarioId = request.VeterinarioId,
            BaiaInternacaoId = baia.Id,
            ComandaAtendimentoId = request.ComandaAtendimentoId,
            DataEntrada = DateTime.UtcNow,
            MotivoInternacao = request.MotivoInternacao.Trim(),
            Status = StatusInternacaoPet.Ativa,
            ValorDiaria = request.ValorDiariaOverride ?? baia.ValorDiaria
        };

        _context.InternacoesPet.Add(internacao);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(internacao.Id);
    }
}

[RequerModulo(ModulosPet.VetInternacao)]
public record PrescreverInternacaoCommand(
    int InternacaoPetId,
    int? ProdutoId,
    string DescricaoMedicamento,
    string Dose,
    string? Via,
    decimal QuantidadePorAplicacao,
    int IntervaloHoras,
    int NumeroDoses,
    decimal ValorUnitarioCobranca) : IRequest<Result<int>>;

public class PrescreverInternacaoCommandHandler : IRequestHandler<PrescreverInternacaoCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public PrescreverInternacaoCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(PrescreverInternacaoCommand request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaService.EmpresaId ?? 0;
        var intervalo = Math.Max(1, request.IntervaloHoras);
        var doses = Math.Clamp(request.NumeroDoses, 1, 24);
        var inicio = DateTime.UtcNow;

        var presc = new PrescricaoInternacao
        {
            EmpresaId = empresaId,
            InternacaoPetId = request.InternacaoPetId,
            ProdutoId = request.ProdutoId,
            DescricaoMedicamento = request.DescricaoMedicamento.Trim(),
            Dose = request.Dose.Trim(),
            Via = request.Via,
            QuantidadePorAplicacao = request.QuantidadePorAplicacao > 0 ? request.QuantidadePorAplicacao : 1,
            IntervaloHoras = intervalo,
            ValorUnitarioCobranca = request.ValorUnitarioCobranca,
            HorarioInicio = inicio
        };

        for (var i = 0; i < doses; i++)
        {
            presc.Checagens.Add(new ChecagemEnfermagem
            {
                EmpresaId = empresaId,
                DataHoraPrevista = inicio.AddHours(i * intervalo),
                Status = StatusChecagemEnfermagem.Pendente
            });
        }

        _context.PrescricoesInternacao.Add(presc);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(presc.Id);
    }
}

[RequerModulo(ModulosPet.VetInternacao)]
public record ChecarEnfermagemCommand(
    int ChecagemId,
    string ResponsavelEnfermagem,
    int? DepositoId,
    string? Observacoes) : IRequest<Result<bool>>;

public class ChecarEnfermagemCommandHandler : IRequestHandler<ChecarEnfermagemCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    public ChecarEnfermagemCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(ChecarEnfermagemCommand request, CancellationToken cancellationToken)
    {
        var checagem = await _context.ChecagensEnfermagem
            .Include(c => c.PrescricaoInternacao)
                .ThenInclude(p => p.InternacaoPet)
            .FirstOrDefaultAsync(c => c.Id == request.ChecagemId, cancellationToken);

        if (checagem == null) return Result<bool>.Falha("Checagem não encontrada.");
        if (checagem.Status == StatusChecagemEnfermagem.Aplicado)
            return Result<bool>.Falha("Esta dose já foi aplicada.");

        checagem.Status = StatusChecagemEnfermagem.Aplicado;
        checagem.DataHoraAplicada = DateTime.UtcNow;
        checagem.ResponsavelEnfermagem = request.ResponsavelEnfermagem.Trim();
        checagem.Observacoes = request.Observacoes;

        var presc = checagem.PrescricaoInternacao;
        var internacao = presc.InternacaoPet;

        // 1. Baixa automática do estoque de medicamento/insumo
        if (presc.ProdutoId.HasValue && request.DepositoId.HasValue)
        {
            var est = await _context.EstoqueProdutos.FirstOrDefaultAsync(
                e => e.ProdutoId == presc.ProdutoId.Value && e.DepositoId == request.DepositoId.Value,
                cancellationToken);
            if (est != null)
            {
                est.Quantidade -= presc.QuantidadePorAplicacao;
                _context.MovimentacoesEstoque.Add(new MovimentacaoEstoque
                {
                    EmpresaId = internacao.EmpresaId,
                    ProdutoId = presc.ProdutoId.Value,
                    DepositoOrigemId = request.DepositoId.Value,
                    Tipo = TipoMovimentacaoEstoque.Saida,
                    Origem = OrigemMovimentacaoEstoque.HospitalInternacao,
                    Quantidade = presc.QuantidadePorAplicacao,
                    CustoUnitario = est.CustoMedio,
                    CustoTotal = Math.Round(presc.QuantidadePorAplicacao * est.CustoMedio, 2),
                    DataMovimentacao = DateTime.UtcNow,
                    Observacao = $"Internação {internacao.Numero} - {presc.DescricaoMedicamento}"
                });
            }
        }

        // 2. Débito automático na Comanda Unificada da internação
        if (internacao.ComandaAtendimentoId.HasValue && presc.ValorUnitarioCobranca > 0)
        {
            var comanda = await _context.ComandasAtendimento
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.Id == internacao.ComandaAtendimentoId.Value, cancellationToken);

            if (comanda != null && comanda.Status == StatusComanda.Aberta)
            {
                var total = Math.Round(presc.QuantidadePorAplicacao * presc.ValorUnitarioCobranca, 2);
                comanda.Itens.Add(new ComandaItem
                {
                    EmpresaId = comanda.EmpresaId,
                    ComandaAtendimentoId = comanda.Id,
                    Origem = OrigemComandaItem.HospitalInternacao,
                    TipoItem = TipoComandaItem.Produto,
                    ProdutoId = presc.ProdutoId,
                    Descricao = $"{presc.DescricaoMedicamento} ({presc.Dose})",
                    Quantidade = presc.QuantidadePorAplicacao,
                    ValorUnitario = presc.ValorUnitarioCobranca,
                    ValorTotal = total,
                    DocumentoFiscalDestino = DocumentoFiscalDestino.NFCe
                });
                AdicionarItemComandaCommandHandler.RecalcularTotais(comanda);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}

[RequerModulo(ModulosPet.VetInternacao)]
public record RegistrarEvolucaoInternacaoCommand(
    int InternacaoPetId,
    string Turno,
    string ProfissionalNome,
    decimal? TemperaturaC,
    int? FrequenciaCardiacaBpm,
    string? EstadoGeral,
    string DescricaoEvolucao,
    bool VisivelBoletimTutor) : IRequest<Result<int>>;

public class RegistrarEvolucaoInternacaoCommandHandler : IRequestHandler<RegistrarEvolucaoInternacaoCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public RegistrarEvolucaoInternacaoCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(RegistrarEvolucaoInternacaoCommand request, CancellationToken cancellationToken)
    {
        var ev = new EvolucaoInternacao
        {
            EmpresaId = _empresaService.EmpresaId ?? 0,
            InternacaoPetId = request.InternacaoPetId,
            DataHora = DateTime.UtcNow,
            Turno = request.Turno,
            ProfissionalNome = request.ProfissionalNome.Trim(),
            TemperaturaC = request.TemperaturaC,
            FrequenciaCardiacaBpm = request.FrequenciaCardiacaBpm,
            EstadoGeral = request.EstadoGeral,
            DescricaoEvolucao = request.DescricaoEvolucao.Trim(),
            VisivelBoletimTutor = request.VisivelBoletimTutor
        };

        _context.EvolucoesInternacao.Add(ev);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(ev.Id);
    }
}

[RequerModulo(ModulosPet.VetInternacao)]
public record DarAltaInternacaoCommand(int InternacaoPetId, StatusInternacaoPet StatusFinal) : IRequest<Result<bool>>;

public class DarAltaInternacaoCommandHandler : IRequestHandler<DarAltaInternacaoCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    public DarAltaInternacaoCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(DarAltaInternacaoCommand request, CancellationToken cancellationToken)
    {
        var internacao = await _context.InternacoesPet
            .Include(i => i.BaiaInternacao)
            .FirstOrDefaultAsync(i => i.Id == request.InternacaoPetId, cancellationToken);

        if (internacao == null) return Result<bool>.Falha("Internação não encontrada.");

        internacao.Status = request.StatusFinal;
        internacao.DataAltaReal = DateTime.UtcNow;
        internacao.BaiaInternacao.Status = StatusBaiaInternacao.EmHigienizacao;

        if (internacao.ComandaAtendimentoId.HasValue && internacao.ValorDiaria > 0)
        {
            var dias = Math.Max(1, (int)Math.Ceiling((DateTime.UtcNow - internacao.DataEntrada).TotalDays));
            var comanda = await _context.ComandasAtendimento
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.Id == internacao.ComandaAtendimentoId.Value, cancellationToken);

            if (comanda != null && comanda.Status == StatusComanda.Aberta)
            {
                comanda.Itens.Add(new ComandaItem
                {
                    EmpresaId = comanda.EmpresaId,
                    ComandaAtendimentoId = comanda.Id,
                    Origem = OrigemComandaItem.HospitalInternacao,
                    TipoItem = TipoComandaItem.Servico,
                    Descricao = $"Diárias de Internação {internacao.Numero} ({dias}d)",
                    Quantidade = dias,
                    ValorUnitario = internacao.ValorDiaria,
                    ValorTotal = dias * internacao.ValorDiaria,
                    DocumentoFiscalDestino = DocumentoFiscalDestino.NFSe
                });
                AdicionarItemComandaCommandHandler.RecalcularTotais(comanda);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}
