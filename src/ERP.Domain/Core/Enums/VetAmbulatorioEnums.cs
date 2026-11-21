namespace ERP.Domain.Core.Enums;

public enum StatusConsultaVet
{
    Agendada = 1,
    EmTriagem = 2,
    EmAtendimento = 3,
    Finalizada = 4,
    Cancelada = 5
}

public enum TipoConsultaVet
{
    ConsultaGeral = 1,
    Retorno = 2,
    Emergencia = 3,
    Especialidade = 4,
    Vacinacao = 5
}

public enum TipoReceitaVet
{
    Simples = 1,
    ControleEspecial = 2,
    UsoContinuo = 3
}

public enum TipoPreventivoPet
{
    Vacina = 1,
    Vermifugo = 2,
    AntipulgasCarrapaticida = 3
}
