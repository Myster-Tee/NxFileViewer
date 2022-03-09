using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Emignatik.NxFileViewer.FileLoading.QuickFileInfoLoading;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Patch;
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
            RenameFile(matchingFile, namingPatterns, out _);
        }
    }

    public void RenameFile(FileInfo inputFile, INamingPatterns namingPatterns, out string? newFileName)
    {
        newFileName = null;

        var packageInfo = _packageInfoLoader.GetPackageInfo(inputFile.FullName);

        if (packageInfo.Contents.Count == 1)
        {
            var content = packageInfo.Contents[0];

            switch (content.Type)
            {
                case ContentMetaType.Application:
                    newFileName = ComputeApplicationPackageFileName(content, packageInfo.AccuratePackageType, namingPatterns.ApplicationPattern);
                    break;
                case ContentMetaType.Patch:
                    newFileName = ComputePatchPackageFileName(content, packageInfo.AccuratePackageType, namingPatterns.PatchPattern);
                    break;
                case ContentMetaType.AddOnContent:
                    //RenameAddOnPackage();
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

        if (newFileName != null && newFileName != inputFile.Name && inputFile.DirectoryName != null)
        {
            var destFileName = Path.Combine(inputFile.DirectoryName, newFileName);
            inputFile.MoveTo(destFileName);
        }

    }

    private string ComputeApplicationPackageFileName(Content content, AccuratePackageType accuratePackageType, IEnumerable<ApplicationPatternPart> patternParts)
    {
        var newFileName = "";

        foreach (var patternPart in patternParts)
        {
            switch (patternPart)
            {
                case StaticTextApplicationPatternPart staticText:
                    newFileName += staticText.Text;
                    break;
                case DynamicTextApplicationPatternPart dynamicText:
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
                        case ApplicationKeyword.VersionNum:
                            newFileName += content.Version.Version.ToString();
                            break;
                        default:
                            throw new NotSupportedException($"Unknown application keyword «{dynamicText.Keyword}».");
                    }

                    break;
                default:
                    throw new NotSupportedException($"Unknown part of type «{patternPart.GetType().Name}».");
            }
        }

        return newFileName;
    }

    private string ComputePatchPackageFileName(Content content, AccuratePackageType accuratePackageType, IReadOnlyList<PatchPatternPart> patternParts)
    {
        var newFileName = "";

        foreach (var patternPart in patternParts)
        {
            switch (patternPart)
            {
                case StaticTextPatchPatternPart staticText:
                    newFileName += staticText.Text;
                    break;
                case DynamicTextPatchPatternPart dynamicText:
                    switch (dynamicText.Keyword)
                    {
                        case PatchKeyword.TitleIdL:
                            newFileName += content.TitleId.ToLower();
                            break;
                        case PatchKeyword.TitleIdU:
                            newFileName += content.TitleId.ToUpper();
                            break;
                        case PatchKeyword.FirstTitleName:
                            var firstTitle = content.NacpData?.Titles.FirstOrDefault();

                            if (firstTitle != null)
                                newFileName += firstTitle.Name;
                            else
                                newFileName += "NO_TITLE";

                            break;
                        case PatchKeyword.PackageTypeL:
                            newFileName += accuratePackageType.ToString().ToLower();
                            break;
                        case PatchKeyword.PackageTypeU:
                            newFileName += accuratePackageType.ToString().ToUpper();
                            break;
                        case PatchKeyword.VersionNum:
                            newFileName += content.Version.Version.ToString();
                            break;
                        default:
                            throw new NotSupportedException($"Unknown patch keyword «{dynamicText.Keyword}».");
                    }

                    break;
                default:
                    throw new NotSupportedException($"Unknown part of type «{patternPart.GetType().Name}».");
            }
        }

        return newFileName;
    }
}

