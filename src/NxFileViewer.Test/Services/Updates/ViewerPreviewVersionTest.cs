using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Services.Updates;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.Updates;

public sealed class ViewerPreviewVersionTest
{
    [Theory]
    [InlineData("4.0.0-beta.2", "4.0.0-beta.1", 1)]
    [InlineData("4.0.0-beta.10", "4.0.0-beta.2", 1)]
    [InlineData("4.0.0-rc.1", "4.0.0-beta.10", 1)]
    [InlineData("4.0.0", "4.0.0-rc.2", 1)]
    [InlineData("4.0.0-beta.1", "4.0.0", -1)]
    [InlineData("4.0.1-beta.1", "4.0.0", 1)]
    [InlineData("v4.0.0-beta.2+build.123", "4.0.0-beta.2+other", 0)]
    [InlineData("4.0.0-beta.2.1", "4.0.0-beta.2", 1)]
    [InlineData("4.0.0-beta.999999999999999999999", "4.0.0-beta.10", 1)]
    public void ComparesSemanticPrecedence(string left, string right, int expected)
    {
        Assert.True(ViewerVersion.TryParse(left, out var a));
        Assert.True(ViewerVersion.TryParse(right, out var b));
        Assert.Equal(expected, Math.Sign(a.CompareTo(b)));
    }

    [Theory]
    [InlineData("4.0.0-beta.01")]
    [InlineData("4.0.0-beta..2")]
    [InlineData("4.0.0-beta/2")]
    [InlineData("4.0.0-beta.2\n")]
    public void RejectsMalformedPreviewVersions(string text) =>
        Assert.False(ViewerVersion.TryParse(text, out _));

    private static JsonElement Release(string tag) => JsonSerializer.SerializeToElement(new
    {
        tag_name = tag, draft = false, prerelease = tag.Contains('-'),
        assets = new[] { new
        {
            name = $"NxFileViewer_v{tag}_x64.zip",
            browser_download_url = $"https://github.com/{ViewerUpdateService.Repository}/releases/download/v{tag}/app.zip",
            digest = "sha256:" + new string('0', 64)
        } }
    });

    [Fact]
    public void OffersOnlyNewerPreviewsAndStableWithoutDowngrading()
    {
        var numeric = new Version(4, 0, 0);
        Assert.NotNull(ViewerUpdateService.SelectRelease(Release("4.0.0-beta.2"), numeric, "x64", true, currentTag: "4.0.0-beta.1"));
        Assert.Null(ViewerUpdateService.SelectRelease(Release("4.0.0-beta.1"), numeric, "x64", true, currentTag: "4.0.0-beta.2"));
        Assert.Null(ViewerUpdateService.SelectRelease(Release("4.0.0-beta.2"), numeric, "x64", true, currentTag: "4.0.0-beta.2+commit"));
        Assert.Null(ViewerUpdateService.SelectRelease(Release("4.0.0-beta.3"), numeric, "x64", false, currentTag: "4.0.0-beta.2"));
        Assert.NotNull(ViewerUpdateService.SelectRelease(Release("4.0.0"), numeric, "x64", false, currentTag: "4.0.0-beta.2"));
        Assert.Null(ViewerUpdateService.SelectRelease(Release("4.0.0-beta.3"), numeric, "x64", true, currentTag: "4.0.0"));
    }

    [Fact]
    public async Task SelectsHighestPreviewEvenWhenApiOrderIsDifferent()
    {
        using var client = new HttpClient(new Handler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(new[]
            {
                Release("4.0.0-beta.3"), Release("4.0.0-beta.10"), Release("4.0.0-beta.2")
            }))
        }));
        var result = await new ViewerUpdateService(client).CheckAsync(new Version(4, 0, 0), "x64",
            TestContext.Current.CancellationToken, true, "4.0.0-beta.1");
        Assert.Equal("4.0.0-beta.10", result!.Tag);
    }

    [Fact]
    public void InstalledInformationalVersionContainsPreviewAndParsesWithBuildMetadata()
    {
        var assembly = typeof(ViewerUpdateService).Assembly;
        var tag = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
        Assert.True(ViewerVersion.TryParse(tag, out var parsed));
        Assert.Equal(ViewerVersion.FromNumeric(assembly.GetName().Version!).Numeric, parsed.Numeric);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DownloadedExecutableMustMatchFullPreviewVersion(bool mismatch)
    {
        var root = System.IO.Directory.CreateDirectory(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "PreviewPayload-" + Guid.NewGuid())).FullName;
        try
        {
            var archive = System.IO.Path.Combine(root, "payload.zip");
            using (var zip = System.IO.Compression.ZipFile.Open(archive, System.IO.Compression.ZipArchiveMode.Create))
            using (var output = zip.CreateEntry("NxFileViewer.exe").Open())
            using (var input = System.IO.File.OpenRead(System.IO.Path.Combine(AppContext.BaseDirectory, "NxFileViewer.exe")))
                input.CopyTo(output);
            var bytes = System.IO.File.ReadAllBytes(archive);
            var assembly = typeof(ViewerUpdateService).Assembly;
            var installed = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
            Assert.True(ViewerVersion.TryParse(installed, out var version));
            var tag = mismatch ? version.Numeric + "-beta.999999" : installed;
            var release = new ViewerRelease(version.Numeric, "NxFileViewer_v" + tag + "_x64.zip",
                new Uri($"https://github.com/{ViewerUpdateService.Repository}/releases/download/v{tag}/app.zip"),
                "sha256:" + Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes))) { Tag = tag };
            using var client = new HttpClient(new Handler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes)
            }));
            var service = new ViewerUpdateService(client);
            if (mismatch)
                await Assert.ThrowsAsync<System.IO.InvalidDataException>(() => service.PrepareAsync(release, root, TestContext.Current.CancellationToken));
            else
                Assert.True(System.IO.File.Exists((await service.PrepareAsync(release, root, TestContext.Current.CancellationToken)).Executable));
        }
        finally { System.IO.Directory.Delete(root, true); }
    }


    private sealed class Handler(Func<HttpRequestMessage, HttpResponseMessage> response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) =>
            Task.FromResult(response(request));
    }
}
