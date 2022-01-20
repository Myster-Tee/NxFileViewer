using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Emignatik.NxFileViewer.Services;

public class HttpDownloader : IHttpDownloader
{
    public async Task DownloadFileAsync(string url, string destFilePath, CancellationToken cancellationToken)
    {
        var uri = new Uri(url);

        using var httpClient = new HttpClient();

        var response = await httpClient.GetAsync(uri, cancellationToken);

        await using var fileStream = File.Create(destFilePath);

        await response.Content.CopyToAsync(fileStream, cancellationToken);
    }

}