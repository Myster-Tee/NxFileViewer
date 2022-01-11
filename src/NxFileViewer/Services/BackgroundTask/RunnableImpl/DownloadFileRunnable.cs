using System;
using System.IO;
using System.Threading;
using Emignatik.NxFileViewer.Localization;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl
{
    public class DownloadFileRunnable : IDownloadFileRunnable
    {
        private readonly IHttpDownloader _httpDownloader;
        private readonly ILogger _logger;
        private string? _url;
        private string? _filePath;

        public DownloadFileRunnable(ILoggerFactory loggerFactory, IHttpDownloader httpDownloader)
        {
            _httpDownloader = httpDownloader ?? throw new ArgumentNullException(nameof(httpDownloader));
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
        }

        public bool SupportsCancellation => false;

        public bool SupportProgress => false;

        public void Run(IProgressReporter progressReporter, CancellationToken cancellationToken)
        {
            try
            {
                if (_url == null || _filePath == null)
                    throw new InvalidOperationException($"{nameof(Setup)} should be called first.");

                progressReporter.SetText(LocalizationManager.Instance.Current.Keys.Status_DownloadingFile.SafeFormat(Path.GetFileName(_filePath)));

                _logger.LogInformation(LocalizationManager.Instance.Current.Keys.Log_DownloadingFileFromUrl.SafeFormat(_filePath, _url));

                _httpDownloader.DownloadFileAsync(_url, _filePath, cancellationToken).Wait(cancellationToken);

                _logger.LogInformation(LocalizationManager.Instance.Current.Keys.Log_FileSuccessfullyDownloaded.SafeFormat(_filePath));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.Log_FailedToDownloadFileFromUrl.SafeFormat(_filePath, _url, ex.Message));
            }
        }

        public void Setup(string url, string filePath)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }
    }

    public interface IDownloadFileRunnable : IRunnable
    {
        void Setup(string url, string filePath);
    }
}
