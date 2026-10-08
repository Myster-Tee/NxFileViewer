using System;
using System.IO;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Services.OnlineServices;
using Emignatik.NxFileViewer.Settings;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Services.OnlineServices;

public class HttpDownloaderTest
{
    [Fact]
    public void AppSettings_UseEditableSphairaFtpDefaults()
    {
        var settings = new AppSettings();

        Assert.Equal("192.168.178.100", settings.KeysDownloadHost);
        Assert.Equal("ftp://{IP}:5000/sdmc:/switch/prod.keys", settings.ProdKeysDownloadUrl);
        Assert.Equal("ftp://{IP}:5000/sdmc:/switch/title.keys", settings.TitleKeysDownloadUrl);
        Assert.Equal("ftp://192.168.178.100:5000/sdmc:/switch/prod.keys",
            Emignatik.NxFileViewer.Services.KeysManagement.KeyDownloads.ResolveUrl(settings.ProdKeysDownloadUrl, settings.KeysDownloadHost));
    }

    [Fact]
    public async Task DownloadFileAsync_RejectsUnsupportedSchemesWithoutReplacingDestination()
    {
        var destination = Path.GetTempFileName();
        await File.WriteAllTextAsync(destination, "existing", TestContext.Current.CancellationToken);

        try
        {
            var downloader = new HttpDownloader();
            await Assert.ThrowsAsync<NotSupportedException>(() =>
                downloader.DownloadFileAsync("sftp://example.invalid/prod.keys", destination, TestContext.Current.CancellationToken));

            Assert.Equal("existing", await File.ReadAllTextAsync(destination, TestContext.Current.CancellationToken));
        }
        finally
        {
            File.Delete(destination);
        }
    }
}
