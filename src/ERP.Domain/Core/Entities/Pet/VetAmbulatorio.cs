using ERP.Domain.Common;
using ERP.Domain.Core.Entities.Cadastros;
using ERP.Domain.Core.Enums;

namespace ERP.Domain.Core.Entities.Pet;

/// <summary>
/// Cadastro de Médicos Veterinários (CRMV, especialidade e valor padrão de consulta).
/// </summary>
public class Veterinario : BaseEmpresaEntity, IAggregateRoot
{
    public string Nome { get; set; } = string.Empty;
    public string Crmv { get; set; } = string.Empty;
    public string UfCrmv { get; set; } = string.Empty;
    public string? Especialidade { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public decimal ValorConsultaPadrao { get; set; }
}

/// <summary>
/// Atendimento Ambulatorial Veterinário: Agenda + Triagem + Prontuário SOAP.
/// </summary>
public class ConsultaVeterinaria : BaseEmpresaEntity, IAggregateRoot
{
    public string Numero { get; set; } = string.Empty;

    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public int VeterinarioId { get; set; }
    public Veterinario Veterinario { get; set; } = null!;

    public int? ComandaAtendimentoId { get; set; }
    public ComandaAtendimento? ComandaAtendimento { get; set; }

    public DateTime DataHora { get; set; } = DateTime.UtcNow;
    public TipoConsultaVet Tipo { get; set; } = TipoConsultaVet.ConsultaGeral;
    public StatusConsultaVet Status { get; set; } = StatusConsultaVet.Agendada;

    // Triagem
    public string? QueixaPrincipal { get; set; }
    public decimal? PesoKg { get; set; }
    public decimal? TemperaturaC { get; set; }
    public int? FrequenciaCardiacaBpm { get; set; }
    public int? FrequenciaRespiratoriaMpm { get; set; }
    public string? Mucosas { get; set; }
    public int? TpcSegundos { get; set; }

    // Prontuário SOAP
    public string? SoapSubjetivo { get; set; }
    public string? SoapObjetivo { get; set; }
    public string? SoapAvaliacao { get; set; }
    public string? SoapPlano { get; set; }

    public decimal ValorConsulta { get; set; }
    public DateTime? FinalizadaEm { get; set; }
}

/// <summary>
/// Receituário Veterinário (Simples, Controle Especial ou Uso Contínuo).
/// </summary>
public class ReceitaVeterinaria : BaseEmpresaEntity, IAggregateRoot
{
    public int? ConsultaVeterinariaId { get; set; }
    public ConsultaVeterinaria? ConsultaVeterinaria { get; set; }

    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;

    public int VeterinarioId { get; set; }
    public Veterinario Veterinario { get; set; } = null!;

    public TipoReceitaVet Tipo { get; set; } = TipoReceitaVet.Simples;
    public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
    public string? Observacoes { get; set; }

    public ICollection<ReceitaVeterinariaItem> Itens { get; set; } = new List<ReceitaVeterinariaItem>();
}

public class ReceitaVeterinariaItem : BaseEmpresaEntity
{
    public int ReceitaVeterinariaId { get; set; }
    public ReceitaVeterinaria ReceitaVeterinaria { get; set; } = null!;

    public string Medicamento { get; set; } = string.Empty;
    public string? PrincipioAtivo { get; set; }
    public string? Concentracao { get; set; }
    public string Posologia { get; set; } = string.Empty;
    public string? ViaAdministracao { get; set; }
    public int? DuracaoDias { get; set; }
    public string? Quantidade { get; set; }
}

/// <summary>
/// Carteira de Vacinação, Vermifugação e Antiparasitários do Pet com controle de reforço.
/// </summary>
public class VacinacaoVermifugoPet : BaseEmpresaEntity, IAggregateRoot
{
    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;

    public int? VeterinarioId { get; set; }
    public Veterinario? Veterinario { get; set; }

    public int? ProdutoId { get; set; }
    public Produto? Produto { get; set; }

    public TipoPreventivoPet Tipo { get; set; } = TipoPreventivoPet.Vacina;
    public string NomeProduto { get; set; } = string.Empty;
    public string? Fabricante { get; set; }
    public string? NumeroLote { get; set; }
    public DateTime? ValidadeLote { get; set; }
    public string? Dose { get; set; }

    public DateTime DataAplicacao { get; set; } = DateTime.UtcNow;
    public DateTime? ProximaAplicacao { get; set; }
    public string? Observacoes { get; set; }
}
