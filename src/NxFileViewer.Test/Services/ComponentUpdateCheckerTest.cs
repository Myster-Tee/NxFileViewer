using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Services.Nand;
using Emignatik.NxFileViewer.Services.Nsz;
using Emignatik.NxFileViewer.Services.Updates;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services;

public sealed class ComponentUpdateCheckerTest
{
    [Theory]
    [InlineData("v23.0.1", "23.0.2", true)]
    [InlineData("23.0.2", "v23.0.2", false)]
    [InlineData("23.0.3", "23.0.2", false)]
    public void VersionsCompareNumerically(string installed, string latest, bool expected) =>
        Assert.Equal(expected, ComponentUpdateChecker.IsNewer(installed, latest));

    [Fact]
    public async Task ChecksInstalledPluginsAndDataWithoutChangingFilesAndContinuesAfterFailure()
    {
        var root = Path.Combine(Path.GetTempPath(), "component-update-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var plugin = Path.Combine(root, "nsz");
            Directory.CreateDirectory(plugin);
            File.WriteAllText(Path.Combine(plugin, "nsz.exe"), "fixture");
            File.WriteAllText(Path.Combine(plugin, "active.json"), "{\"Executable\":\"nsz.exe\",\"Version\":\"1.0\"}");
            var titleRoot = Path.Combine(root, "Cache", "TitleDB");
            Directory.CreateDirectory(titleRoot);
            var titlePath = Path.Combine(titleRoot, "US.en.json");
            File.WriteAllText(titlePath, "{}");
            var sha = Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes("blob 2\0{}")));
            using var client = new HttpClient(new Handler(sha));
            var settings = new AppSettings { TitleDbRegion = "US.en" };
            var checker = new ComponentUpdateChecker(new NszPluginManager(settings, NullLogger<NszPluginManager>.Instance, plugin),
                new NandPluginManager(settings, Path.Combine(root, "nand")), settings, client, root);
            var status = await checker.CheckAsync(CancellationToken.None);
            Assert.Contains("NSZ:", status);
            Assert.Contains("2.0", status);
            Assert.Contains("NxNandManager:", status);
            Assert.Contains("Title DB US.en:", status);
            Assert.Equal("{}", File.ReadAllText(titlePath));
            Assert.Equal("fixture", File.ReadAllText(Path.Combine(plugin, "nsz.exe")));
            Assert.True(ComponentUpdateChecker.MatchesGitBlob(titlePath, sha));
            File.WriteAllText(titlePath, "changed");
            Assert.False(ComponentUpdateChecker.MatchesGitBlob(titlePath, sha));
        }
        finally { Directory.Delete(root, true); }
    }

    private sealed class Handler(string sha) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            var url = request.RequestUri!.AbsoluteUri;
            return Task.FromResult(url.Contains("fw/hashes") ? new HttpResponseMessage(HttpStatusCode.ServiceUnavailable) :
                new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(url.Contains("releases/latest") ?
                    "{\"tag_name\":\"2.0\"}" : "{\"sha\":\"" + sha + "\"}") });
        }
    }
}
