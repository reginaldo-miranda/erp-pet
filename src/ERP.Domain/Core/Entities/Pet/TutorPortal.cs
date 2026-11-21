using ERP.Domain.Common;
using ERP.Domain.Core.Entities.Cadastros;
using ERP.Domain.Core.Enums;

namespace ERP.Domain.Core.Entities.Pet;

public class ConfiguracaoWhatsAppEmpresa : BaseEmpresaEntity, IAggregateRoot
{
    public string Provedor { get; set; } = "EvolutionApi"; // EvolutionApi, MetaCloudApi, Webhook
    public string? BaseUrl { get; set; }
    public string? InstanceName { get; set; }
    public string? ApiKey { get; set; }
    public bool EnviarBanhoPronto { get; set; } = true;
    public bool EnviarLembreteVacina { get; set; } = true;
    public bool EnviarBoletimInternacao { get; set; } = true;
}

public class NotificacaoWhatsAppOutbox : BaseEmpresaEntity, IAggregateRoot
{
    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public int? PetId { get; set; }
    public Pet? Pet { get; set; }

    public string TelefoneDestino { get; set; } = string.Empty;
    public TipoEventoWhatsApp TipoEvento { get; set; } = TipoEventoWhatsApp.BanhoPronto;
    public string Mensagem { get; set; } = string.Empty;
    public StatusNotificacaoWhatsApp Status { get; set; } = StatusNotificacaoWhatsApp.Pendente;

    public DateTime DataAgendada { get; set; } = DateTime.UtcNow;
    public DateTime? DataEnvio { get; set; }
    public int Tentativas { get; set; }
    public string? ErroDetalhe { get; set; }
}

public class AcessoPortalTutor : BaseEmpresaEntity, IAggregateRoot
{
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public string TokenAcesso { get; set; } = string.Empty;
    public DateTime ValidoAte { get; set; } = DateTime.UtcNow.AddDays(30);
    public DateTime? UltimoAcessoEm { get; set; }
}
