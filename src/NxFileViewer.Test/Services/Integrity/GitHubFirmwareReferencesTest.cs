using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Services.Integrity;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.Integrity;

public class GitHubFirmwareReferencesTest
{
    [Fact] public async Task LoadsFreshReferencesForEachCheckWithoutDiskCache()
    {
        var handler = new Handler();
        using var client = new HttpClient(handler);
        var first = await GitHubFirmwareReferences.LoadAsync(client, TestContext.Current.CancellationToken);
        var second = await GitHubFirmwareReferences.LoadAsync(client, TestContext.Current.CancellationToken);
        Assert.NotSame(first, second);
        Assert.Equal(4, handler.Requests);
    }
    [Theory] [InlineData("changed")] [InlineData("invalid")] [InlineData("offline")]
    public async Task InvalidOrUnavailableReferencesDoNotProduceVerifier(string failure)
    {
        using var client = new HttpClient(new Handler { Failure = failure });
        await Assert.ThrowsAnyAsync<Exception>(() => GitHubFirmwareReferences.LoadAsync(client, TestContext.Current.CancellationToken));
    }
    [Fact] public async Task CancellationIsNotSwallowed()
    {
        using var client = new HttpClient(new Handler());
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => GitHubFirmwareReferences.LoadAsync(client, new CancellationToken(true)));
    }
    [Theory] [InlineData(false)] [InlineData(true)]
    public void GameFolderNeedsNeitherReferencesNorNetwork(bool hasGame)
    {
        var directory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "NxBatch-" + Guid.NewGuid())).FullName;
        try
        {
            if (hasGame) File.WriteAllBytes(Path.Combine(directory, "game.nsp"), new byte[] { 0 });
            var handler = new Handler { Failure = "offline" };
            using var client = new HttpClient(handler);
            var loader = new Loader();
            var runnable = CreateRunner(loader, Path.Combine(directory, "missing"), client);
            var results = runnable.Setup(directory, false).Run(new Progress(), TestContext.Current.CancellationToken);
            Assert.Equal(hasGame ? 1 : 0, results.Count);
            Assert.Equal(hasGame ? 1 : 0, loader.Calls);
            Assert.Equal(0, handler.Requests);
        }
        finally { Directory.Delete(directory, true); }
    }
    [Theory] [InlineData(false)] [InlineData(true)]
    public void FirmwareWithoutLocalReferencesUsesOnlineOrReportsIndividualError(bool offline)
    {
        var directory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "NxBatch-" + Guid.NewGuid())).FullName;
        try
        {
            File.WriteAllBytes(Path.Combine(directory, "new.nca"), new byte[] { 0 });
            File.WriteAllBytes(Path.Combine(directory, "game.nsp"), new byte[] { 0 });
            var handler = new Handler { Failure = offline ? "offline" : null };
            using var client = new HttpClient(handler);
            var loader = new Loader();
            var results = CreateRunner(loader, Path.Combine(directory, "missing"), client)
                .Setup(directory, false).Run(new Progress(), TestContext.Current.CancellationToken);
            Assert.Equal(2, results.Count);
            Assert.Equal(1, loader.Calls);
            Assert.True(handler.Requests > 0);
            var firmware = Assert.Single(results, r => r.IsFirmware);
            if (!offline) Assert.Equal("new", firmware.Structure);
            else Assert.Contains(Emignatik.NxFileViewer.Localization.LocalizationManager.Instance.Current.Keys.Firmware_NoReferences, firmware.Error!);
        }
        finally { Directory.Delete(directory, true); }
    }
    private static Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl.VerifyDirectoryIntegrityRunnable CreateRunner(Loader loader, string refs, HttpClient client) =>
        new(loader, null!, new Emignatik.NxFileViewer.Settings.AppSettings(),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl.VerifyDirectoryIntegrityRunnable>.Instance, refs, client);
    private sealed class Loader : Emignatik.NxFileViewer.FileLoading.IFileLoader
    {
        public int Calls;
        public Emignatik.NxFileViewer.Models.NxFile Load(string path) { Calls++; throw new InvalidDataException("synthetic package"); }
    }
    private sealed class Progress : Emignatik.NxFileViewer.Services.BackgroundTask.IProgressReporter
    {
        public void SetMode(bool value) { }
        public void SetText(string value) { }
        public void SetPercentage(double value) { }
    }
#if !DEBUG
    [Fact] public void ReleaseStackTracesUseNeutralBuildPaths()
    {
        var error = Assert.Throws<InvalidDataException>(() => new FirmwareIntegrityVerifier(Array.Empty<string>()));
        var stack = error.StackTrace!.Replace('\\', '/');
        Assert.Contains("/_/NxFileViewer/src/NxFileViewer/Services/Integrity/FirmwareIntegrityVerifier.cs:line", stack);
        Assert.DoesNotContain(":/Users/", stack, StringComparison.OrdinalIgnoreCase);
    }
#endif
    private sealed class Handler : HttpMessageHandler
    {
        public string? Failure { get; init; }
        public int Requests;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests++;
            Assert.True(request.Headers.CacheControl!.NoCache);
            if (Failure == "offline") return Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
            var manifest = Failure == "invalid" ? "{}" : JsonSerializer.Serialize(new { name = "new", file_count = 1,
                files = new System.Collections.Generic.Dictionary<string, object> { ["new.nca"] = new { size = 1, sha256 = new string('a', 64) } } });
            var bytes = Encoding.UTF8.GetBytes(manifest);
            var prefix = Encoding.ASCII.GetBytes("blob " + bytes.Length + "\0");
            var data = new byte[prefix.Length + bytes.Length];
            prefix.CopyTo(data, 0); bytes.CopyTo(data, prefix.Length);
            var sha = Convert.ToHexString(SHA1.HashData(data));
            var listing = JsonSerializer.Serialize(new[] { new { name = "new.json", type = "file", sha } });
            var text = request.RequestUri!.Host == "api.github.com" ? listing : Failure == "changed" ? manifest + " " : manifest;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(text) });
        }
    }
}
