using System.Net.Http.Json;
using ERP.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace ERP.Infrastructure.Services;

public class WhatsAppSenderService : IWhatsAppSenderService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WhatsAppSenderService> _logger;

    public WhatsAppSenderService(HttpClient httpClient, ILogger<WhatsAppSenderService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<WhatsAppSendResult> EnviarMensagemAsync(
        string provedor,
        string? baseUrl,
        string? instanceName,
        string? apiKey,
        string telefoneDestino,
        string mensagem,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            _logger.LogInformation("[WhatsApp Simulado -> {Telefone}] {Mensagem}", telefoneDestino, mensagem);
            return new WhatsAppSendResult(true, null);
        }

        try
        {
            var url = provedor.Equals("EvolutionApi", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(instanceName)
                ? $"{baseUrl.TrimEnd('/')}/message/sendText/{instanceName}"
                : baseUrl;

            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            if (!string.IsNullOrWhiteSpace(apiKey))
                req.Headers.TryAddWithoutValidation("apikey", apiKey);

            req.Content = JsonContent.Create(new { number = telefoneDestino, text = mensagem });
            var resp = await _httpClient.SendAsync(req, cancellationToken);
            if (!resp.IsSuccessStatusCode)
                return new WhatsAppSendResult(false, $"HTTP {(int)resp.StatusCode}: {resp.ReasonPhrase}");

            return new WhatsAppSendResult(true, null);
        }
        catch (Exception ex)
        {
            return new WhatsAppSendResult(false, ex.Message);
        }
    }
}
