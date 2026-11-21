using ERP.Domain.Common;

namespace ERP.Domain.Core.Entities;

/// <summary>
/// Catálogo global de módulos disponíveis no ERP Pet (habilitados por empresa).
/// </summary>
public class ModuloSistema : BaseEntity, IAggregateRoot
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty; // Core, Loja, BanhoTosa, Hospital, Tutor
    public int Ordem { get; set; }
    public bool Ativo { get; set; } = true;
}
