using ERP.Application.Common.Behaviors;
using ERP.Application.Common.Interfaces;
using ERP.Application.Common.Models;
using ERP.Domain.Core.Entities.Pet;
using ERP.Domain.Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Pet.TutorPortal;

public record ConfigWhatsAppDto(
    string Provedor,
    string? BaseUrl,
    string? InstanceName,
    string? ApiKey,
    bool EnviarBanhoPronto,
    bool EnviarLembreteVacina,
    bool EnviarBoletimInternacao);

public record OutboxWhatsAppDto(
    int Id,
    string TelefoneDestino,
    TipoEventoWhatsApp TipoEvento,
    string Mensagem,
    StatusNotificacaoWhatsApp Status,
    DateTime DataAgendada,
    DateTime? DataEnvio,
    string? ErroDetalhe);

public record PortalPetResumoDto(int Id, string Nome, EspeciePet Especie, string? Raca, decimal? PesoAtualKg);
public record PortalVacinaDto(string NomePet, string NomeProduto, DateTime DataAplicacao, DateTime? ProximaAplicacao);
public record PortalExameDto(string NomePet, string NomeExame, StatusExameVet Status, string? ResultadoTexto);
public record PortalTermoDto(int Id, string NomePet, string Titulo, bool Assinado, DateTime? DataAssinatura);
public record PortalBoletimDto(string NomePet, DateTime DataHora, string Turno, string DescricaoEvolucao);

public record DadosPortalTutorDto(
    int ClienteId,
    string NomeTutor,
    string TokenAcesso,
    List<PortalPetResumoDto> Pets,
    List<PortalVacinaDto> Vacinas,
    List<PortalExameDto> Exames,
    List<PortalTermoDto> Termos,
    List<PortalBoletimDto> BoletinsInternacao);

// ==================== WHATSAPP ====================

[RequerModulo(ModulosPet.WhatsApp)]
public record GetConfigWhatsAppQuery : IRequest<Result<ConfigWhatsAppDto>>;

public class GetConfigWhatsAppQueryHandler : IRequestHandler<GetConfigWhatsAppQuery, Result<ConfigWhatsAppDto>>
{
    private readonly IApplicationDbContext _context;
    public GetConfigWhatsAppQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<ConfigWhatsAppDto>> Handle(GetConfigWhatsAppQuery request, CancellationToken cancellationToken)
    {
        var cfg = await _context.ConfiguracoesWhatsAppEmpresa.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        return Result<ConfigWhatsAppDto>.Ok(cfg == null
            ? new ConfigWhatsAppDto("EvolutionApi", null, null, null, true, true, true)
            : new ConfigWhatsAppDto(cfg.Provedor, cfg.BaseUrl, cfg.InstanceName, cfg.ApiKey, cfg.EnviarBanhoPronto, cfg.EnviarLembreteVacina, cfg.EnviarBoletimInternacao));
    }
}

[RequerModulo(ModulosPet.WhatsApp)]
public record SalvarConfigWhatsAppCommand(
    string Provedor,
    string? BaseUrl,
    string? InstanceName,
    string? ApiKey,
    bool EnviarBanhoPronto,
    bool EnviarLembreteVacina,
    bool EnviarBoletimInternacao) : IRequest<Result<bool>>;

public class SalvarConfigWhatsAppCommandHandler : IRequestHandler<SalvarConfigWhatsAppCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public SalvarConfigWhatsAppCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<bool>> Handle(SalvarConfigWhatsAppCommand request, CancellationToken cancellationToken)
    {
        var cfg = await _context.ConfiguracoesWhatsAppEmpresa.FirstOrDefaultAsync(cancellationToken);
        if (cfg == null)
        {
            cfg = new ConfiguracaoWhatsAppEmpresa { EmpresaId = _empresaService.EmpresaId ?? 0 };
            _context.ConfiguracoesWhatsAppEmpresa.Add(cfg);
        }

        cfg.Provedor = request.Provedor;
        cfg.BaseUrl = request.BaseUrl;
        cfg.InstanceName = request.InstanceName;
        cfg.ApiKey = request.ApiKey;
        cfg.EnviarBanhoPronto = request.EnviarBanhoPronto;
        cfg.EnviarLembreteVacina = request.EnviarLembreteVacina;
        cfg.EnviarBoletimInternacao = request.EnviarBoletimInternacao;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}

[RequerModulo(ModulosPet.WhatsApp)]
public record GetOutboxWhatsAppQuery : IRequest<Result<List<OutboxWhatsAppDto>>>;

public class GetOutboxWhatsAppQueryHandler : IRequestHandler<GetOutboxWhatsAppQuery, Result<List<OutboxWhatsAppDto>>>
{
    private readonly IApplicationDbContext _context;
    public GetOutboxWhatsAppQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<OutboxWhatsAppDto>>> Handle(GetOutboxWhatsAppQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.NotificacoesWhatsAppOutbox.AsNoTracking()
            .OrderByDescending(n => n.DataAgendada)
            .Take(100)
            .Select(n => new OutboxWhatsAppDto(n.Id, n.TelefoneDestino, n.TipoEvento, n.Mensagem, n.Status, n.DataAgendada, n.DataEnvio, n.ErroDetalhe))
            .ToListAsync(cancellationToken);
        return Result<List<OutboxWhatsAppDto>>.Ok(list);
    }
}

[RequerModulo(ModulosPet.WhatsApp)]
public record EnfileirarWhatsAppCommand(int? ClienteId, int? PetId, string TelefoneDestino, TipoEventoWhatsApp TipoEvento, string Mensagem) : IRequest<Result<int>>;

public class EnfileirarWhatsAppCommandHandler : IRequestHandler<EnfileirarWhatsAppCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public EnfileirarWhatsAppCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(EnfileirarWhatsAppCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TelefoneDestino) || string.IsNullOrWhiteSpace(request.Mensagem))
            return Result<int>.Falha("Telefone e mensagem são obrigatórios.");

        var item = new NotificacaoWhatsAppOutbox
        {
            EmpresaId = _empresaService.EmpresaId ?? 0,
            ClienteId = request.ClienteId,
            PetId = request.PetId,
            TelefoneDestino = request.TelefoneDestino.Trim(),
            TipoEvento = request.TipoEvento,
            Mensagem = request.Mensagem.Trim(),
            Status = StatusNotificacaoWhatsApp.Pendente,
            DataAgendada = DateTime.UtcNow
        };
        _context.NotificacoesWhatsAppOutbox.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(item.Id);
    }
}

[RequerModulo(ModulosPet.WhatsApp)]
public record DispararOutboxWhatsAppCommand : IRequest<Result<int>>;

public class DispararOutboxWhatsAppCommandHandler : IRequestHandler<DispararOutboxWhatsAppCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IWhatsAppSenderService _sender;
    public DispararOutboxWhatsAppCommandHandler(IApplicationDbContext context, IWhatsAppSenderService sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<Result<int>> Handle(DispararOutboxWhatsAppCommand request, CancellationToken cancellationToken)
    {
        var cfg = await _context.ConfiguracoesWhatsAppEmpresa.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        var pendentes = await _context.NotificacoesWhatsAppOutbox
            .Where(n => n.Status == StatusNotificacaoWhatsApp.Pendente)
            .OrderBy(n => n.DataAgendada)
            .Take(50)
            .ToListAsync(cancellationToken);

        var enviados = 0;
        foreach (var msg in pendentes)
        {
            msg.Tentativas += 1;
            var res = await _sender.EnviarMensagemAsync(
                cfg?.Provedor ?? "Simulado", cfg?.BaseUrl, cfg?.InstanceName, cfg?.ApiKey,
                msg.TelefoneDestino, msg.Mensagem, cancellationToken);

            if (res.Sucesso)
            {
                msg.Status = StatusNotificacaoWhatsApp.Enviada;
                msg.DataEnvio = DateTime.UtcNow;
                msg.ErroDetalhe = null;
                enviados++;
            }
            else
            {
                msg.Status = StatusNotificacaoWhatsApp.Erro;
                msg.ErroDetalhe = res.Erro;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(enviados);
    }
}

// ==================== PORTAL DO TUTOR & AGENDAMENTO ONLINE ====================

[RequerModulo(ModulosPet.PortalTutor)]
public record GerarAcessoPortalTutorCommand(int ClienteId) : IRequest<Result<string>>;

public class GerarAcessoPortalTutorCommandHandler : IRequestHandler<GerarAcessoPortalTutorCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;
    public GerarAcessoPortalTutorCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<string>> Handle(GerarAcessoPortalTutorCommand request, CancellationToken cancellationToken)
    {
        var acesso = await _context.AcessosPortalTutor
            .FirstOrDefaultAsync(a => a.ClienteId == request.ClienteId && a.ValidoAte > DateTime.UtcNow, cancellationToken);

        if (acesso == null)
        {
            acesso = new AcessoPortalTutor
            {
                EmpresaId = _empresaService.EmpresaId ?? 0,
                ClienteId = request.ClienteId,
                TokenAcesso = Guid.NewGuid().ToString("N")[..12].ToUpperInvariant(),
                ValidoAte = DateTime.UtcNow.AddDays(30)
            };
            _context.AcessosPortalTutor.Add(acesso);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Result<string>.Ok(acesso.TokenAcesso);
    }
}

[RequerModulo(ModulosPet.PortalTutor)]
public record GetDadosPortalTutorQuery(int? ClienteId = null, string? Token = null) : IRequest<Result<DadosPortalTutorDto>>;

public class GetDadosPortalTutorQueryHandler : IRequestHandler<GetDadosPortalTutorQuery, Result<DadosPortalTutorDto>>
{
    private readonly IApplicationDbContext _context;
    public GetDadosPortalTutorQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<DadosPortalTutorDto>> Handle(GetDadosPortalTutorQuery request, CancellationToken cancellationToken)
    {
        int clienteId;
        string token = request.Token ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(request.Token))
        {
            var acesso = await _context.AcessosPortalTutor
                .FirstOrDefaultAsync(a => a.TokenAcesso == request.Token.Trim(), cancellationToken);
            if (acesso == null || acesso.ValidoAte < DateTime.UtcNow)
                return Result<DadosPortalTutorDto>.Falha("Token de acesso do Portal inválido ou expirado.");
            acesso.UltimoAcessoEm = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            clienteId = acesso.ClienteId;
        }
        else if (request.ClienteId.HasValue)
        {
            clienteId = request.ClienteId.Value;
        }
        else
        {
            return Result<DadosPortalTutorDto>.Falha("Informe um Tutor ou Token.");
        }

        var cliente = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == clienteId, cancellationToken);
        if (cliente == null) return Result<DadosPortalTutorDto>.Falha("Tutor não encontrado.");

        var petIds = await _context.TutorPets.AsNoTracking()
            .Where(tp => tp.ClienteId == clienteId)
            .Select(tp => tp.PetId)
            .ToListAsync(cancellationToken);

        var pets = await _context.Pets.AsNoTracking()
            .Where(p => petIds.Contains(p.Id))
            .Select(p => new PortalPetResumoDto(p.Id, p.Nome, p.Especie, p.Raca, p.PesoAtualKg))
            .ToListAsync(cancellationToken);

        var vacinas = await _context.VacinacoesVermifugosPet.AsNoTracking()
            .Include(v => v.Pet)
            .Where(v => petIds.Contains(v.PetId))
            .OrderByDescending(v => v.DataAplicacao)
            .Select(v => new PortalVacinaDto(v.Pet.Nome, v.NomeProduto, v.DataAplicacao, v.ProximaAplicacao))
            .ToListAsync(cancellationToken);

        var exames = await _context.ExamesVeterinarios.AsNoTracking()
            .Include(e => e.Pet)
            .Where(e => petIds.Contains(e.PetId))
            .OrderByDescending(e => e.DataSolicitacao)
            .Select(e => new PortalExameDto(e.Pet.Nome, e.NomeExame, e.Status, e.ResultadoTexto))
            .ToListAsync(cancellationToken);

        var termos = await _context.TermosConsentimentoPet.AsNoTracking()
            .Include(t => t.Pet)
            .Where(t => petIds.Contains(t.PetId))
            .OrderByDescending(t => t.Id)
            .Select(t => new PortalTermoDto(t.Id, t.Pet.Nome, t.Titulo, t.Assinado, t.DataAssinatura))
            .ToListAsync(cancellationToken);

        var boletins = await _context.EvolucoesInternacao.AsNoTracking()
            .Include(e => e.InternacaoPet).ThenInclude(i => i.Pet)
            .Where(e => petIds.Contains(e.InternacaoPet.PetId) && e.VisivelBoletimTutor)
            .OrderByDescending(e => e.DataHora)
            .Take(20)
            .Select(e => new PortalBoletimDto(e.InternacaoPet.Pet.Nome, e.DataHora, e.Turno, e.DescricaoEvolucao))
            .ToListAsync(cancellationToken);

        return Result<DadosPortalTutorDto>.Ok(new DadosPortalTutorDto(
            cliente.Id, cliente.Nome, token, pets, vacinas, exames, termos, boletins));
    }
}
