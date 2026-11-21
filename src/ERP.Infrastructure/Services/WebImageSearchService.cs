using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using ERP.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace ERP.Infrastructure.Services;

/// <summary>
/// Busca imagens de produtos na web (Bing como primário e DuckDuckGo como contingência) e faz download local.
/// </summary>
public partial class WebImageSearchService : IWebImageSearchService
{
    private const string BrowserUserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";

    private readonly HttpClient _httpClient;
    private readonly ILogger<WebImageSearchService> _logger;

    public WebImageSearchService(HttpClient httpClient, ILogger<WebImageSearchService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<WebImageResult>> BuscarImagensAsync(
        string termo,
        string? categoria = null,
        int limite = 4,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return new List<WebImageResult>();

        var query = MontarQuery(termo, categoria);

        var bing = await BuscarBingAsync(query, limite, cancellationToken);
        if (bing.Count > 0)
            return bing;

        return await BuscarDuckDuckGoAsync(query, limite, cancellationToken);
    }

    public async Task<(byte[] Bytes, string Extensao)?> BaixarImagemAsync(
        string url,
        string? fallbackUrl = null,
        CancellationToken cancellationToken = default)
    {
        var candidatos = new List<string>();
        if (!string.IsNullOrWhiteSpace(url)) candidatos.Add(url);
        if (!string.IsNullOrWhiteSpace(fallbackUrl) && !string.Equals(fallbackUrl, url, StringComparison.OrdinalIgnoreCase))
            candidatos.Add(fallbackUrl);

        foreach (var candidato in candidatos)
        {
            if (!Uri.TryCreate(candidato, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                continue;

            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Get, uri);
                req.Headers.UserAgent.ParseAdd(BrowserUserAgent);
                req.Headers.Accept.ParseAdd("image/avif,image/webp,image/apng,image/*,*/*;q=0.8");

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(6));

                using var res = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                if (!res.IsSuccessStatusCode) continue;

                var contentType = res.Content.Headers.ContentType?.MediaType?.ToLowerInvariant() ?? string.Empty;
                if (!string.IsNullOrEmpty(contentType) && !contentType.StartsWith("image/"))
                    continue;

                var bytes = await res.Content.ReadAsByteArrayAsync(cts.Token);
                if (bytes.Length == 0 || bytes.Length > 5 * 1024 * 1024)
                    continue;

                var ext = ResolverExtensao(contentType, uri.AbsolutePath);
                return (bytes, ext);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao baixar imagem candidata {Url}", candidato);
            }
        }

        return null;
    }

    private static string MontarQuery(string termo, string? categoria)
    {
        var cleanQ = termo.Trim();
        if (string.IsNullOrWhiteSpace(categoria))
            return cleanQ;

        var cleanCat = categoria.Trim();
        var singularCat = cleanCat.EndsWith("s", StringComparison.OrdinalIgnoreCase) && cleanCat.Length > 3
            ? cleanCat[..^1]
            : cleanCat;

        if (!cleanQ.Contains(singularCat, StringComparison.OrdinalIgnoreCase) &&
            !cleanQ.Contains(cleanCat, StringComparison.OrdinalIgnoreCase))
        {
            return $"{singularCat} {cleanQ}";
        }

        return cleanQ;
    }

    private async Task<List<WebImageResult>> BuscarBingAsync(string query, int limite, CancellationToken cancellationToken)
    {
        var results = new List<WebImageResult>();
        try
        {
            var url = $"https://www.bing.com/images/search?q={Uri.EscapeDataString(query)}&form=HDRSC2&first=1";
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.UserAgent.ParseAdd(BrowserUserAgent);
            req.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            req.Headers.AcceptLanguage.ParseAdd("pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7");

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(6));

            using var res = await _httpClient.SendAsync(req, cts.Token);
            if (!res.IsSuccessStatusCode) return results;

            var html = await res.Content.ReadAsStringAsync(cts.Token);
            foreach (Match match in BingIuscRegex().Matches(html))
            {
                try
                {
                    var decoded = WebUtility.HtmlDecode(match.Groups[1].Value);
                    using var doc = JsonDocument.Parse(decoded);
                    var root = doc.RootElement;

                    var murl = root.TryGetProperty("murl", out var mProp) ? mProp.GetString() : null;
                    if (string.IsNullOrWhiteSpace(murl)) continue;

                    var turl = root.TryGetProperty("turl", out var tProp) ? tProp.GetString() : null;
                    var title = root.TryGetProperty("t", out var titleProp) ? titleProp.GetString() : null;

                    results.Add(new WebImageResult(
                        title ?? query,
                        murl,
                        string.IsNullOrWhiteSpace(turl) ? murl : turl));

                    if (results.Count >= limite) break;
                }
                catch
                {
                    // Ignora item malformado e continua
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Aviso na busca de imagens no Bing para '{Query}'", query);
        }

        return results;
    }

    private async Task<List<WebImageResult>> BuscarDuckDuckGoAsync(string query, int limite, CancellationToken cancellationToken)
    {
        var results = new List<WebImageResult>();
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(8));

            using var tokenReq = new HttpRequestMessage(HttpMethod.Get, $"https://duckduckgo.com/?q={Uri.EscapeDataString(query)}");
            tokenReq.Headers.UserAgent.ParseAdd(BrowserUserAgent);

            using var tokenRes = await _httpClient.SendAsync(tokenReq, cts.Token);
            if (!tokenRes.IsSuccessStatusCode) return results;

            var html = await tokenRes.Content.ReadAsStringAsync(cts.Token);
            var vqdMatch = DuckDuckGoVqdRegex().Match(html);
            if (!vqdMatch.Success) return results;

            var vqd = vqdMatch.Groups[1].Success ? vqdMatch.Groups[1].Value : vqdMatch.Groups[2].Value;
            var imgUrl = $"https://duckduckgo.com/i.js?l=wt-wt&o=json&q={Uri.EscapeDataString(query)}&vqd={Uri.EscapeDataString(vqd)}&f=,,,";

            using var imgReq = new HttpRequestMessage(HttpMethod.Get, imgUrl);
            imgReq.Headers.UserAgent.ParseAdd(BrowserUserAgent);
            imgReq.Headers.Referrer = new Uri("https://duckduckgo.com/");

            using var imgRes = await _httpClient.SendAsync(imgReq, cts.Token);
            if (!imgRes.IsSuccessStatusCode) return results;

            var json = await imgRes.Content.ReadAsStringAsync(cts.Token);
            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("results", out var items) || items.ValueKind != JsonValueKind.Array)
                return results;

            foreach (var item in items.EnumerateArray())
            {
                var image = item.TryGetProperty("image", out var iProp) ? iProp.GetString() : null;
                if (string.IsNullOrWhiteSpace(image)) continue;

                var thumb = item.TryGetProperty("thumbnail", out var thProp) ? thProp.GetString() : null;
                var title = item.TryGetProperty("title", out var ttProp) ? ttProp.GetString() : null;

                results.Add(new WebImageResult(
                    title ?? query,
                    image,
                    string.IsNullOrWhiteSpace(thumb) ? image : thumb));

                if (results.Count >= limite) break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Aviso na busca de imagens no DuckDuckGo para '{Query}'", query);
        }

        return results;
    }

    private static string ResolverExtensao(string contentType, string path)
    {
        if (contentType.Contains("png")) return ".png";
        if (contentType.Contains("webp")) return ".webp";
        if (contentType.Contains("jpeg") || contentType.Contains("jpg")) return ".jpg";

        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext is ".jpg" or ".jpeg" or ".png" or ".webp" ? ext : ".jpg";
    }

    [GeneratedRegex(@"class=""iusc""[^>]*m=""([^""]+)""", RegexOptions.Compiled)]
    private static partial Regex BingIuscRegex();

    [GeneratedRegex(@"vqd=([0-9-]+)|vqd=['""]([0-9-]+)['""]", RegexOptions.Compiled)]
    private static partial Regex DuckDuckGoVqdRegex();
}
