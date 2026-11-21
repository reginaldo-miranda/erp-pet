namespace ERP.Domain.Core.Enums;

public enum SetorInternacao
{
    Canil = 1,
    Gatil = 2,
    Isolamento = 3,
    UTI = 4
}

public enum StatusBaiaInternacao
{
    Livre = 1,
    Ocupada = 2,
    EmHigienizacao = 3,
    Manutencao = 4
}

public enum StatusInternacaoPet
{
    Ativa = 1,
    AltaMedica = 2,
    Obito = 3,
    Transferida = 4
}

public enum StatusChecagemEnfermagem
{
    Pendente = 1,
    Aplicado = 2,
    NaoRealizado = 3
}

public enum StatusCirurgiaVet
{
    Agendada = 1,
    EmPreparo = 2,
    EmAndamento = 3,
    Recuperacao = 4,
    Finalizada = 5,
    Cancelada = 6
}

public enum TipoTermoConsentimento
{
    Internacao = 1,
    Anestesia = 2,
    Cirurgia = 3,
    Transfusao = 4,
    Eutanasia = 5
}

public enum TipoExameVet
{
    Hematologia = 1,
    Bioquimica = 2,
    Urinalise = 3,
    RaioX = 4,
    Ultrassom = 5,
    Outro = 6
}

public enum StatusExameVet
{
    Solicitado = 1,
    AmostraColetada = 2,
    ResultadoRecebido = 3
}
