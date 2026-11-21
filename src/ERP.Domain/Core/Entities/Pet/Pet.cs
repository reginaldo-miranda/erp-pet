using ERP.Domain.Common;
using ERP.Domain.Core.Entities.Cadastros;
using ERP.Domain.Core.Enums;

namespace ERP.Domain.Core.Entities.Pet;

/// <summary>
/// Cadastro central do Pet (paciente / cliente de estética).
/// Todo o histórico clínico, vacinal e de banho e tosa é vinculado ao Pet.
/// </summary>
public class Pet : BaseEmpresaEntity, IAggregateRoot
{
    public string Nome { get; set; } = string.Empty;
    public EspeciePet Especie { get; set; } = EspeciePet.Cao;
    public string? Raca { get; set; }
    public string? CorPelagem { get; set; }
    public SexoPet Sexo { get; set; } = SexoPet.Indefinido;
    public bool Castrado { get; set; }
    public DateTime? DataNascimento { get; set; }
    public int? IdadeEstimadaMeses { get; set; }
    public PortePet Porte { get; set; } = PortePet.M;
    public TipoPelagemPet TipoPelagem { get; set; } = TipoPelagemPet.Curta;
    public decimal? PesoAtualKg { get; set; }
    public string? Microchip { get; set; }
    public string? FotoUrl { get; set; }

    // Saúde e Comportamento
    public TemperamentoPet Temperamento { get; set; } = TemperamentoPet.Docil;
    public string? Alergias { get; set; }
    public string? RestricoesAlimentares { get; set; }
    public string? Observacoes { get; set; }

    // Convênio Pet
    public int? ConvenioPetId { get; set; }
    public ConvenioPet? ConvenioPet { get; set; }
    public string? NumeroCarteirinhaConvenio { get; set; }

    // Óbito
    public bool Obito { get; set; }
    public DateTime? DataObito { get; set; }
    public string? CausaObito { get; set; }

    // Navegação
    public ICollection<TutorPet> Tutores { get; set; } = new List<TutorPet>();
}

/// <summary>
/// Relacionamento N:N entre Tutor (Cliente) e Pet.
/// Permite Tutor Principal, Tutores Secundários e Pessoas Autorizadas a Retirar.
/// </summary>
public class TutorPet : BaseEmpresaEntity
{
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;

    public TipoVinculoTutor TipoVinculo { get; set; } = TipoVinculoTutor.Principal;
    public bool PodeAssinarTermos { get; set; } = true;
    public bool RecebeNotificacoes { get; set; } = true;
}
