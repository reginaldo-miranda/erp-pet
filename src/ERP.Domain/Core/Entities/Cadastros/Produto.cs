using ERP.Domain.Common;

namespace ERP.Domain.Core.Entities.Cadastros;

/// <summary>
/// Cadastro de produtos. Suporta controle de lote, série e dados fiscais básicos.
/// </summary>
public class Produto : BaseEmpresaEntity, IAggregateRoot
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }

    // Classificação
    public int? CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public int? UnidadeMedidaId { get; set; }
    public UnidadeMedida? UnidadeMedida { get; set; }

    // Preços
    public decimal PrecoVenda { get; set; }
    public decimal PrecoCusto { get; set; }

    // Estoque
    public decimal EstoqueMinimo { get; set; }
    public bool ControlaLote { get; set; }
    public bool ControlaSerie { get; set; }

    // Dados fiscais
    public string? Ncm { get; set; }
    public string? Cest { get; set; }
    public string? Cfop { get; set; }
    public decimal? AliquotaIcms { get; set; }
    public decimal? AliquotaIpi { get; set; }
    public decimal? AliquotaPis { get; set; }
    public decimal? AliquotaCofins { get; set; }

    // Código de barras / referência
    public string? CodigoBarras { get; set; }
    public string? CodigoFornecedor { get; set; }
    public string? ImagemUrl { get; set; }

    // Fracionamento / Venda a Granel (Pet Shop)
    public bool PermiteFracionamento { get; set; }
    public int? ProdutoOrigemGranelId { get; set; }
    public Produto? ProdutoOrigemGranel { get; set; }
    public decimal? FatorConversaoGranel { get; set; } // Ex: 15 (1 saco = 15 kg)
    public int? DiasValidadeAposAbertura { get; set; }
}
