using System;
using System.IO;
using Emignatik.NxFileViewer.NSP;
using Emignatik.NxFileViewer.Properties;
using log4net;

namespace Emignatik.NxFileViewer.Services
{
    public class SupportedFilesOpenerService
    {
        private readonly OpenedFileService _openedFileService;
        private readonly ILog _log;

        public SupportedFilesOpenerService(OpenedFileService openedFileService)
        {
            _openedFileService = openedFileService ?? throw new ArgumentNullException(nameof(openedFileService));
            _log = LogManager.GetLogger(this.GetType());
        }

        public void OpenFile(string filePath)
        {
            try
            {
                Settings.Default.LastOpenedFile = filePath;
                Settings.Default.Save();
            }
            catch
            {
            }

            try
            {
                var fileName = Path.GetFileName(filePath);
                _log.Info($"===> {fileName} <===");

                var nspInfoLoader = new NspInfoLoader(KeySetProviderService.GetKeySet());
                var nspInfo = nspInfoLoader.Load(filePath);

                _openedFileService.OpenedFile = new OpenedFile
                {
                    FileData = nspInfo,
                    Location = filePath
                };
            }
            catch (Exception ex)
            {
                _log.Error(string.Format(Resources.ErrFailedToLoadFile, filePath), ex);
            }
        }

    }
}
