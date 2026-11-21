namespace ERP.Domain.Core.Enums;

/// <summary>
/// Planos de licenciamento SaaS por porte da empresa.
/// </summary>
public enum PlanoLicenca
{
    Pequeno = 1,
    Medio = 2,
    Grande = 3,
    Custom = 4
}

public enum OrigemModuloEmpresa
{
    Plano = 1,
    AddOn = 2
}

public enum EspeciePet
{
    Cao = 1,
    Gato = 2,
    Ave = 3,
    Roedor = 4,
    Silvestre = 5,
    Outro = 6
}

public enum SexoPet
{
    Macho = 1,
    Femea = 2,
    Indefinido = 3
}

public enum PortePet
{
    P = 1,
    M = 2,
    G = 3,
    GG = 4
}

public enum TipoPelagemPet
{
    Curta = 1,
    Longa = 2,
    Dupla = 3,
    Lisa = 4,
    Encaracolada = 5,
    SemPelo = 6
}

public enum TemperamentoPet
{
    Docil = 1,
    Medroso = 2,
    Agitado = 3,
    Reativo = 4
}

public enum TipoVinculoTutor
{
    Principal = 1,
    Secundario = 2,
    AutorizadoRetirada = 3
}

public enum StatusComanda
{
    Aberta = 1,
    EmFechamento = 2,
    Faturada = 3,
    Cancelada = 4
}

public enum OrigemComandaItem
{
    Loja = 1,
    BanhoTosa = 2,
    HospitalAmbulatorio = 3,
    HospitalInternacao = 4,
    HospitalCirurgia = 5,
    Exame = 6
}

public enum TipoComandaItem
{
    Produto = 1,
    Servico = 2
}

public enum DocumentoFiscalDestino
{
    NFCe = 1,
    NFe = 2,
    NFSe = 3
}
