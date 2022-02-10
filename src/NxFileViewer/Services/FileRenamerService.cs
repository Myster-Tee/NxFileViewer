using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.FileLoading.QuickFileInfoLoading;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services;

public class FileRenamerService : IFileRenamerService
{
    private readonly IPackageInfoLoader _packageInfoLoader;
    private readonly ILogger _logger;

    public FileRenamerService(ILoggerFactory loggerFactory, IPackageInfoLoader packageInfoLoader)
    {
        _packageInfoLoader = packageInfoLoader;
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());

    }


    public void Rename(string inputDirectory, string namingPattern, IReadOnlyCollection<string> fileExtensions, bool includeSubDirectories)
    {
        var searchOption = includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;


        var directoryInfo = new DirectoryInfo(inputDirectory);

        var matchingFiles = directoryInfo.GetFiles("*", searchOption).Where(file =>
        {
            return fileExtensions.Any(fileExtension => string.Equals(file.Extension, fileExtension, StringComparison.OrdinalIgnoreCase));
        }).ToArray();

        foreach (var matchingFile in matchingFiles)
        {
            var packageInfo = _packageInfoLoader.GetPackageInfo(matchingFile.FullName);
        }






    }


}

