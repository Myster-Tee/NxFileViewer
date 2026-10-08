using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Services.KeysManagement;
using Emignatik.NxFileViewer.Services.OnlineServices;
using Emignatik.NxFileViewer.Settings;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.KeysManagement;

public sealed class KeyDownloadsTest : IDisposable
{
    private readonly string _root = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "KeyDownloads-" + Guid.NewGuid())).FullName;

    [Theory]
    [InlineData("192.168.1.23", "ftp://192.168.1.23:5000/sdmc:/switch/prod.keys")]
    [InlineData("::1", "ftp://[::1]:5000/sdmc:/switch/prod.keys")]
    public void ResolvesIpWithoutChangingPortOrRemotePath(string ip, string expected) =>
        Assert.Equal(expected, KeyDownloads.ResolveUrl("ftp://{IP}:5000/sdmc:/switch/prod.keys", ip));

    [Fact]
    public void MigratesExistingCommonFtpHostAndPersistsTemplates()
    {
        var settings = new AppSettings
        {
            ProdKeysDownloadUrl = "ftp://192.168.1.23:5000/sdmc:/switch/prod.keys",
            TitleKeysDownloadUrl = "ftp://192.168.1.23:5000/sdmc:/switch/title.keys"
        };
        KeyDownloads.MigrateLegacyHost(settings);
        Assert.Equal("192.168.1.23", settings.KeysDownloadHost);
        Assert.Contains("{IP}", settings.ProdKeysDownloadUrl);
        settings.KeysDownloadHost = "192.168.1.42";
        var saved = JsonSerializer.Deserialize<AppSettings>(JsonSerializer.Serialize(settings))!;
        Assert.Equal("ftp://192.168.1.42:5000/sdmc:/switch/title.keys", KeyDownloads.ResolveUrl(saved.TitleKeysDownloadUrl, saved.KeysDownloadHost));
    }

    [Fact]
    public void KeepsDifferentLegacyHostsAndRejectsInvalidSubstitution()
    {
        var settings = new AppSettings { ProdKeysDownloadUrl = "ftp://one:5000/prod.keys", TitleKeysDownloadUrl = "ftp://two:5000/title.keys" };
        KeyDownloads.MigrateLegacyHost(settings);
        Assert.Equal("ftp://one:5000/prod.keys", settings.ProdKeysDownloadUrl);
        Assert.Throws<ArgumentException>(() => KeyDownloads.ResolveUrl("ftp://{IP}:5000/prod.keys", "host/path"));
        Assert.Equal("https://example.test/prod.keys", KeyDownloads.ResolveUrl("https://example.test/prod.keys", ""));
    }

    [Fact]
    public async Task UsesCustomDestinationAndCreatesMissingParent()
    {
        var program = Path.Combine(_root, "prod.keys");
        File.WriteAllText(program, "keep program copy");
        var custom = Path.Combine(_root, "custom", "prod.keys");
        Assert.Equal(program, KeyDownloads.Destination("", program));
        Assert.Equal(custom, KeyDownloads.Destination(custom, program));
        await KeyDownloads.DownloadAsync(new FakeDownloader((path, _) => File.WriteAllText(path, "new synthetic keys")),
            "ftp://example.test/prod.keys", custom, TestContext.Current.CancellationToken);
        Assert.Equal("new synthetic keys", File.ReadAllText(custom));
        Assert.Equal("keep program copy", File.ReadAllText(program));
        Assert.Single(Directory.GetFiles(Path.GetDirectoryName(custom)!));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FailureOrCancellationKeepsExistingKeysAndCleansPartialDownload(bool cancel)
    {
        var destination = Path.Combine(_root, "prod.keys");
        File.WriteAllText(destination, "previous synthetic keys");
        using var cancellation = new CancellationTokenSource();
        var downloader = new FakeDownloader((path, _) =>
        {
            File.WriteAllText(path, "partial or uncommitted download");
            if (cancel) cancellation.Cancel();
            else throw new IOException("simulated failure");
        });
        if (cancel)
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => KeyDownloads.DownloadAsync(downloader, "ftp://example.test/prod.keys", destination, cancellation.Token));
        else
            await Assert.ThrowsAsync<IOException>(() => KeyDownloads.DownloadAsync(downloader, "ftp://example.test/prod.keys", destination, cancellation.Token));
        Assert.Equal("previous synthetic keys", File.ReadAllText(destination));
        Assert.Single(Directory.GetFiles(_root));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ApprovalComparesStagedDownloadBeforeReplacingExistingKeys(bool approve)
    {
        var destination = Path.Combine(_root, "prod.keys");
        File.WriteAllText(destination, "previous synthetic keys");
        var called = false;
        var operation = KeyDownloads.DownloadAsync(new FakeDownloader((path, _) => File.WriteAllText(path, "incoming synthetic keys")),
            "ftp://example.test/prod.keys", destination, TestContext.Current.CancellationToken, (source, target) =>
            {
                called = true;
                Assert.Equal("previous synthetic keys", File.ReadAllText(target));
                Assert.Equal("incoming synthetic keys", File.ReadAllText(source));
                return approve;
            });
        if (approve) await operation;
        else await Assert.ThrowsAnyAsync<OperationCanceledException>(() => operation);
        Assert.True(called);
        Assert.Equal(approve ? "incoming synthetic keys" : "previous synthetic keys", File.ReadAllText(destination));
        Assert.Single(Directory.GetFiles(_root));
    }

    private sealed class FakeDownloader(Action<string, CancellationToken> action) : IHttpDownloader
    {
        public Task DownloadFileAsync(string url, string destFilePath, CancellationToken cancellationToken)
        {
            action(destFilePath, cancellationToken);
            return Task.CompletedTask;
        }
    }
    public void Dispose() => Directory.Delete(_root, true);
}
