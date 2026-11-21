using ERP.Domain.Common;
using ERP.Domain.Core.Entities.Cadastros;
using ERP.Domain.Core.Entities.Estoque;

namespace ERP.Domain.Core.Entities.Pet;

/// <summary>
/// Registro de abertura/fracionamento de saco fechado (ex: 15kg) para venda a granel (kg).
/// </summary>
public class AberturaSacoGranel : BaseEmpresaEntity, IAggregateRoot
{
    public int ProdutoSacoId { get; set; }
    public Produto ProdutoSaco { get; set; } = null!;

    public int ProdutoGranelId { get; set; }
    public Produto ProdutoGranel { get; set; } = null!;

    public int DepositoId { get; set; }
    public Deposito Deposito { get; set; } = null!;

    public int? LoteId { get; set; }
    public Lote? Lote { get; set; }

    public decimal QuantidadeSacos { get; set; } = 1;
    public decimal PesoTotalGeradoKg { get; set; }
    public decimal CustoUnitarioGranel { get; set; }

    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
    public DateTime? DataValidadeAposAbertura { get; set; }
    public string? Observacoes { get; set; }
}
