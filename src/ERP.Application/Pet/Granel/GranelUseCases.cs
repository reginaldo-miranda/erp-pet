using ERP.Application.Common.Behaviors;
using ERP.Application.Common.Interfaces;
using ERP.Application.Common.Models;
using ERP.Domain.Core.Entities.Estoque;
using ERP.Domain.Core.Entities.Pet;
using ERP.Domain.Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Pet.Granel;

public record AberturaSacoGranelDto(
    int Id,
    int ProdutoSacoId,
    string NomeProdutoSaco,
    int ProdutoGranelId,
    string NomeProdutoGranel,
    string NomeDeposito,
    decimal QuantidadeSacos,
    decimal PesoTotalGeradoKg,
    decimal CustoUnitarioGranel,
    DateTime DataAbertura,
    DateTime? DataValidadeAposAbertura,
    string? Observacoes);

[RequerModulo(ModulosPet.LojaGranel)]
public record GetAberturasGranelQuery : IRequest<Result<List<AberturaSacoGranelDto>>>;

public class GetAberturasGranelQueryHandler : IRequestHandler<GetAberturasGranelQuery, Result<List<AberturaSacoGranelDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAberturasGranelQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<AberturaSacoGranelDto>>> Handle(GetAberturasGranelQuery request, CancellationToken cancellationToken)
    {
        var lista = await _context.AberturasSacoGranel
            .AsNoTracking()
            .Include(a => a.ProdutoSaco)
            .Include(a => a.ProdutoGranel)
            .Include(a => a.Deposito)
            .OrderByDescending(a => a.DataAbertura)
            .Take(100)
            .Select(a => new AberturaSacoGranelDto(
                a.Id,
                a.ProdutoSacoId,
                a.ProdutoSaco.Nome,
                a.ProdutoGranelId,
                a.ProdutoGranel.Nome,
                a.Deposito.Nome,
                a.QuantidadeSacos,
                a.PesoTotalGeradoKg,
                a.CustoUnitarioGranel,
                a.DataAbertura,
                a.DataValidadeAposAbertura,
                a.Observacoes))
            .ToListAsync(cancellationToken);

        return Result<List<AberturaSacoGranelDto>>.Ok(lista);
    }
}

[RequerModulo(ModulosPet.LojaGranel)]
public record AbrirSacoGranelCommand(
    int ProdutoSacoId,
    int ProdutoGranelId,
    int DepositoId,
    decimal QuantidadeSacos,
    decimal? PesoPorSacoKgOverride = null,
    int? DiasValidadeAposAberturaOverride = null,
    string? Observacoes = null) : IRequest<Result<int>>;

public class AbrirSacoGranelCommandHandler : IRequestHandler<AbrirSacoGranelCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;

    public AbrirSacoGranelCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(AbrirSacoGranelCommand request, CancellationToken cancellationToken)
    {
        if (request.QuantidadeSacos <= 0)
            return Result<int>.Falha("A quantidade de sacos deve ser maior que zero.");

        var produtoSaco = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == request.ProdutoSacoId, cancellationToken);
        if (produtoSaco == null)
            return Result<int>.Falha("Produto saco fechado não encontrado.");

        var produtoGranel = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == request.ProdutoGranelId, cancellationToken);
        if (produtoGranel == null)
            return Result<int>.Falha("Produto a granel destino não encontrado.");

        var pesoPorSaco = request.PesoPorSacoKgOverride ?? produtoSaco.FatorConversaoGranel ?? produtoGranel.FatorConversaoGranel ?? 0m;
        if (pesoPorSaco <= 0)
            return Result<int>.Falha("Informe o fator de conversão em kg por saco (ex: 15 kg).");

        var empresaId = _empresaService.EmpresaId ?? produtoSaco.EmpresaId;

        var estoqueSaco = await _context.EstoqueProdutos
            .FirstOrDefaultAsync(e => e.ProdutoId == request.ProdutoSacoId && e.DepositoId == request.DepositoId, cancellationToken);

        if (estoqueSaco == null || estoqueSaco.Quantidade < request.QuantidadeSacos)
            return Result<int>.Falha("Saldo insuficiente do saco fechado no depósito selecionado.");

        var custoSaco = estoqueSaco.CustoMedio > 0 ? estoqueSaco.CustoMedio : produtoSaco.PrecoCusto;
        var pesoTotalKg = request.QuantidadeSacos * pesoPorSaco;
        var custoTotal = request.QuantidadeSacos * custoSaco;
        var custoUnitarioGranel = pesoTotalKg > 0 ? Math.Round(custoTotal / pesoTotalKg, 4) : 0m;

        // 1. Baixa no estoque do saco fechado
        estoqueSaco.Quantidade -= request.QuantidadeSacos;

        _context.MovimentacoesEstoque.Add(new MovimentacaoEstoque
        {
            EmpresaId = empresaId,
            ProdutoId = request.ProdutoSacoId,
            DepositoOrigemId = request.DepositoId,
            Tipo = TipoMovimentacaoEstoque.Saida,
            Origem = OrigemMovimentacaoEstoque.FracionamentoGranel,
            Quantidade = request.QuantidadeSacos,
            CustoUnitario = custoSaco,
            CustoTotal = custoTotal,
            DataMovimentacao = DateTime.UtcNow,
            Observacao = $"Abertura para granel -> {produtoGranel.Nome} ({pesoTotalKg:N3} kg)"
        });

        // 2. Entrada no estoque do produto a granel
        var estoqueGranel = await _context.EstoqueProdutos
            .FirstOrDefaultAsync(e => e.ProdutoId == request.ProdutoGranelId && e.DepositoId == request.DepositoId, cancellationToken);

        if (estoqueGranel == null)
        {
            estoqueGranel = new EstoqueProduto
            {
                EmpresaId = empresaId,
                ProdutoId = request.ProdutoGranelId,
                DepositoId = request.DepositoId,
                Quantidade = pesoTotalKg,
                CustoMedio = custoUnitarioGranel,
                CustoUltimaCompra = custoUnitarioGranel
            };
            _context.EstoqueProdutos.Add(estoqueGranel);
        }
        else
        {
            var valorAnterior = estoqueGranel.Quantidade * estoqueGranel.CustoMedio;
            var novaQtd = estoqueGranel.Quantidade + pesoTotalKg;
            estoqueGranel.CustoMedio = novaQtd > 0 ? Math.Round((valorAnterior + custoTotal) / novaQtd, 4) : custoUnitarioGranel;
            estoqueGranel.Quantidade = novaQtd;
        }

        _context.MovimentacoesEstoque.Add(new MovimentacaoEstoque
        {
            EmpresaId = empresaId,
            ProdutoId = request.ProdutoGranelId,
            DepositoOrigemId = request.DepositoId,
            Tipo = TipoMovimentacaoEstoque.Entrada,
            Origem = OrigemMovimentacaoEstoque.FracionamentoGranel,
            Quantidade = pesoTotalKg,
            CustoUnitario = custoUnitarioGranel,
            CustoTotal = custoTotal,
            DataMovimentacao = DateTime.UtcNow,
            Observacao = $"Fracionamento de {request.QuantidadeSacos}x {produtoSaco.Nome}"
        });

        var diasValidade = request.DiasValidadeAposAberturaOverride ?? produtoGranel.DiasValidadeAposAbertura ?? produtoSaco.DiasValidadeAposAbertura;
        DateTime? dataValidade = diasValidade.HasValue ? DateTime.UtcNow.AddDays(diasValidade.Value) : null;

        var abertura = new AberturaSacoGranel
        {
            EmpresaId = empresaId,
            ProdutoSacoId = request.ProdutoSacoId,
            ProdutoGranelId = request.ProdutoGranelId,
            DepositoId = request.DepositoId,
            QuantidadeSacos = request.QuantidadeSacos,
            PesoTotalGeradoKg = pesoTotalKg,
            CustoUnitarioGranel = custoUnitarioGranel,
            DataAbertura = DateTime.UtcNow,
            DataValidadeAposAbertura = dataValidade,
            Observacoes = request.Observacoes
        };

        _context.AberturasSacoGranel.Add(abertura);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Ok(abertura.Id);
    }
}
