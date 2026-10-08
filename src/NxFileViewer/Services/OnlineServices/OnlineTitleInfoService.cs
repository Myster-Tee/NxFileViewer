using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.IO;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.OnlineServices;

public class OnlineTitleInfoService : IOnlineTitleInfoService, ITitleDbUpdater
{
    private readonly IAppSettings _appSettings;
    private readonly ILogger<OnlineTitleInfoService>? _logger;
    private readonly HttpClient? _httpClient;
    private readonly TitleDbCatalog _titleDb;

    public OnlineTitleInfoService(IAppSettings appSettings, ILoggerFactory? loggerFactory = null, HttpClient? httpClient = null, string? cacheDirectory = null)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _logger = loggerFactory?.CreateLogger<OnlineTitleInfoService>();
        _httpClient = httpClient;
        _titleDb = new TitleDbCatalog(cacheDirectory ?? Path.Combine(AppContext.BaseDirectory, "Cache", "TitleDB"), _logger);
    }


    public async Task<int> RefreshTitleDbAsync(string region, CancellationToken token)
    {
        using var ownedClient = _httpClient == null ? new HttpClient { Timeout = TimeSpan.FromMinutes(2) } : null;
        return await _titleDb.RefreshAsync(region, _httpClient ?? ownedClient!, token);
    }

    public async Task<IOnlineTitleInfo?> GetTitleInfoAsync(string titleId)
    {
        using var ownedClient = _httpClient == null ? new HttpClient { Timeout = TimeSpan.FromSeconds(_appSettings.TitleInfoProvider == TitleInfoProvider.GitHubTitleDb ? 60 : 15) } : null;
        try
        {
            var client = _httpClient ?? ownedClient!;
            if (_appSettings.TitleInfoProvider == TitleInfoProvider.GitHubTitleDb)
            {
                var region = _appSettings.TitleDbRegion;
                var local = await _titleDb.FindAsync(region, titleId, client);
                return local ?? (region == "US.en" ? null : await _titleDb.FindAsync("US.en", titleId, client));
            }
            var template = _appSettings.TitleInfoProvider == TitleInfoProvider.NLib ? _appSettings.NLibApiUrl : _appSettings.TitleInfoApiUrl;
            var language = "en";
            if (_appSettings.TitleInfoProvider == TitleInfoProvider.NLib)
            {
                try { language = CultureInfo.GetCultureInfo(_appSettings.AppLanguage).TwoLetterISOLanguageName; }
                catch (CultureNotFoundException) { language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName; }
            }
            if (language is not ("en" or "ja" or "es" or "de" or "fr" or "nl" or "pt" or "it" or "zh" or "ko" or "ru"))
                language = "en";
            var url = template.Replace("{TitleId}", Uri.EscapeDataString(titleId), StringComparison.OrdinalIgnoreCase)
                .Replace("{Language}", language, StringComparison.OrdinalIgnoreCase);
            using var document = await client.GetFromJsonAsync<JsonDocument>(new Uri(url));
            var title = document?.RootElement.Deserialize<OnlineTitleInfo>();
            if (_appSettings.TitleInfoProvider == TitleInfoProvider.NLib && document != null &&
                document.RootElement.TryGetProperty("icon", out var icon) && icon.ValueKind == JsonValueKind.String && title != null)
                title.IconUrl = icon.GetString()!;
            return string.IsNullOrWhiteSpace(title?.Name) ? null : title;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or ArgumentException or IOException or UnauthorizedAccessException)
        {
            _logger?.LogWarning("Online title lookup for {TitleId} failed: {Reason}. Using local title information when available.", titleId, ex.Message);
            return null;
        }
    }
}

public interface ITitleDbUpdater
{
    Task<int> RefreshTitleDbAsync(string region, CancellationToken token);
}
