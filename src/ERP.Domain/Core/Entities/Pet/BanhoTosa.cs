using ERP.Domain.Common;
using ERP.Domain.Core.Entities.Cadastros;
using ERP.Domain.Core.Enums;

namespace ERP.Domain.Core.Entities.Pet;

/// <summary>
/// Matriz de preço e duração de Banho & Tosa por Porte x Pelagem.
/// </summary>
public class TabelaPrecoBanho : BaseEmpresaEntity, IAggregateRoot
{
    public int? ServicoId { get; set; }
    public Servico? Servico { get; set; }

    public string NomeServico { get; set; } = string.Empty;
    public PortePet Porte { get; set; } = PortePet.M;
    public TipoPelagemPet TipoPelagem { get; set; } = TipoPelagemPet.Curta;
    public decimal Valor { get; set; }
    public int DuracaoMinutos { get; set; } = 60;

    public decimal ComissaoBanhoPercentual { get; set; } = 20m;
    public decimal ComissaoTosaPercentual { get; set; } = 30m;
}

/// <summary>
/// Recursos físicos da estética pet (Banheiras, Mesas de Tosa, Boxes).
/// </summary>
public class RecursoBanhoTosa : BaseEmpresaEntity, IAggregateRoot
{
    public string Nome { get; set; } = string.Empty;
    public TipoRecursoBanho Tipo { get; set; } = TipoRecursoBanho.Banheira;
}

/// <summary>
/// Pacotes de sessões de Banho & Tosa (ex: 4 banhos/mês) com controle de saldo e validade.
/// </summary>
public class PacoteBanhoPet : BaseEmpresaEntity, IAggregateRoot
{
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public int? PetId { get; set; }
    public Pet? Pet { get; set; }

    public string NomePacote { get; set; } = string.Empty;
    public int TotalSessoes { get; set; } = 4;
    public int SessoesUtilizadas { get; set; }
    public decimal ValorTotal { get; set; }

    public DateTime DataInicio { get; set; } = DateTime.UtcNow;
    public DateTime DataValidade { get; set; } = DateTime.UtcNow.AddDays(30);
    public string? Observacoes { get; set; }

    public int SessoesDisponiveis => Math.Max(0, TotalSessoes - SessoesUtilizadas);
    public decimal ValorProporcionalSessao => TotalSessoes > 0 ? Math.Round(ValorTotal / TotalSessoes, 2) : 0m;
}

/// <summary>
/// Atendimento de Banho & Tosa (Agenda + Check-in/Checklist + Fila em tempo real + Comanda).
/// </summary>
public class AtendimentoBanhoTosa : BaseEmpresaEntity, IAggregateRoot
{
    public string Numero { get; set; } = string.Empty;

    public int PetId { get; set; }
    public Pet Pet { get; set; } = null!;

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public int? TabelaPrecoBanhoId { get; set; }
    public TabelaPrecoBanho? TabelaPrecoBanho { get; set; }

    public int? PacoteBanhoPetId { get; set; }
    public PacoteBanhoPet? PacoteBanhoPet { get; set; }

    public int? ComandaAtendimentoId { get; set; }
    public ComandaAtendimento? ComandaAtendimento { get; set; }

    public int? RecursoBanhoTosaId { get; set; }
    public RecursoBanhoTosa? RecursoBanhoTosa { get; set; }

    public string? BanhistaNome { get; set; }
    public string? TosadorNome { get; set; }

    public DateTime DataAgendada { get; set; } = DateTime.UtcNow;
    public int DuracaoMinutos { get; set; } = 60;
    public StatusAtendimentoBanho Status { get; set; } = StatusAtendimentoBanho.Agendado;

    public decimal ValorServico { get; set; }
    public decimal ValorAdicionais { get; set; }
    public decimal ValorTotal { get; set; }
    public bool CobertoPorPacote { get; set; }

    // Check-in & Checklist de Entrada
    public DateTime? CheckInEm { get; set; }
    public bool TemNos { get; set; }
    public bool TemPulgasCarrapatos { get; set; }
    public bool TemLesoesPele { get; set; }
    public bool TemOtite { get; set; }
    public string? Pertences { get; set; }
    public string? ObservacoesCheckIn { get; set; }
    public string? FotoEntradaUrl { get; set; }
    public string? FotoSaidaUrl { get; set; }

    public DateTime? ProntoEm { get; set; }
    public DateTime? EntregueEm { get; set; }

    public ICollection<ComissaoBanhoTosa> Comissoes { get; set; } = new List<ComissaoBanhoTosa>();
}

/// <summary>
/// Comissão calculada por etapa (Banhista / Tosador) para um atendimento de Banho & Tosa.
/// </summary>
public class ComissaoBanhoTosa : BaseEmpresaEntity
{
    public int AtendimentoBanhoTosaId { get; set; }
    public AtendimentoBanhoTosa AtendimentoBanhoTosa { get; set; } = null!;

    public string ProfissionalNome { get; set; } = string.Empty;
    public EtapaComissaoBanho Etapa { get; set; } = EtapaComissaoBanho.Banho;
    public decimal BaseCalculo { get; set; }
    public decimal Percentual { get; set; }
    public decimal ValorComissao { get; set; }
    public bool Paga { get; set; }
    public DateTime? DataPagamento { get; set; }
}
