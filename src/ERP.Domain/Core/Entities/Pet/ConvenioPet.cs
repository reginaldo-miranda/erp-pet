using ERP.Domain.Common;

namespace ERP.Domain.Core.Entities.Pet;

/// <summary>
/// Cadastro de Convênios / Planos de Saúde Pet aceitos pela empresa.
/// </summary>
public class ConvenioPet : BaseEmpresaEntity, IAggregateRoot
{
    public string Nome { get; set; } = string.Empty;
    public string? Cnpj { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public decimal PercentualCoberturaPadrao { get; set; }
    public string? Observacoes { get; set; }
}
