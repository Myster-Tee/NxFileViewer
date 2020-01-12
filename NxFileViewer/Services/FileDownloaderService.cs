using System.Net;
using System.Threading.Tasks;

namespace Emignatik.NxFileViewer.Services
{
    public class FileDownloaderService : IFileDownloaderService
    {
        public async Task DownloadFileAsync(string url, string filePath)
        {
            using var client = new WebClient();
            await client.DownloadFileTaskAsync(url, filePath);
        }
    }
}