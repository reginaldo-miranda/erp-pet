namespace ERP.Application.Common.Interfaces;

public record WebImageResult(string Title, string Url, string ThumbnailUrl);

public interface IWebImageSearchService
{
    Task<List<WebImageResult>> BuscarImagensAsync(
        string termo,
        string? categoria = null,
        int limite = 4,
        CancellationToken cancellationToken = default);

    Task<(byte[] Bytes, string Extensao)?> BaixarImagemAsync(
        string url,
        string? fallbackUrl = null,
        CancellationToken cancellationToken = default);
}
