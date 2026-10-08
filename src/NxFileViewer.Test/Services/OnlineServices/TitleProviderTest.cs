using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Services.OnlineServices;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.OnlineServices;

public class TitleProviderTest : IDisposable
{
    private const string TitleId = "010019C023004000";
    private const string Catalog = "{\"70010001234567\":{\"id\":\"010019C023004000\",\"name\":\"Until Then\",\"publisher\":\"Test\"}}";
    private readonly string _cacheDirectory = Path.Combine(Path.GetTempPath(), "TitleDbTests." + Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task DefaultTitleDbCacheIsInsideProgramDirectory()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Cache", "TitleDB", "XY.xy.json");
        var previous = File.Exists(path) ? File.ReadAllBytes(path) : null;
        try
        {
            using var handler = new Handler(_ => Ok(Catalog));
            using var client = new HttpClient(handler);
            var service = new OnlineTitleInfoService(new AppSettings {
                TitleInfoProvider = TitleInfoProvider.GitHubTitleDb, TitleDbRegion = "XY.xy"
            }, httpClient: client);
            Assert.NotNull(await service.GetTitleInfoAsync(TitleId));
            Assert.Equal(Catalog, File.ReadAllText(path));
        }
        finally
        {
            if (previous == null) File.Delete(path);
            else File.WriteAllBytes(path, previous);
        }
    }

    [Fact]
    public async Task NLibUsesConfiguredEndpointLanguageAndIconField()
    {
        var settings = new AppSettings { TitleInfoProvider = TitleInfoProvider.NLib, AppLanguage = "de-DE" };
        using var handler = new Handler(request =>
        {
            Assert.Equal("api.nlib.cc", request.RequestUri!.Host);
            Assert.Equal("/nx/" + TitleId, request.RequestUri.AbsolutePath);
            Assert.Equal("?lang=de", request.RequestUri.Query);
            return Ok("{\"name\":\"Until Then\",\"icon\":\"https://example.test/icon\"}");
        });
        using var client = new HttpClient(handler);
        var result = await new OnlineTitleInfoService(settings, httpClient: client).GetTitleInfoAsync(TitleId);
        Assert.Equal("Until Then", result!.Name);
        Assert.Equal("https://example.test/icon", result.IconUrl);
    }

    [Fact]
    public async Task GitHubIndexesByTitleIdAndPersistsCatalogForOfflineUse()
    {
        var settings = new AppSettings { TitleInfoProvider = TitleInfoProvider.GitHubTitleDb };
        using var handler = new Handler(request =>
        {
            Assert.EndsWith("/DE.de.json", request.RequestUri!.AbsolutePath);
            return Ok(Catalog);
        });
        using var client = new HttpClient(handler);
        var service = new OnlineTitleInfoService(settings, httpClient: client, cacheDirectory: _cacheDirectory);
        Assert.Equal("Until Then", (await service.GetTitleInfoAsync(TitleId.ToLowerInvariant()))!.Name);
        Assert.Equal("Until Then", (await service.GetTitleInfoAsync(TitleId))!.Name);
        Assert.Equal(1, handler.Calls);
        using var offlineHandler = new Handler(_ => throw new HttpRequestException());
        using var offlineClient = new HttpClient(offlineHandler);
        var restartedService = new OnlineTitleInfoService(settings, httpClient: offlineClient, cacheDirectory: _cacheDirectory);
        Assert.Equal("Until Then", (await restartedService.GetTitleInfoAsync(TitleId))!.Name);
        Assert.Equal(0, offlineHandler.Calls);
        File.SetLastWriteTimeUtc(Path.Combine(_cacheDirectory, "DE.de.json"), DateTime.UtcNow.AddDays(-2));
        var staleService = new OnlineTitleInfoService(settings, httpClient: offlineClient, cacheDirectory: _cacheDirectory);
        Assert.Equal("Until Then", (await staleService.GetTitleInfoAsync(TitleId))!.Name);
        Assert.Equal(1, offlineHandler.Calls);
    }

    [Fact]
    public async Task MissingRegionalTitleFallsBackToEnglishCatalog()
    {
        var settings = new AppSettings { TitleInfoProvider = TitleInfoProvider.GitHubTitleDb };
        using var handler = new Handler(request => Ok(request.RequestUri!.AbsolutePath.EndsWith("DE.de.json")
            ? Catalog.Replace(TitleId, "0100000000000000") : Catalog));
        using var client = new HttpClient(handler);
        var service = new OnlineTitleInfoService(settings, httpClient: client, cacheDirectory: _cacheDirectory);
        Assert.Equal("Until Then", (await service.GetTitleInfoAsync(TitleId))!.Name);
        Assert.Equal(2, handler.Calls);
    }

    [Fact]
    public async Task InvalidUpdateDoesNotReplaceGoodStaleCatalog()
    {
        Directory.CreateDirectory(_cacheDirectory);
        var path = Path.Combine(_cacheDirectory, "DE.de.json");
        await File.WriteAllTextAsync(path, Catalog, TestContext.Current.CancellationToken);
        File.SetLastWriteTimeUtc(path, DateTime.UtcNow.AddDays(-2));
        using var handler = new Handler(_ => Ok("{}"));
        using var client = new HttpClient(handler);
        var service = new OnlineTitleInfoService(new AppSettings { TitleInfoProvider = TitleInfoProvider.GitHubTitleDb }, httpClient: client, cacheDirectory: _cacheDirectory);
        Assert.Equal("Until Then", (await service.GetTitleInfoAsync(TitleId))!.Name);
        Assert.Equal(Catalog, await File.ReadAllTextAsync(path, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ChangingProviderDoesNotReusePreviousProviderTitle()
    {
        var settings = new AppSettings();
        using var handler = new Handler(request => Ok(request.RequestUri!.Host == "api.nlib.cc" ? "{\"name\":\"NLib name\"}" : "{\"name\":\"Tinfoil name\"}"));
        using var client = new HttpClient(handler);
        var cached = new CachedOnlineTitleInfoService(new OnlineTitleInfoService(settings, httpClient: client), settings);
        Assert.Equal("Tinfoil name", (await cached.GetTitleInfoAsync(TitleId))!.Name);
        settings.TitleInfoProvider = TitleInfoProvider.NLib;
        Assert.Equal("NLib name", (await cached.GetTitleInfoAsync(TitleId))!.Name);
        Assert.Equal(2, handler.Calls);
    }

    [Fact]
    public void SettingsPersistProviderAndKeepExistingSettingsCompatible()
    {
        Assert.Equal(TitleInfoProvider.Tinfoil, JsonSerializer.Deserialize<AppSettings>("{}")!.TitleInfoProvider);
        var settings = new AppSettings { TitleInfoProvider = TitleInfoProvider.NLib, NLibApiUrl = "https://example.test/{TitleId}", TitleDbRegion = "US.en" };
        var restored = JsonSerializer.Deserialize<AppSettings>(JsonSerializer.Serialize(settings))!;
        Assert.Equal(settings.TitleInfoProvider, restored.TitleInfoProvider);
        Assert.Equal(settings.NLibApiUrl, restored.NLibApiUrl);
        Assert.Equal(settings.TitleDbRegion, restored.TitleDbRegion);
        IAppSettings copied = new AppSettings();
        new ShallowCopier().Copy<IAppSettings>(settings, copied);
        Assert.Equal(settings.TitleInfoProvider, copied.TitleInfoProvider);
        Assert.Equal(settings.NLibApiUrl, copied.NLibApiUrl);
        Assert.Equal(settings.TitleDbRegion, copied.TitleDbRegion);
    }

    [Fact]
    public void ServicesResolveUsingApplicationRegistrations()
    {
        using var provider = new ServiceCollection()
            .AddSingleton<IAppSettings>(new AppSettings())
            .AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance)
            .AddSingleton<IOnlineTitleInfoService, OnlineTitleInfoService>()
            .AddSingleton<ICachedOnlineTitleInfoService, CachedOnlineTitleInfoService>()
            .BuildServiceProvider();
        Assert.NotNull(provider.GetRequiredService<ICachedOnlineTitleInfoService>());
    }

    [Fact]
    public async Task ManualTitleDbRefreshBypassesFreshCacheAndClearsTitleResponses()
    {
        var json = Catalog;
        using var client = new HttpClient(new Handler(_ => Ok(json)));
        var settings = new AppSettings { TitleInfoProvider = TitleInfoProvider.GitHubTitleDb, TitleDbRegion = "US.en" };
        var online = new OnlineTitleInfoService(settings, httpClient: client, cacheDirectory: _cacheDirectory);
        var cache = new CachedOnlineTitleInfoService(online, settings);
        Assert.Equal("Until Then", (await cache.GetTitleInfoAsync(TitleId))!.Name);
        json = Catalog.Replace("Until Then", "Updated title");
        Assert.Equal(1, await online.RefreshTitleDbAsync("US.en", TestContext.Current.CancellationToken));
        cache.ClearCache();
        Assert.Equal("Updated title", (await cache.GetTitleInfoAsync(TitleId))!.Name);
        Assert.Contains("Updated title", File.ReadAllText(Path.Combine(_cacheDirectory, "US.en.json")));
    }

    [Fact]
    public async Task FailedManualRefreshPreservesUsableCatalog()
    {
        var json = Catalog;
        using var client = new HttpClient(new Handler(_ => Ok(json)));
        var online = new OnlineTitleInfoService(new AppSettings { TitleInfoProvider = TitleInfoProvider.GitHubTitleDb, TitleDbRegion = "US.en" },
            httpClient: client, cacheDirectory: _cacheDirectory);
        Assert.Equal("Until Then", (await online.GetTitleInfoAsync(TitleId))!.Name);
        var before = File.ReadAllText(Path.Combine(_cacheDirectory, "US.en.json"));
        json = "{}";
        await Assert.ThrowsAsync<JsonException>(() => online.RefreshTitleDbAsync("US.en", TestContext.Current.CancellationToken));
        Assert.Equal(before, File.ReadAllText(Path.Combine(_cacheDirectory, "US.en.json")));
        Assert.Equal("Until Then", (await online.GetTitleInfoAsync(TitleId))!.Name);
    }

    private static HttpResponseMessage Ok(string json) => new(HttpStatusCode.OK) { Content = new StringContent(json) };
    private sealed class Handler(Func<HttpRequestMessage, HttpResponseMessage> response) : HttpMessageHandler
    {
        public int Calls { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(response(request));
        }
    }
    public void Dispose()
    {
        if (Directory.Exists(_cacheDirectory)) Directory.Delete(_cacheDirectory, true);
    }
}
