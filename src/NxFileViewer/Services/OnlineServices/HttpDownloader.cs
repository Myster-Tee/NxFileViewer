using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Emignatik.NxFileViewer.Services.OnlineServices;

public class HttpDownloader : IHttpDownloader
{
    public async Task DownloadFileAsync(string url, string destFilePath, CancellationToken cancellationToken)
    {
        var uri = new Uri(url, UriKind.Absolute);
        var tempFilePath = $"{destFilePath}.{Guid.NewGuid():N}.download";

        try
        {
            switch (uri.Scheme.ToLowerInvariant())
            {
                case "http":
                case "https":
                    await DownloadHttpAsync(uri, tempFilePath, cancellationToken);
                    break;
                case "ftp":
                    await DownloadAnonymousFtpAsync(uri, tempFilePath, cancellationToken);
                    break;
                default:
                    throw new NotSupportedException($"The URL scheme '{uri.Scheme}' is not supported. Use HTTP, HTTPS or FTP.");
            }

            File.Move(tempFilePath, destFilePath, overwrite: true);
        }
        finally
        {
            try { File.Delete(tempFilePath); }
            catch
            {
                // Best-effort cleanup of an incomplete download.
            }
        }
    }

    private static async Task DownloadHttpAsync(Uri uri, string tempFilePath, CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var fileStream = File.Create(tempFilePath);
        await response.Content.CopyToAsync(fileStream, cancellationToken);
    }

    private static async Task DownloadAnonymousFtpAsync(Uri uri, string tempFilePath, CancellationToken cancellationToken)
    {
#pragma warning disable SYSLIB0014 // FtpWebRequest remains the built-in FTP client in .NET 8.
        var request = (FtpWebRequest)WebRequest.Create(uri);
#pragma warning restore SYSLIB0014
        request.Method = WebRequestMethods.Ftp.DownloadFile;
        request.Credentials = new NetworkCredential("anonymous", "anonymous@");
        request.UseBinary = true;
        request.UsePassive = true;
        request.KeepAlive = false;

        using var cancellationRegistration = cancellationToken.Register(request.Abort);
        try
        {
#pragma warning disable SYSLIB0014
            using var response = (FtpWebResponse)await request.GetResponseAsync();
#pragma warning restore SYSLIB0014
            await using var responseStream = response.GetResponseStream();
            await using var fileStream = File.Create(tempFilePath);

            await responseStream.CopyToAsync(fileStream, cancellationToken);
        }
        catch (WebException) when (cancellationToken.IsCancellationRequested)
        {
            throw new OperationCanceledException(cancellationToken);
        }
    }

}
