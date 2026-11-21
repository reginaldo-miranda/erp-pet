namespace ERP.Domain.Core.Enums;

public enum TipoEventoWhatsApp
{
    BanhoPronto = 1,
    LembreteVacina = 2,
    ConfirmacaoAgenda = 3,
    BoletimInternacao = 4,
    TermoPendente = 5
}

public enum StatusNotificacaoWhatsApp
{
    Pendente = 1,
    Enviada = 2,
    Erro = 3
}
