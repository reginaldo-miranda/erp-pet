using ERP.Application.Common.Interfaces;
using ERP.Domain.Core.Enums;
using ERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ERP.Infrastructure.Services;

/// <summary>
/// Worker em background que processa periodicamente a fila Outbox de notificações WhatsApp.
/// </summary>
public class WhatsAppOutboxWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WhatsAppOutboxWorker> _logger;

    public WhatsAppOutboxWorker(IServiceScopeFactory scopeFactory, ILogger<WhatsAppOutboxWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var sender = scope.ServiceProvider.GetRequiredService<IWhatsAppSenderService>();

                var pendentes = await db.NotificacoesWhatsAppOutbox
                    .IgnoreQueryFilters()
                    .Where(n => n.Status == StatusNotificacaoWhatsApp.Pendente && n.DataAgendada <= DateTime.UtcNow)
                    .OrderBy(n => n.DataAgendada)
                    .Take(25)
                    .ToListAsync(stoppingToken);

                if (pendentes.Count == 0) continue;

                var configs = await db.ConfiguracoesWhatsAppEmpresa
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .ToDictionaryAsync(c => c.EmpresaId, stoppingToken);

                foreach (var msg in pendentes)
                {
                    configs.TryGetValue(msg.EmpresaId, out var cfg);
                    msg.Tentativas += 1;

                    var res = await sender.EnviarMensagemAsync(
                        cfg?.Provedor ?? "Simulado",
                        cfg?.BaseUrl,
                        cfg?.InstanceName,
                        cfg?.ApiKey,
                        msg.TelefoneDestino,
                        msg.Mensagem,
                        stoppingToken);

                    if (res.Sucesso)
                    {
                        msg.Status = StatusNotificacaoWhatsApp.Enviada;
                        msg.DataEnvio = DateTime.UtcNow;
                        msg.ErroDetalhe = null;
                    }
                    else
                    {
                        msg.Status = StatusNotificacaoWhatsApp.Erro;
                        msg.ErroDetalhe = res.Erro;
                    }
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar fila Outbox de WhatsApp.");
            }
        }
    }
}
