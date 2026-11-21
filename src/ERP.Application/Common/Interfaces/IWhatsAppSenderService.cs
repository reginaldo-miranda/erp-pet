namespace ERP.Application.Common.Interfaces;

public record WhatsAppSendResult(bool Sucesso, string? Erro);

public interface IWhatsAppSenderService
{
    Task<WhatsAppSendResult> EnviarMensagemAsync(
        string provedor,
        string? baseUrl,
        string? instanceName,
        string? apiKey,
        string telefoneDestino,
        string mensagem,
        CancellationToken cancellationToken = default);
}
