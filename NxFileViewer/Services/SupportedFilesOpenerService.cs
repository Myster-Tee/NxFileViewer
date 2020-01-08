using System;
using System.IO;
using Emignatik.NxFileViewer.NSP;
using Emignatik.NxFileViewer.Properties;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services
{
    public class SupportedFilesOpenerService : ISupportedFilesOpenerService
    {
        private readonly IOpenedFileService _openedFileService;
        private readonly IAppSettings _appSettings;
        private readonly IKeySetProviderService _keySetProviderService;
        private readonly ILogger _logger;

        public SupportedFilesOpenerService(IOpenedFileService openedFileService, ILoggerFactory loggerFactory, IAppSettings appSettings, IKeySetProviderService keySetProviderService)
        {
            _openedFileService = openedFileService ?? throw new ArgumentNullException(nameof(openedFileService));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _keySetProviderService = keySetProviderService ?? throw new ArgumentNullException(nameof(keySetProviderService));
            _logger = loggerFactory.CreateLogger(this.GetType());
        }

        public void OpenFile(string filePath)
        {
            try
            {
                _appSettings.LastOpenedFile = filePath;
                _appSettings.Save();
            }
            catch
            {
            }

            try
            {
                var fileName = Path.GetFileName(filePath);
                _logger.LogInformation($"===> {fileName} <===");

                var nspInfoLoader = new NspInfoLoader(_keySetProviderService.GetKeySet());
                var nspInfo = nspInfoLoader.Load(filePath);

                _openedFileService.OpenedFile = new OpenedFile
                {
                    FileData = nspInfo,
                    Location = filePath
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Resources.ErrFailedToLoadFile, filePath, ex.Message), ex);
            }
        }

    }
}
