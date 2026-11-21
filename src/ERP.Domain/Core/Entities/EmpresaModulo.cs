using ERP.Domain.Common;
using ERP.Domain.Core.Enums;

namespace ERP.Domain.Core.Entities;

/// <summary>
/// Controle de módulos ativos por Empresa (Feature Flags SaaS / Add-ons).
/// </summary>
public class EmpresaModulo : BaseEmpresaEntity, IAggregateRoot
{
    public string ModuloCodigo { get; set; } = string.Empty;
    public OrigemModuloEmpresa Origem { get; set; } = OrigemModuloEmpresa.Plano;
    public DateTime DataAtivacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataExpiracao { get; set; }
}
