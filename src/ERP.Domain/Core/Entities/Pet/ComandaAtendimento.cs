using ERP.Domain.Common;
using ERP.Domain.Core.Entities.Cadastros;
using ERP.Domain.Core.Entities.Pdv;
using ERP.Domain.Core.Enums;

namespace ERP.Domain.Core.Entities.Pet;

/// <summary>
/// Comanda Unificada de Atendimento Pet: agrega itens da Loja, Banho & Tosa e Hospital
/// para checkout único com Split Fiscal (NFC-e/NF-e vs NFS-e) e cobertura de Convênio Pet.
/// </summary>
public class ComandaAtendimento : BaseEmpresaEntity, IAggregateRoot
{
    public string Numero { get; set; } = string.Empty;

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public int? PetId { get; set; }
    public Pet? Pet { get; set; }

    public int? ConvenioPetId { get; set; }
    public ConvenioPet? ConvenioPet { get; set; }

    public StatusComanda Status { get; set; } = StatusComanda.Aberta;
    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
    public DateTime? DataFechamento { get; set; }

    public decimal ValorBruto { get; set; }
    public decimal ValorDesconto { get; set; }
    public decimal ValorCobertoConvenio { get; set; }
    public decimal ValorLiquido { get; set; }

    public int? VendaPdvId { get; set; }
    public VendaPdv? VendaPdv { get; set; }

    public string? Observacoes { get; set; }

    public ICollection<ComandaItem> Itens { get; set; } = new List<ComandaItem>();
}

public class ComandaItem : BaseEmpresaEntity
{
    public int ComandaAtendimentoId { get; set; }
    public ComandaAtendimento ComandaAtendimento { get; set; } = null!;

    public OrigemComandaItem Origem { get; set; } = OrigemComandaItem.Loja;
    public TipoComandaItem TipoItem { get; set; } = TipoComandaItem.Produto;

    public int? ProdutoId { get; set; }
    public Produto? Produto { get; set; }

    public int? ServicoId { get; set; }
    public Servico? Servico { get; set; }

    public string Descricao { get; set; } = string.Empty;
    public decimal Quantidade { get; set; } = 1;
    public decimal ValorUnitario { get; set; }
    public decimal ValorDesconto { get; set; }
    public decimal ValorCobertoConvenio { get; set; }
    public decimal ValorTotal { get; set; }

    public bool CobertoPorPacote { get; set; }
    public DocumentoFiscalDestino DocumentoFiscalDestino { get; set; } = DocumentoFiscalDestino.NFCe;
}
