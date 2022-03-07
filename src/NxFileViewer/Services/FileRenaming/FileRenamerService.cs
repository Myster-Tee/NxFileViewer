using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Emignatik.NxFileViewer.FileLoading.QuickFileInfoLoading;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;
using LibHac.Ncm;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.FileRenaming;

public class FileRenamerService : IFileRenamerService
{
    private readonly IPackageInfoLoader _packageInfoLoader;
    private readonly ILogger _logger;

    public FileRenamerService(ILoggerFactory loggerFactory, IPackageInfoLoader packageInfoLoader)
    {
        _packageInfoLoader = packageInfoLoader;
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());

    }


    public void RenameFromDirectory(string inputDirectory, INamingPatterns namingPatterns, IReadOnlyCollection<string> fileExtensions, bool includeSubDirectories)
    {
        var searchOption = includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;


        var directoryInfo = new DirectoryInfo(inputDirectory);

        var matchingFiles = directoryInfo.GetFiles("*", searchOption).Where(file =>
        {
            return fileExtensions.Any(fileExtension => string.Equals(file.Extension, fileExtension, StringComparison.OrdinalIgnoreCase));
        }).ToArray();

        foreach (var matchingFile in matchingFiles)
        {
            // TODO: try/catcher et loguer
            RenameFile(matchingFile.FullName, namingPatterns);
        }
    }

    public void RenameFile(string inputFile, INamingPatterns namingPatterns)
    {
        var packageInfo = _packageInfoLoader.GetPackageInfo(inputFile);

        if (packageInfo.Contents.Count == 1)
        {
            var content = packageInfo.Contents[0];
            switch (content.Type)
            {
                case ContentMetaType.Application:
                    RenameApplicationPackage(inputFile, content, packageInfo.AccuratePackageType, namingPatterns.ApplicationPattern);
                    break;
                case ContentMetaType.Patch:
                    //RenamePatchFile();
                    break;
                case ContentMetaType.AddOnContent:
                    //RenameAddOnFile();
                    break;
                default:
                    //TODO: loguer comme quoi pas supporté
                    break;
            }


        }
        else
        {
            //TODO: gérer les super NSP/XCI
        }

    }

    private void RenameApplicationPackage(string inputFile, Content content, AccuratePackageType accuratePackageType, IEnumerable<ApplicationPatternPart> patternParts)
    {
        var newFileName = "";


        foreach (var patternPart in patternParts)
        {
            if (patternPart is StaticTextApplicationPatternPart staticText)
            {
                newFileName += staticText.Text;
            }
            else if(patternPart is DynamicTextApplicationPatternPart dynamicText)
            {
                switch (dynamicText.Keyword)
                {
                    case ApplicationKeyword.TitleIdL:
                        newFileName += content.TitleId.ToLower();
                        break;
                    case ApplicationKeyword.TitleIdU:
                        newFileName += content.TitleId.ToUpper();
                        break;
                    case ApplicationKeyword.FirstTitleName:
                        var firstTitle = content.NacpData?.Titles.FirstOrDefault();

                        if (firstTitle != null)
                            newFileName += firstTitle.Name;
                        else
                            newFileName += "NO_TITLE";

                        break;
                    case ApplicationKeyword.PackageTypeL:
                        newFileName += accuratePackageType.ToString().ToLower();
                        break;
                    case ApplicationKeyword.PackageTypeU:
                        newFileName += accuratePackageType.ToString().ToUpper();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                // TODO: throw
            }

        }


        throw new NotImplementedException();
    }
}

