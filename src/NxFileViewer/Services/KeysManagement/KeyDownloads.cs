using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Services.OnlineServices;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.Services.KeysManagement;

public static class KeyDownloads
{
    public static string ResolveUrl(string template, string host)
    {
        if (template.Contains("{IP}", StringComparison.OrdinalIgnoreCase))
        {
            host = host.Trim().Trim('[', ']');
            if (Uri.CheckHostName(host) == UriHostNameType.Unknown) throw new ArgumentException("Invalid download IP address or host.");
            if (host.Contains(':')) host = "[" + host + "]";
            template = template.Replace("{IP}", host, StringComparison.OrdinalIgnoreCase);
        }
        if (!Uri.TryCreate(template, UriKind.Absolute, out var uri) || uri.Scheme is not ("ftp" or "http" or "https"))
            throw new ArgumentException("Use a valid FTP, HTTP or HTTPS download URL.");
        return template;
    }

    public static string Destination(string? customPath, string programPath) =>
        (string.IsNullOrWhiteSpace(customPath) ? programPath : customPath).ToFullPath();

    public static void MigrateLegacyHost(IAppSettings settings)
    {
        if (!Uri.TryCreate(settings.ProdKeysDownloadUrl, UriKind.Absolute, out var prod) ||
            !Uri.TryCreate(settings.TitleKeysDownloadUrl, UriKind.Absolute, out var title) ||
            prod.Scheme != "ftp" || title.Scheme != "ftp" || prod.UserInfo.Length != 0 || title.UserInfo.Length != 0 ||
            !string.Equals(prod.Host, title.Host, StringComparison.OrdinalIgnoreCase) ||
            settings.ProdKeysDownloadUrl.Contains('{') || settings.TitleKeysDownloadUrl.Contains('{')) return;
        settings.KeysDownloadHost = prod.Host.Trim('[', ']');
        settings.ProdKeysDownloadUrl = Template(settings.ProdKeysDownloadUrl, prod);
        settings.TitleKeysDownloadUrl = Template(settings.TitleKeysDownloadUrl, title);

        static string Template(string original, Uri uri)
        {
            var start = original.IndexOf("://", StringComparison.Ordinal) + 3;
            var length = original[start] == '[' ? original.IndexOf(']', start) - start + 1 : uri.Host.Length;
            return original[..start] + "{IP}" + original[(start + length)..];
        }
    }

    public static async Task DownloadAsync(IHttpDownloader downloader, string url, string destination, CancellationToken token,
        Func<string, string, bool>? confirmOverwrite = null)
    {
        var directory = Path.GetDirectoryName(destination)!;
        Directory.CreateDirectory(directory);
        var temporary = Path.Combine(directory, ".keys-" + Guid.NewGuid().ToString("N") + ".download");
        try
        {
            await downloader.DownloadFileAsync(url, temporary, token).ConfigureAwait(false);
            token.ThrowIfCancellationRequested();
            if (File.Exists(destination) && confirmOverwrite != null && !confirmOverwrite(temporary, destination))
                throw new OperationCanceledException("Key replacement declined.", token);
            token.ThrowIfCancellationRequested();
            File.Move(temporary, destination, overwrite: true);
        }
        finally
        {
            try { File.Delete(temporary); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
}
