using System;
using System.Diagnostics;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model;
using Emignatik.NxFileViewer.Model.Overview;
using Emignatik.NxFileViewer.Model.TreeItems;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services
{
    public class FileOpenerService : IFileOpenerService
    {
        private readonly IOpenedFileService _openedFileService;
        private readonly IAppSettings _appSettings;
        private readonly IFileTypeAnalyzer _fileTypeAnalyzer;
        private readonly IFileItemLoader _fileItemLoader;
        private readonly IFileOverviewLoader _fileOverviewLoader;
        private readonly ILogger _logger;

        public FileOpenerService(
            IOpenedFileService openedFileService,
            ILoggerFactory loggerFactory,
            IAppSettings appSettings,
            IFileTypeAnalyzer fileTypeAnalyzer,
            IFileItemLoader fileItemLoader,
            IFileOverviewLoader fileOverviewLoader)
        {
            _openedFileService = openedFileService ?? throw new ArgumentNullException(nameof(openedFileService));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _fileTypeAnalyzer = fileTypeAnalyzer ?? throw new ArgumentNullException(nameof(fileTypeAnalyzer));
            _fileItemLoader = fileItemLoader ?? throw new ArgumentNullException(nameof(fileItemLoader));
            _fileOverviewLoader = fileOverviewLoader;
            _logger = loggerFactory.CreateLogger(this.GetType());
        }

        public void OpenFile(string filePath)
        {
            try
            {
                _appSettings.LastOpenedFile = filePath;

                IItem rootItem;
                FileOverview fileOverview;
                switch (_fileTypeAnalyzer.GetFileType(filePath))
                {
                    case FileType.UNKNOWN:
                        _logger.LogError(string.Format(LocalizationManager.Instance.Current.Keys.ErrFileNotSupported, filePath));
                        return;

                    case FileType.XCI:
                        var xciItem = _fileItemLoader.LoadXci(filePath);
                        fileOverview = _fileOverviewLoader.Load(xciItem);
                        rootItem = xciItem;

                        break;
                    case FileType.NSP:
                        var nspItem = _fileItemLoader.LoadNsp(filePath);
                        fileOverview = _fileOverviewLoader.Load(nspItem);
                        rootItem = nspItem;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (_appSettings.StructureLoadingMode == StructureLoadingMode.Full)
                    ForceLoadAllChildren(rootItem);

                _openedFileService.OpenedFile = new OpenedFile(filePath, rootItem, fileOverview);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(LocalizationManager.Instance.Current.Keys.LoadingError_Failed, filePath, ex.Message), ex);
            }
        }

        private static void ForceLoadAllChildren(IItem rootItem)
        {
            try
            {
                rootItem.LoadChildItems(true);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"{nameof(IItem)}.{nameof(rootItem.LoadChildItems)} exception: {ex.Message}");
            }
            foreach (var childItem in rootItem.ChildItems)
            {
                ForceLoadAllChildren(childItem);
            }
        }
    }
}
