using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model;
using Emignatik.NxFileViewer.Model.Overview;
using Emignatik.NxFileViewer.Model.TreeItems;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.FileLoading;

internal class FileLoader : IFileLoader
{
    private readonly IPackageTypeAnalyzer _packageTypeAnalyzer;
    private readonly IFileItemLoader _fileItemLoader;
    private readonly IFileOverviewLoader _fileOverviewLoader;
    private readonly ILogger _logger;

    public FileLoader(ILoggerFactory loggerFactory, IPackageTypeAnalyzer packageTypeAnalyzer, IFileItemLoader fileItemLoader, IFileOverviewLoader fileOverviewLoader)
    {
        _packageTypeAnalyzer = packageTypeAnalyzer ?? throw new ArgumentNullException(nameof(packageTypeAnalyzer));
        _fileItemLoader = fileItemLoader ?? throw new ArgumentNullException(nameof(fileItemLoader));
        _fileOverviewLoader = fileOverviewLoader ?? throw new ArgumentNullException(nameof(fileOverviewLoader));
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
    }

    public NxFile Load(string filePath)
    {
        _logger.LogInformation(LocalizationManager.Instance.Current.Keys.Log_OpeningFile.SafeFormat(filePath));


        HashSet<MissingKey> missingKeys = new();
        _fileItemLoader.MissingKey += (_, args) =>
        {
            var ex = args.Exception;
            var missingKey = new MissingKey(ex.Name, ex.Type);

            missingKeys.Add(missingKey);
        };

        IItem rootItem;
        FileOverview fileOverview;
        switch (_packageTypeAnalyzer.GetType(filePath))
        {
            case PackageType.UNKNOWN:
                throw new FileNotSupportedException(filePath);

            case PackageType.XCI:
                var xciItem = _fileItemLoader.LoadXci(filePath);
                fileOverview = _fileOverviewLoader.Load(xciItem);
                rootItem = xciItem;

                break;
            case PackageType.NSP:
                var nspItem = _fileItemLoader.LoadNsp(filePath);
                fileOverview = _fileOverviewLoader.Load(nspItem);
                rootItem = nspItem;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        foreach (var missingKey in missingKeys)
            fileOverview.MissingKeys.Add(missingKey);

        var openedFile = new NxFile(filePath, rootItem, fileOverview);
        return openedFile;
    }


}