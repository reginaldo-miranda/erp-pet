using ERP.Application.Common.Interfaces;
using ERP.Application.Common.Models;
using ERP.Domain.Core.Entities.Pet;
using ERP.Domain.Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Pet.Comandas;

public record ComandaItemDto(
    int Id,
    OrigemComandaItem Origem,
    TipoComandaItem TipoItem,
    int? ProdutoId,
    int? ServicoId,
    string Descricao,
    decimal Quantidade,
    decimal ValorUnitario,
    decimal ValorDesconto,
    decimal ValorCobertoConvenio,
    decimal ValorTotal,
    bool CobertoPorPacote,
    DocumentoFiscalDestino DocumentoFiscalDestino);

public record ResumoSplitFiscalDto(
    decimal TotalProdutosNfceNfe,
    decimal TotalServicosNfse,
    decimal TotalCobertoConvenio,
    decimal TotalLiquidoTutor);

public record ComandaAtendimentoDto(
    int Id,
    string Numero,
    int ClienteId,
    string NomeTutor,
    int? PetId,
    string? NomePet,
    int? ConvenioPetId,
    string? NomeConvenio,
    StatusComanda Status,
    DateTime DataAbertura,
    DateTime? DataFechamento,
    decimal ValorBruto,
    decimal ValorDesconto,
    decimal ValorCobertoConvenio,
    decimal ValorLiquido,
    string? Observacoes,
    List<ComandaItemDto> Itens,
    ResumoSplitFiscalDto SplitFiscal);

public record GetComandasQuery(StatusComanda? Status = null) : IRequest<Result<List<ComandaAtendimentoDto>>>;

public class GetComandasQueryHandler : IRequestHandler<GetComandasQuery, Result<List<ComandaAtendimentoDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetComandasQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ComandaAtendimentoDto>>> Handle(GetComandasQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ComandasAtendimento
            .AsNoTracking()
            .Include(c => c.Cliente)
            .Include(c => c.Pet)
            .Include(c => c.ConvenioPet)
            .Include(c => c.Itens)
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(c => c.Status == request.Status.Value);

        var comandas = await query
            .OrderByDescending(c => c.DataAbertura)
            .Take(100)
            .ToListAsync(cancellationToken);

        var dtos = comandas.Select(MapDto).ToList();
        return Result<List<ComandaAtendimentoDto>>.Ok(dtos);
    }

    internal static ComandaAtendimentoDto MapDto(ComandaAtendimento c)
    {
        var itens = c.Itens.Select(i => new ComandaItemDto(
            i.Id,
            i.Origem,
            i.TipoItem,
            i.ProdutoId,
            i.ServicoId,
            i.Descricao,
            i.Quantidade,
            i.ValorUnitario,
            i.ValorDesconto,
            i.ValorCobertoConvenio,
            i.ValorTotal,
            i.CobertoPorPacote,
            i.DocumentoFiscalDestino)).ToList();

        var totalProdutos = itens
            .Where(i => i.DocumentoFiscalDestino is DocumentoFiscalDestino.NFCe or DocumentoFiscalDestino.NFe)
            .Sum(i => i.ValorTotal);
        var totalServicos = itens
            .Where(i => i.DocumentoFiscalDestino == DocumentoFiscalDestino.NFSe)
            .Sum(i => i.ValorTotal);

        return new ComandaAtendimentoDto(
            c.Id,
            c.Numero,
            c.ClienteId,
            c.Cliente.Nome,
            c.PetId,
            c.Pet?.Nome,
            c.ConvenioPetId,
            c.ConvenioPet?.Nome,
            c.Status,
            c.DataAbertura,
            c.DataFechamento,
            c.ValorBruto,
            c.ValorDesconto,
            c.ValorCobertoConvenio,
            c.ValorLiquido,
            c.Observacoes,
            itens,
            new ResumoSplitFiscalDto(totalProdutos, totalServicos, c.ValorCobertoConvenio, c.ValorLiquido));
    }
}

public record AbrirComandaCommand(
    int ClienteId,
    int? PetId,
    int? ConvenioPetId,
    string? Observacoes) : IRequest<Result<int>>;

public class AbrirComandaCommandHandler : IRequestHandler<AbrirComandaCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;

    public AbrirComandaCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(AbrirComandaCommand request, CancellationToken cancellationToken)
    {
        var numero = $"CMD-{DateTime.UtcNow:yyyyMMddHHmmss}";
        var comanda = new ComandaAtendimento
        {
            EmpresaId = _empresaService.EmpresaId ?? 1,
            Numero = numero,
            ClienteId = request.ClienteId,
            PetId = request.PetId,
            ConvenioPetId = request.ConvenioPetId,
            Status = StatusComanda.Aberta,
            DataAbertura = DateTime.UtcNow,
            Observacoes = request.Observacoes
        };

        _context.ComandasAtendimento.Add(comanda);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(comanda.Id);
    }
}

public record AdicionarItemComandaCommand(
    int ComandaId,
    OrigemComandaItem Origem,
    TipoComandaItem TipoItem,
    int? ProdutoId,
    int? ServicoId,
    string Descricao,
    decimal Quantidade,
    decimal ValorUnitario,
    decimal ValorDesconto,
    bool CobertoPorPacote,
    bool AplicarCoberturaConvenio) : IRequest<Result<bool>>;

public class AdicionarItemComandaCommandHandler : IRequestHandler<AdicionarItemComandaCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;

    public AdicionarItemComandaCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<bool>> Handle(AdicionarItemComandaCommand request, CancellationToken cancellationToken)
    {
        var comanda = await _context.ComandasAtendimento
            .Include(c => c.ConvenioPet)
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.Id == request.ComandaId, cancellationToken);

        if (comanda == null)
            return Result<bool>.Falha("Comanda não encontrada.");

        if (comanda.Status != StatusComanda.Aberta)
            return Result<bool>.Falha("Apenas comandas abertas podem receber itens.");

        var brutoItem = request.CobertoPorPacote ? 0m : Math.Round(request.Quantidade * request.ValorUnitario, 2);
        var liquidoAntesConvenio = Math.Max(0m, brutoItem - request.ValorDesconto);

        var coberturaConvenio = 0m;
        if (!request.CobertoPorPacote && request.AplicarCoberturaConvenio && comanda.ConvenioPet != null)
        {
            coberturaConvenio = Math.Round(liquidoAntesConvenio * (comanda.ConvenioPet.PercentualCoberturaPadrao / 100m), 2);
        }

        var totalItem = Math.Max(0m, liquidoAntesConvenio - coberturaConvenio);

        var destinoFiscal = request.TipoItem == TipoComandaItem.Servico
            ? DocumentoFiscalDestino.NFSe
            : DocumentoFiscalDestino.NFCe;

        comanda.Itens.Add(new ComandaItem
        {
            EmpresaId = _empresaService.EmpresaId ?? comanda.EmpresaId,
            ComandaAtendimentoId = comanda.Id,
            Origem = request.Origem,
            TipoItem = request.TipoItem,
            ProdutoId = request.ProdutoId,
            ServicoId = request.ServicoId,
            Descricao = request.Descricao.Trim(),
            Quantidade = request.Quantidade,
            ValorUnitario = request.ValorUnitario,
            ValorDesconto = request.ValorDesconto,
            ValorCobertoConvenio = coberturaConvenio,
            ValorTotal = totalItem,
            CobertoPorPacote = request.CobertoPorPacote,
            DocumentoFiscalDestino = destinoFiscal
        });

        RecalcularTotais(comanda);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }

    internal static void RecalcularTotais(ComandaAtendimento comanda)
    {
        comanda.ValorBruto = comanda.Itens.Sum(i => i.CobertoPorPacote ? 0m : Math.Round(i.Quantidade * i.ValorUnitario, 2));
        comanda.ValorDesconto = comanda.Itens.Sum(i => i.ValorDesconto);
        comanda.ValorCobertoConvenio = comanda.Itens.Sum(i => i.ValorCobertoConvenio);
        comanda.ValorLiquido = comanda.Itens.Sum(i => i.ValorTotal);
    }
}

public record FecharComandaCommand(int ComandaId, int? VendaPdvId = null) : IRequest<Result<ResumoSplitFiscalDto>>;

public class FecharComandaCommandHandler : IRequestHandler<FecharComandaCommand, Result<ResumoSplitFiscalDto>>
{
    private readonly IApplicationDbContext _context;

    public FecharComandaCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ResumoSplitFiscalDto>> Handle(FecharComandaCommand request, CancellationToken cancellationToken)
    {
        var comanda = await _context.ComandasAtendimento
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.Id == request.ComandaId, cancellationToken);

        if (comanda == null)
            return Result<ResumoSplitFiscalDto>.Falha("Comanda não encontrada.");

        if (comanda.Status != StatusComanda.Aberta)
            return Result<ResumoSplitFiscalDto>.Falha("Comanda já foi encerrada.");

        AdicionarItemComandaCommandHandler.RecalcularTotais(comanda);
        comanda.Status = StatusComanda.Faturada;
        comanda.DataFechamento = DateTime.UtcNow;
        if (request.VendaPdvId.HasValue)
            comanda.VendaPdvId = request.VendaPdvId.Value;

        await _context.SaveChangesAsync(cancellationToken);

        var totalProdutos = comanda.Itens
            .Where(i => i.DocumentoFiscalDestino is DocumentoFiscalDestino.NFCe or DocumentoFiscalDestino.NFe)
            .Sum(i => i.ValorTotal);
        var totalServicos = comanda.Itens
            .Where(i => i.DocumentoFiscalDestino == DocumentoFiscalDestino.NFSe)
            .Sum(i => i.ValorTotal);

        return Result<ResumoSplitFiscalDto>.Ok(new ResumoSplitFiscalDto(
            totalProdutos,
            totalServicos,
            comanda.ValorCobertoConvenio,
            comanda.ValorLiquido));
    }
}
