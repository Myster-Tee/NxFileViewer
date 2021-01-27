using System.Threading.Tasks;

namespace Emignatik.NxFileViewer.Services
{
    public interface IFileDownloaderService
    {
        /// <summary>
        /// Downloads a file from the specified URL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        public Task DownloadFileAsync(string url, string filePath);
    }
}
