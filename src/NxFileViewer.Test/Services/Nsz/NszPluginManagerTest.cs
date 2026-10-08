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
using Emignatik.NxFileViewer.Services.Nsz;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.Nsz;

public sealed class NszPluginManagerTest : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "NszPluginTest-" + Guid.NewGuid());
    private readonly Handler _handler = new();
    private readonly HttpClient _client;
    private readonly AppSettings _settings = new();
    private readonly Reporter _reporter = new();
    private readonly NszPluginManager _manager;
    private bool _incompatible, _runtimeFailure, _guiIncompatible;
    private int _checks;
    public NszPluginManagerTest()
    {
        _client = new HttpClient(_handler);
        _manager = new(_settings, NullLogger<NszPluginManager>.Instance, _root, _client, (path, token) =>
        {
            token.ThrowIfCancellationRequested();
            Assert.True(File.Exists(path));
            _checks++;
            if (_runtimeFailure && Path.GetFileName(path).StartsWith("nsz-cli-")) throw new NszRuntimeStartupException("Python DLL failed");
            if (_guiIncompatible && Path.GetFileName(path).StartsWith("nsz-gui-")) throw new InvalidDataException("GUI incompatible");
            if (_incompatible) throw new InvalidDataException("Incompatible CLI");
        });
    }
    private Task Update() => _manager.UpdateAsync(_reporter, TestContext.Current.CancellationToken);

    [Fact]
    public async Task VerifiedReleaseIsActivatedAndPreviousVersionCanBeRestored()
    {
        await Update();
        var first = _manager.ExecutablePath;
        Assert.Contains("1.0.0", _manager.Status);
        Assert.Contains(Emignatik.NxFileViewer.Localization.LocalizationManager.Instance.Current.Keys.Nsz_Installed, _manager.Status);
        Assert.Contains(_manager.ExecutablePath, _manager.Status);
        Assert.Equal(_handler.Content, File.ReadAllBytes(first));
        Assert.False(_manager.CanRollback);
        _handler.Tag = "2.0.0";
        await Update();
        Assert.NotEqual(first, _manager.ExecutablePath);
        Assert.True(File.Exists(first));
        Assert.True(_manager.CanRollback);
        Assert.Equal(2, _checks);
        _manager.Rollback();
        Assert.Equal(first, _manager.ExecutablePath);
        Assert.Contains("1.0.0", _manager.Status);
        Assert.Contains(Emignatik.NxFileViewer.Localization.LocalizationManager.Instance.Current.Keys.Nsz_Installed, _manager.Status);
        Assert.Contains(_manager.ExecutablePath, _manager.Status);
    }

    [Fact]
    public async Task BadDigestDoesNotReplaceActiveVersion()
    {
        await Update();
        var first = _manager.ExecutablePath;
        _handler.Tag = "2.0.0"; _handler.BadDigest = true;
        await Assert.ThrowsAsync<InvalidDataException>(Update);
        Assert.Equal(first, _manager.ExecutablePath);
        Assert.Single(Directory.GetDirectories(_root, "version-*"));
        Assert.Equal(1, _checks);
    }

    [Fact]
    public async Task IncompatibleReleaseDoesNotReplaceActiveVersion()
    {
        await Update();
        var first = _manager.ExecutablePath;
        _handler.Tag = "2.0.0"; _incompatible = true;
        await Assert.ThrowsAsync<InvalidDataException>(Update);
        Assert.Equal(first, _manager.ExecutablePath);
        Assert.Single(Directory.GetDirectories(_root, "version-*"));
    }

    [Fact]
    public async Task PrereleaseIsNotDownloaded()
    {
        _handler.Prerelease = true;
        await Assert.ThrowsAsync<InvalidDataException>(Update);
        Assert.Equal(0, _handler.Downloads);
        Assert.False(Directory.Exists(_root));
    }

    [Fact]
    public async Task ExistingCorrectVersionDoesNotDownloadAgain()
    {
        await Update();
        await Update();
        Assert.Equal(1, _handler.Downloads);
        Assert.Equal(1, _checks);
        Assert.False(_reporter.Indeterminate);
    }

    [Fact]
    public async Task OfflineUpdateUsesPreviouslyCheckedVersion()
    {
        await Update();
        var first = _manager.ExecutablePath;
        _handler.Offline = true;
        _manager.Prepare(_reporter, TestContext.Current.CancellationToken);
        Assert.Equal(first, _manager.ExecutablePath);
        Assert.Equal(2, _checks);
        Assert.False(_reporter.Indeterminate);
    }

    [Fact]
    public async Task CustomExecutableIsNotAutomaticallyReplaced()
    {
        await Update();
        _settings.NszExecutablePath = _manager.ExecutablePath;
        _handler.Offline = true;
        _manager.Prepare(_reporter, TestContext.Current.CancellationToken);
        Assert.Equal(1, _handler.MetadataRequests);
        Assert.Equal(2, _checks);
    }

    [Fact]
    public async Task PythonStartupFailureActivatesVerifiedGuiInCliModeWithoutRepeatedDownloads()
    {
        _runtimeFailure = true;
        await Update();
        Assert.EndsWith("nsz-gui-windows-x64.exe", _manager.ExecutablePath);
        Assert.Equal(_handler.GuiExecutable, File.ReadAllBytes(_manager.ExecutablePath));
        Assert.Equal(2, _handler.Downloads);
        await Update();
        Assert.Equal(2, _handler.Downloads);
        Assert.Equal(2, _checks);
    }
    [Theory] [InlineData(true)] [InlineData(false)]
    public async Task FailedGuiFallbackKeepsPreviouslyActiveVersion(bool badDigest)
    {
        await Update();
        var original = _manager.ExecutablePath;
        _handler.Tag = "2.0.0";
        _runtimeFailure = true;
        _handler.BadGuiDigest = badDigest;
        _guiIncompatible = !badDigest;
        await Assert.ThrowsAsync<InvalidDataException>(Update);
        Assert.Equal(original, _manager.ExecutablePath);
        Assert.Single(Directory.GetDirectories(_root, "version-*"));
    }

    [Fact]
    public async Task FailedStartupRetainsVerifiedDownloadsForRetry()
    {
        _runtimeFailure = true;
        _guiIncompatible = true;
        await Assert.ThrowsAsync<InvalidDataException>(Update);
        Assert.Equal(2, _handler.Downloads);
        Assert.False(File.Exists(_manager.ExecutablePath));
        Assert.Equal(2, Directory.GetFiles(Path.Combine(_root, "Downloads"), "*.download").Length);
        _guiIncompatible = false;
        await Update();
        Assert.Equal(2, _handler.Downloads);
        Assert.True(File.Exists(_manager.ExecutablePath));
    }
    [Fact]
    public void DefaultPluginDirectoryIsNextToExecutable() => Assert.Equal(
        Path.Combine(AppContext.BaseDirectory, "Plugins", "NSZ"),
        new NszPluginManager(new AppSettings(), NullLogger<NszPluginManager>.Instance).RootDirectory);

    [Fact]
    public async Task InstallationUsesRelativePathsAndSurvivesMovingApplicationDirectory()
    {
        await Update();
        Assert.DoesNotContain(_root.Replace("\\", "\\\\"), File.ReadAllText(Path.Combine(_root, "active.json")));
        var movedRoot = _root + "-moved";
        Directory.Move(_root, movedRoot);
        try
        {
            var moved = new NszPluginManager(_settings, NullLogger<NszPluginManager>.Instance, movedRoot);
            Assert.True(File.Exists(moved.ExecutablePath));
            Assert.StartsWith(movedRoot, moved.ExecutablePath);
        }
        finally { Directory.Move(movedRoot, _root); }
    }

    [Fact]
    public void MissingLocalInstallationDoesNotImportOtherDirectories()
    {
        var empty = new NszPluginManager(_settings, NullLogger<NszPluginManager>.Instance, Path.Combine(_root, "empty"));
        Assert.Equal("", empty.ExecutablePath);
        Assert.False(Directory.Exists(empty.RootDirectory));
    }
    [Fact]
    public void ManagedStateCannotPointOutsideProgramPluginDirectory()
    {
        Directory.CreateDirectory(_root);
        File.WriteAllText(Path.Combine(_root, "active.json"), JsonSerializer.Serialize(new {
            Executable = Path.Combine(Path.GetTempPath(), "outside.exe"), Version = "test"
        }));
        Assert.Throws<InvalidDataException>(() => _manager.ExecutablePath);
    }

    public void Dispose()
    {
        _client.Dispose();
        if (Directory.Exists(_root)) Directory.Delete(_root, recursive: true);
    }

    private sealed class Handler : HttpMessageHandler
    {
        public string Tag = "1.0.0";
        public byte[] Content = new byte[] { 1, 2, 3, 4 };
        public bool BadDigest, Prerelease, Offline, BadGuiDigest;
        public readonly byte[] GuiExecutable = new byte[] { 5, 6, 7 };
        private byte[] GuiArchive
        {
            get
            {
                using var stream = new MemoryStream();
                using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    var entry = zip.CreateEntry("nsz-gui-windows-x64.exe");
                    entry.LastWriteTime = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
                    using var file = entry.Open(); file.Write(GuiExecutable);
                }
                return stream.ToArray();
            }
        }
        public int Downloads, MetadataRequests;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Offline) throw new HttpRequestException("Offline");
            if (request.RequestUri!.Host == "api.github.com")
            {
                MetadataRequests++;
                var digest = "sha256:" + (BadDigest ? new string('0', 64) : Convert.ToHexString(SHA256.HashData(Content)));
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(new
                {
                    tag_name = Tag, draft = false, prerelease = Prerelease,
                    assets = new[]
                    {
                        new { name = "nsz-cli-windows-x64.exe", digest, browser_download_url = "https://github.com/nicoboss/nsz/releases/download/test/nsz-cli-windows-x64.exe" },
                        new { name = "nsz-cli-windows-arm64.exe", digest, browser_download_url = "https://github.com/nicoboss/nsz/releases/download/test/nsz-cli-windows-arm64.exe" },
                        new { name = "nsz-gui-windows-x64.zip", digest = "sha256:" + (BadGuiDigest ? new string('0', 64) : Convert.ToHexString(SHA256.HashData(GuiArchive))), browser_download_url = "https://github.com/nicoboss/nsz/releases/download/test/nsz-gui-windows-x64.zip" }
                    }
                })) });
            }
            Downloads++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(request.RequestUri.AbsolutePath.EndsWith(".zip") ? GuiArchive : Content) });
        }
    }
    private sealed class Reporter : IProgressReporter
    {
        public bool Indeterminate;
        public void SetMode(bool isIndeterminate) => Indeterminate = isIndeterminate;
        public void SetText(string text) { }
        public void SetPercentage(double value) { }
    }
}
