using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.Nand;
using Emignatik.NxFileViewer.Settings;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.Nand;

public sealed class NandPluginManagerTest : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "nand-update-test-" + Guid.NewGuid().ToString("N"));
    private readonly Handler _handler = new();
    private readonly AppSettings _settings = new();
    private readonly Progress _progress = new();
    private NandPluginManager Manager(Action<string, CancellationToken>? check = null) => new(_settings, _root, new HttpClient(_handler), check ?? ((path, _) => Assert.True(File.Exists(path))));

    [Fact]
    public async Task DownloadsDependenciesActivatesAndRollsBackUsingRelativePaths()
    {
        var manager = Manager();
        await manager.UpdateAsync(_progress, CancellationToken.None);
        var first = manager.ExecutablePath;
        Assert.Contains("NxNandManager", manager.Status);
        Assert.DoesNotContain("nicoboss", manager.Status);
        Assert.True(File.Exists(Path.Combine(Path.GetDirectoryName(first)!, "dokan1.dll")));
        Assert.False(manager.CanRollback);
        _handler.Tag = "2";
        await manager.UpdateAsync(_progress, CancellationToken.None);
        Assert.NotEqual(first, manager.ExecutablePath);
        Assert.True(manager.CanRollback);
        manager.Rollback();
        Assert.Equal(first, manager.ExecutablePath);
        Assert.DoesNotContain(_root.Replace("\\", "\\\\"), File.ReadAllText(Path.Combine(_root, "active.json")));
    }

    [Theory]
    [InlineData("digest")]
    [InlineData("url")]
    [InlineData("compatibility")]
    [InlineData("traversal")]
    [InlineData("prerelease")]
    public async Task FailedUpdateKeepsActiveVersionAndDiscardsStaging(string failure)
    {
        var manager = Manager();
        await manager.UpdateAsync(_progress, CancellationToken.None);
        var active = manager.ExecutablePath;
        _handler.Tag = "2";
        _handler.Failure = failure;
        var updater = failure == "compatibility" ? Manager((_, _) => throw new InvalidDataException("bad CLI")) : manager;
        await Assert.ThrowsAnyAsync<Exception>(() => updater.UpdateAsync(_progress, CancellationToken.None));
        Assert.Equal(active, manager.ExecutablePath);
        Assert.Single(Directory.GetDirectories(_root));
    }

    [Fact]
    public async Task SameReleaseIsReusedAndCustomExecutableIsNeverUpdated()
    {
        var manager = Manager();
        await manager.UpdateAsync(_progress, CancellationToken.None);
        var count = _handler.Downloads;
        await manager.UpdateAsync(_progress, CancellationToken.None);
        Assert.Equal(count, _handler.Downloads);
        _settings.NandExecutablePath = manager.ExecutablePath;
        await Assert.ThrowsAsync<InvalidOperationException>(() => manager.UpdateAsync(_progress, CancellationToken.None));
        Assert.Equal(count, _handler.Downloads);
    }
    public void Dispose() { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
    private sealed class Progress : IProgressReporter
    {
        public void SetMode(bool value) { }
        public void SetText(string value) { }
        public void SetPercentage(double value) { }
    }
    private sealed class Handler : HttpMessageHandler
    {
        public string Tag = "1";
        public string Failure = "";
        public int Downloads;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using var data = new MemoryStream();
            using (var zip = new ZipArchive(data, ZipArchiveMode.Create, true))
            {
                var executable = zip.CreateEntry(Failure == "traversal" ? "../NxNandManager.exe" : "NxNandManager.exe");
                executable.LastWriteTime = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
                using (var writer = new StreamWriter(executable.Open())) writer.Write("exe " + Tag);
                var dll = zip.CreateEntry("dokan1.dll");
                dll.LastWriteTime = executable.LastWriteTime;
                using (var writer = new StreamWriter(dll.Open())) writer.Write("dll");
            }
            var bytes = data.ToArray();
            if (request.RequestUri!.Host == "api.github.com")
            {
                var json = JsonSerializer.Serialize(new { tag_name = Tag, draft = false, prerelease = Failure == "prerelease", assets = new[] { new {
                    name = $"NxNandManager.{Tag}.zip", digest = "sha256:" + (Failure == "digest" ? new string('0', 64) : Convert.ToHexString(SHA256.HashData(bytes))),
                    browser_download_url = Failure == "url" ? "https://example.com/package.zip" : $"https://github.com/THZoria/NxNandManager/releases/download/{Tag}/NxNandManager.{Tag}.zip"
                } } });
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });
            }
            Downloads++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(bytes) });
        }
    }
}
