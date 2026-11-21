using ERP.Domain.Common;
using ERP.Domain.Core.Entities.Cadastros;
using ERP.Domain.Core.Enums;

namespace ERP.Domain.Core.Entities.Pet;

public class BaiaInternacao : BaseEmpresaEntity, IAggregateRoot
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public SetorInternacao Setor { get; set; } = SetorInternacao.Canil;
    public StatusBaiaInternacao Status { get; set; } = StatusBaiaInternacao.Livre;
    public decimal ValorDiaria { get; set; }
}

public class InternacaoPet : BaseEmpresaEntity, IAggregateRoot
{
    public string Numero { get; set; } = string.Empty;

    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public int VeterinarioId { get; set; }
    public Veterinario Veterinario { get; set; } = null!;

    public int BaiaInternacaoId { get; set; }
    public BaiaInternacao BaiaInternacao { get; set; } = null!;

    public int? ComandaAtendimentoId { get; set; }
    public ComandaAtendimento? ComandaAtendimento { get; set; }

    public DateTime DataEntrada { get; set; } = DateTime.UtcNow;
    public DateTime? DataAltaPrevista { get; set; }
    public DateTime? DataAltaReal { get; set; }
    public string MotivoInternacao { get; set; } = string.Empty;
    public StatusInternacaoPet Status { get; set; } = StatusInternacaoPet.Ativa;
    public decimal ValorDiaria { get; set; }

    public ICollection<PrescricaoInternacao> Prescricoes { get; set; } = new List<PrescricaoInternacao>();
    public ICollection<EvolucaoInternacao> Evolucoes { get; set; } = new List<EvolucaoInternacao>();
}

public class PrescricaoInternacao : BaseEmpresaEntity
{
    public int InternacaoPetId { get; set; }
    public InternacaoPet InternacaoPet { get; set; } = null!;

    public int? ProdutoId { get; set; }
    public Produto? Produto { get; set; }

    public string DescricaoMedicamento { get; set; } = string.Empty;
    public string Dose { get; set; } = string.Empty;
    public string? Via { get; set; }
    public decimal QuantidadePorAplicacao { get; set; } = 1;
    public int IntervaloHoras { get; set; } = 8;
    public decimal ValorUnitarioCobranca { get; set; }
    public DateTime HorarioInicio { get; set; } = DateTime.UtcNow;

    public ICollection<ChecagemEnfermagem> Checagens { get; set; } = new List<ChecagemEnfermagem>();
}

public class ChecagemEnfermagem : BaseEmpresaEntity
{
    public int PrescricaoInternacaoId { get; set; }
    public PrescricaoInternacao PrescricaoInternacao { get; set; } = null!;

    public DateTime DataHoraPrevista { get; set; }
    public DateTime? DataHoraAplicada { get; set; }
    public StatusChecagemEnfermagem Status { get; set; } = StatusChecagemEnfermagem.Pendente;
    public string? ResponsavelEnfermagem { get; set; }
    public string? Observacoes { get; set; }
}

public class EvolucaoInternacao : BaseEmpresaEntity
{
    public int InternacaoPetId { get; set; }
    public InternacaoPet InternacaoPet { get; set; } = null!;

    public DateTime DataHora { get; set; } = DateTime.UtcNow;
    public string Turno { get; set; } = "Manhã";
    public string ProfissionalNome { get; set; } = string.Empty;
    public decimal? TemperaturaC { get; set; }
    public int? FrequenciaCardiacaBpm { get; set; }
    public int? FrequenciaRespiratoriaMpm { get; set; }
    public string? EstadoGeral { get; set; }
    public string DescricaoEvolucao { get; set; } = string.Empty;
    public bool VisivelBoletimTutor { get; set; } = true;
}

public class CirurgiaVeterinaria : BaseEmpresaEntity, IAggregateRoot
{
    public string Numero { get; set; } = string.Empty;

    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public int VeterinarioCirurgiaoId { get; set; }
    public Veterinario VeterinarioCirurgiao { get; set; } = null!;

    public int? ComandaAtendimentoId { get; set; }
    public ComandaAtendimento? ComandaAtendimento { get; set; }

    public string? AnestesistaNome { get; set; }
    public string? SalaCirurgica { get; set; }
    public string Procedimento { get; set; } = string.Empty;
    public DateTime DataHoraAgendada { get; set; } = DateTime.UtcNow;
    public StatusCirurgiaVet Status { get; set; } = StatusCirurgiaVet.Agendada;

    public string? ProtocoloAnestesico { get; set; }
    public string? MonitoramentoTransOperatorio { get; set; }
    public string? Intercorrencias { get; set; }
    public decimal ValorProcedimento { get; set; }
}

public class TermoConsentimentoPet : BaseEmpresaEntity, IAggregateRoot
{
    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public TipoTermoConsentimento Tipo { get; set; } = TipoTermoConsentimento.Cirurgia;
    public string Titulo { get; set; } = string.Empty;
    public string ConteudoTermo { get; set; } = string.Empty;
    public bool Assinado { get; set; }
    public DateTime? DataAssinatura { get; set; }
    public string? AssinadoPorNome { get; set; }
    public string? TokenAssinatura { get; set; }
}

public class ExameVeterinario : BaseEmpresaEntity, IAggregateRoot
{
    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;

    public int? VeterinarioSolicitanteId { get; set; }
    public Veterinario? VeterinarioSolicitante { get; set; }

    public TipoExameVet Tipo { get; set; } = TipoExameVet.Hematologia;
    public string NomeExame { get; set; } = string.Empty;
    public string CodigoAmostraLis { get; set; } = string.Empty;
    public StatusExameVet Status { get; set; } = StatusExameVet.Solicitado;

    public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataResultado { get; set; }
    public string? ResultadoTexto { get; set; }
    public string? LaudoUrl { get; set; }
    public string? PayloadHl7Bruto { get; set; }
}
