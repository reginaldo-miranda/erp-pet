namespace ERP.Domain.Core.Enums;

public enum StatusAtendimentoBanho
{
    Agendado = 1,
    AguardandoCheckIn = 2,
    EmBanho = 3,
    SecagemTosa = 4,
    ProntoRetirada = 5,
    Entregue = 6,
    Cancelado = 7
}

public enum EtapaComissaoBanho
{
    Banho = 1,
    Tosa = 2,
    Completo = 3
}

public enum TipoRecursoBanho
{
    Banheira = 1,
    MesaTosa = 2,
    Secador = 3,
    Box = 4
}
