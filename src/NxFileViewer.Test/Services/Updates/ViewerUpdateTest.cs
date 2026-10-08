using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Services.Updates;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.Updates;

public sealed class ViewerUpdateTest : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "ViewerUpdateTest-" + Guid.NewGuid());
    public ViewerUpdateTest() => Directory.CreateDirectory(_root);
    [Theory]
    [InlineData("x64", false)] [InlineData("x64", true)]
    [InlineData("x86", false)] [InlineData("x86", true)]
    public void KeepsInstalledDistributionForPreviewAssets(string architecture, bool selfContained)
    {
        var names = new[] { "x64", "x86", "self-contained_x64", "self-contained_x86" };
        var json = JsonSerializer.SerializeToElement(new { tag_name = "v4.0.0-beta.3", draft = false, prerelease = true,
            assets = names.Select(suffix => new { name = "NxFileViewer_v4.0.0-beta.3_" + suffix + ".zip", browser_download_url = Url, digest = "sha256:" + new string('1', 64) }) });
        var result = ViewerUpdateService.SelectRelease(json, new Version(4,0,0), architecture, true, currentTag: "4.0.0-beta.2", selfContained: selfContained)!;
        Assert.Equal("NxFileViewer_v4.0.0-beta.3_" + (selfContained ? "self-contained_" : "") + architecture + ".zip", result.AssetName);
    }
    [Fact]
    public void DoesNotSwitchDistributionWhenRuntimeAssetIsMissing()
    {
        Assert.Null(ViewerUpdateService.SelectRelease(Release(), new Version(3,0,4), "x64", skipMissingPackage: true, selfContained: true));
        Assert.Throws<InvalidDataException>(() => ViewerUpdateService.SelectRelease(Release(), new Version(3,0,4), "x64", selfContained: true));
    }
    private const string Url = "https://github.com/sandmaennchen5/fork_NxFileViewer/releases/download/v3.0.5/NxFileViewer_v3.0.5_x64.zip";
    private static JsonElement Release(string tag = "v3.0.5", bool draft = false, bool prerelease = false, string url = Url) =>
        JsonSerializer.SerializeToElement(new { tag_name = tag, draft, prerelease, assets = new[]
        {
            new { name = "NxFileViewer_v3.0.5_firmware-hashes.zip", browser_download_url = url, digest = "sha256:" + new string('0',64) },
            new { name = "NxFileViewer_v3.0.5_x64.zip", browser_download_url = url, digest = "sha256:" + new string('1',64) },
            new { name = "NxFileViewer_v3.0.5_x86.zip", browser_download_url = url.Replace("x64", "x86"), digest = "sha256:" + new string('2',64) }
        }});
    [Theory]
    [InlineData("x64")]
    [InlineData("x86")]
    public void ChoosesArchitectureAndIgnoresHashAddon(string architecture)
    {
        var release = ViewerUpdateService.SelectRelease(Release(), new Version(3,0,4,0), architecture)!;
        Assert.Equal(new Version(3,0,5), release.Version);
        Assert.EndsWith("_" + architecture + ".zip", release.AssetName);
    }
    [Theory]
    [InlineData("v3.0.4", false, false)]
    [InlineData("v3.0.3", false, false)]
    [InlineData("v3.0.5-beta", false, false)]
    [InlineData("v3.0.5", true, false)]
    [InlineData("v3.0.5", false, true)]
    public void DoesNotDowngradeOrInstallDraftsAndPrereleases(string tag, bool draft, bool prerelease) =>
        Assert.Null(ViewerUpdateService.SelectRelease(Release(tag, draft, prerelease), new Version(3,0,4), "x64"));
    [Fact]
    public void RejectsWrongRepositoryAndUnsupportedArchitecture()
    {
        Assert.Throws<InvalidDataException>(() => ViewerUpdateService.SelectRelease(Release(url: "https://github.com/other/repo/releases/download/x/a.zip"), new Version(3,0,4), "x64"));
        Assert.Throws<NotSupportedException>(() => ViewerUpdateService.SelectRelease(Release(), new Version(3,0,4), "arm64"));
    }
    [Theory] [InlineData("v3.0.5-beta.1")] [InlineData("v3.0.5")]
    public void OptInAcceptsPrereleasesAndPreservesTheirTag(string tag)
    {
        var release = ViewerUpdateService.SelectRelease(Release(tag, prerelease: true), new Version(3,0,4), "x64", true)!;
        Assert.True(release.IsPrerelease);
        Assert.Equal(tag, release.DisplayVersion);
        Assert.Equal(new Version(3,0,5), release.Version);
        Assert.Null(ViewerUpdateService.SelectRelease(Release(tag, draft: true, prerelease: true), new Version(3,0,4), "x64", true));
        Assert.Null(ViewerUpdateService.SelectRelease(Release(tag, prerelease: true), new Version(3,0,5), "x64", true));
    }
    [Fact] public void SupportsPrereleaseNamedAssetAndSkipsMissingArchitectureWhenListing()
    {
        var tagged = JsonDocument.Parse(Release("v3.0.5-rc.1", prerelease: true).GetRawText().Replace("NxFileViewer_v3.0.5_", "NxFileViewer_v3.0.5-rc.1_"));
        using (tagged)
            Assert.Equal("NxFileViewer_v3.0.5-rc.1_x64.zip", ViewerUpdateService.SelectRelease(tagged.RootElement, new Version(3,0,4), "x64", true)!.AssetName);
        var missing = JsonSerializer.SerializeToElement(new { tag_name = "v3.0.6-beta", draft = false, prerelease = true, assets = Array.Empty<object>() });
        Assert.Null(ViewerUpdateService.SelectRelease(missing, new Version(3,0,4), "x64", true, true));
    }
    [Theory] [InlineData(false)] [InlineData(true)]
    public async Task CheckUsesSelectedChannelAndSelectsNewestNumericVersion(bool includePrereleases)
    {
        using var beta = JsonDocument.Parse(Release("v3.0.5-beta", prerelease: true).GetRawText().Replace("3.0.5", "3.0.6"));
        using var client = new HttpClient(new Handler(request =>
        {
            Assert.Equal(!includePrereleases, request.RequestUri!.AbsolutePath.EndsWith("/latest"));
            var body = includePrereleases ? JsonSerializer.Serialize(new[] { Release(), beta.RootElement, Release("v3.0.9", draft: true) }) : Release().GetRawText();
            return new(HttpStatusCode.OK) { Content = new StringContent(body) };
        }));
        var release = await new ViewerUpdateService(client).CheckAsync(new Version(3,0,4), "x64", TestContext.Current.CancellationToken, includePrereleases);
        Assert.Equal(new Version(3,0,includePrereleases ? 6 : 5), release!.Version);
        Assert.Equal(includePrereleases, release.IsPrerelease);
    }
    [Fact] public async Task OptInPrefersStableAtSameNumericVersionAndReadsAdditionalPages()
    {
        var requests = 0;
        using var client = new HttpClient(new Handler(request =>
        {
            requests++;
            var releases = requests == 1 ? Enumerable.Repeat(Release(draft: true), 100).ToArray() :
                new[] { Release("v3.0.5-beta.2", prerelease: true), Release() };
            Assert.Contains("page=" + requests, request.RequestUri!.Query);
            return new(HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(releases)) };
        }));
        var result = await new ViewerUpdateService(client).CheckAsync(new Version(3,0,4), "x64", TestContext.Current.CancellationToken, true);
        Assert.Equal(2, requests);
        Assert.False(result!.IsPrerelease);
    }
    [Fact] public void PrereleaseOptInDefaultsOffAndIsPersisted()
    {
        var settings = new Emignatik.NxFileViewer.Settings.AppSettings();
        Assert.False(settings.IncludeViewerPrereleases);
        settings.IncludeViewerPrereleases = true;
        Assert.True(JsonSerializer.Deserialize<Emignatik.NxFileViewer.Settings.AppSettings>(JsonSerializer.Serialize(settings))!.IncludeViewerPrereleases);
        Assert.False(JsonSerializer.Deserialize<Emignatik.NxFileViewer.Settings.AppSettings>("{}")!.IncludeViewerPrereleases);
    }
    [Theory]
    [InlineData(null)]
    [InlineData("sha256:bad")]
    [InlineData("md5:abcd")]
    public void RequiresPublishedSha256(string? digest) => Assert.Throws<InvalidDataException>(() => ViewerUpdateService.ExpectedHash(digest));
    [Fact]
    public async Task ReleaseNotFoundIsNotAnError()
    {
        using var client = new HttpClient(new Handler(_ => new HttpResponseMessage(HttpStatusCode.NotFound)));
        Assert.Null(await new ViewerUpdateService(client).CheckAsync(new Version(3,0,4), "x64", TestContext.Current.CancellationToken));
    }
    [Fact]
    public async Task CorruptDownloadCannotChangeInstallation()
    {
        var target = Path.Combine(_root, "NxFileViewer.exe");
        File.WriteAllText(target, "previous");
        using var client = new HttpClient(new Handler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(Encoding.UTF8.GetBytes("bad archive")) }));
        var release = ViewerUpdateService.SelectRelease(Release(), new Version(3,0,4), "x64")!;
        await Assert.ThrowsAsync<InvalidDataException>(() => new ViewerUpdateService(client).PrepareAsync(release, _root, TestContext.Current.CancellationToken));
        Assert.Equal("previous", File.ReadAllText(target));
        Assert.Empty(Directory.GetFiles(Path.Combine(_root, "Updates"), "*", SearchOption.AllDirectories));
    }
    [Fact]
    public async Task VerifiedNativeExeIsStagedWithoutChangingInstalledExe()
    {
        var installed = Path.Combine(_root, "NxFileViewer.exe");
        File.WriteAllText(installed, "previous installation");
        var assembly = typeof(ViewerUpdateService).Assembly.GetName().Version!;
        var version = new Version(assembly.Major, assembly.Minor, assembly.Build);
        var architecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
        var name = $"NxFileViewer_v{version}_{architecture}.zip";
        var archive = Path.Combine(_root, "verified.zip");
        using (var zip = ZipFile.Open(archive, ZipArchiveMode.Create))
        using (var output = zip.CreateEntry("NxFileViewer.exe").Open())
        using (var input = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "NxFileViewer.exe"))) input.CopyTo(output);
        var bytes = File.ReadAllBytes(archive);
        var release = new ViewerRelease(version, name, new Uri(Url), "sha256:" + Convert.ToHexString(SHA256.HashData(bytes)));
        using var client = new HttpClient(new Handler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(bytes) }));
        var prepared = await new ViewerUpdateService(client).PrepareAsync(release, _root, TestContext.Current.CancellationToken);
        Assert.True(File.Exists(prepared.Executable));
        Assert.StartsWith(Path.Combine(_root, "Updates"), prepared.Directory);
        Assert.Equal("previous installation", File.ReadAllText(installed));
        Assert.False(File.Exists(Path.Combine(prepared.Directory, "release.zip")));
    }
    private string Zip(params string[] entries)
    {
        var archive = Path.Combine(_root, Guid.NewGuid() + ".zip");
        using var zip = ZipFile.Open(archive, ZipArchiveMode.Create);
        foreach (var entry in entries)
        {
            using var writer = new StreamWriter(zip.CreateEntry(entry).Open());
            writer.Write("new executable content");
        }
        return archive;
    }
    [Theory]
    [InlineData("NxFileViewer_v3.0.5_x64/NxFileViewer.exe")]
    [InlineData("NxFileViewer_v3.0.5_x64\\NxFileViewer.exe")]
    public void ExtractsOnlyExpectedExecutableToFixedPath(string entry)
    {
        var exe = ViewerUpdateService.ExtractExecutable(Zip(entry), _root,
            "NxFileViewer_v3.0.5_x64.zip", TestContext.Current.CancellationToken);
        Assert.Equal(Path.Combine(_root, "new.exe"), exe);
        Assert.Equal("new executable content", File.ReadAllText(exe));
    }
    [Theory]
    [InlineData("../prod.keys")]
    [InlineData("..\\prod.keys")]
    [InlineData("NxFileViewer_v3.0.5_x64\\..\\..\\NxFileViewer.exe")]
    [InlineData("NxFileViewer_v3.0.5_x64/../../NxFileViewer.exe")]
    [InlineData("other/NxFileViewer.exe")]
    public void RejectsUntrustedArchivePaths(string entry) =>
        Assert.Throws<InvalidDataException>(() => ViewerUpdateService.ExtractExecutable(Zip(entry), _root, "NxFileViewer_v3.0.5_x64.zip", TestContext.Current.CancellationToken));
    [Fact]
    public void RejectsArchivesContainingUserData() =>
        Assert.Throws<InvalidDataException>(() => ViewerUpdateService.ExtractExecutable(Zip("NxFileViewer.exe", "prod.keys"), _root,
            "NxFileViewer_v3.0.5_x64.zip", TestContext.Current.CancellationToken));
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task InstallerBacksUpExecutableAndPreservesUserDataOrRefusesBadHash(bool validHash)
    {
        var stage = Path.Combine(_root, "Updates", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(stage);
        var target = Path.Combine(_root, "NxFileViewer.exe");
        File.WriteAllText(target, "previous executable");
        File.WriteAllText(Path.Combine(_root, "prod.keys"), "private user data");
        Directory.CreateDirectory(Path.Combine(_root, "Plugins"));
        File.WriteAllText(Path.Combine(_root, "Plugins", "plugin.txt"), "plugin data");
        var newExe = Path.Combine(stage, "new.exe");
        File.WriteAllText(newExe, "new executable");
        var digest = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(newExe)));
        var plan = Path.Combine(stage, "plan.json");
        File.WriteAllText(plan, JsonSerializer.Serialize(new { Target = target, ProcessId = 999999999, ExecutableHash = validHash ? digest : new string('0',64) }));
        var script = Path.Combine(stage, "install.ps1");
        using (var input = typeof(ViewerUpdateService).Assembly.GetManifestResourceStream("ViewerUpdate.InstallUpdate.ps1")!)
        using (var output = File.Create(script)) input.CopyTo(output);
        var start = new ProcessStartInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "WindowsPowerShell", "v1.0", "powershell.exe"))
        { UseShellExecute = false, CreateNoWindow = true, RedirectStandardError = true, RedirectStandardOutput = true };
        foreach (var arg in new[] { "-NoProfile", "-NonInteractive", "-ExecutionPolicy", "Bypass", "-File", script, "-PlanPath", plan, "-NoRestart" }) start.ArgumentList.Add(arg);
        using var process = Process.Start(start)!;
        var stdout = process.StandardOutput.ReadToEndAsync(TestContext.Current.CancellationToken);
        var stderr = process.StandardError.ReadToEndAsync(TestContext.Current.CancellationToken);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(15));
        try { await process.WaitForExitAsync(timeout.Token); }
        catch { process.Kill(entireProcessTree: true); throw; }
        await Task.WhenAll(stdout, stderr);
        Assert.True(process.ExitCode == (validHash ? 0 : 1), await stderr +
            (File.Exists(Path.Combine(stage, "error.txt")) ? File.ReadAllText(Path.Combine(stage, "error.txt")) : ""));
        Assert.Equal(validHash ? "new executable" : "previous executable", File.ReadAllText(target));
        if (validHash) Assert.Equal("previous executable", File.ReadAllText(Path.Combine(stage, "previous.exe")));
        Assert.Equal("private user data", File.ReadAllText(Path.Combine(_root, "prod.keys")));
        Assert.Equal("plugin data", File.ReadAllText(Path.Combine(_root, "Plugins", "plugin.txt")));
    }
    public void Dispose() => Directory.Delete(_root, true);
    private sealed class Handler(Func<HttpRequestMessage,HttpResponseMessage> response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(response(request));
    }
}
