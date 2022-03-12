using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.FileLoading.QuickFileInfoLoading;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Addon;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Patch;
using Emignatik.NxFileViewer.Services.OnlineServices;
using LibHac.Ncm;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.FileRenaming;

public class FileRenamerService : IFileRenamerService
{
    private readonly IPackageInfoLoader _packageInfoLoader;
    private readonly IOnlineTitleInfoService _onlineTitleInfoService;
    private readonly ILogger _logger;

    public FileRenamerService(ILoggerFactory loggerFactory, IPackageInfoLoader packageInfoLoader, IOnlineTitleInfoService onlineTitleInfoService)
    {
        _packageInfoLoader = packageInfoLoader ?? throw new ArgumentNullException(nameof(packageInfoLoader));
        _onlineTitleInfoService = onlineTitleInfoService ?? throw new ArgumentNullException(nameof(onlineTitleInfoService));
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());

    }

    public async Task RenameFromDirectoryAsync(string inputDirectory, INamingPatterns namingPatterns, string? fileFilters, bool includeSubdirectories, bool simulation, ILogger? logger, IProgressReporter progressReporter, CancellationToken cancellationToken)
    {
        var searchOption = simulation ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        logger?.LogWarning("Hello World!");

        var directoryInfo = new DirectoryInfo(inputDirectory);

        var fileFiltersRegex = fileFilters?.Split(';').Select(filter => new Regex(filter.Replace("*", ".*"), RegexOptions.IgnoreCase | RegexOptions.Singleline)).ToArray();

        var matchingFiles = directoryInfo.GetFiles("*", searchOption).Where(file =>
        {
            return fileFiltersRegex == null || fileFiltersRegex.Any(regex => regex.IsMatch(file.Name));
        }).ToArray();

        foreach (var matchingFile in matchingFiles)
        {
            // TODO: try/catcher et loguer
            var newFileName = await RenameFileAsyncInternal(matchingFile, namingPatterns, cancellationToken);
            logger?.LogInformation(newFileName);
            Thread.Sleep(5000);
        }
    }

    public Task<string?> RenameFileAsync(string inputFile, INamingPatterns namingPatterns, CancellationToken cancellationToken)
    {
        return RenameFileAsyncInternal(new FileInfo(inputFile), namingPatterns, cancellationToken);
    }

    private async Task<string?> RenameFileAsyncInternal(FileInfo inputFile, INamingPatterns namingPatterns, CancellationToken cancellationToken)
    {
        string? newFileName = null;

        var packageInfo = _packageInfoLoader.GetPackageInfo(inputFile.FullName);

        if (packageInfo.Contents.Count == 1)
        {
            var content = packageInfo.Contents[0];

            switch (content.Type)
            {
                case ContentMetaType.Application:
                    newFileName = ComputeApplicationPackageFileName(content, packageInfo.AccuratePackageType, namingPatterns.ApplicationPattern, cancellationToken);
                    break;
                case ContentMetaType.Patch:
                    newFileName = ComputePatchPackageFileName(content, packageInfo.AccuratePackageType, namingPatterns.PatchPattern, cancellationToken);
                    break;
                case ContentMetaType.AddOnContent:
                    newFileName = await ComputeAddonPackageFileName(content, packageInfo.AccuratePackageType, namingPatterns.AddonPattern, cancellationToken);
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
            // TODO: gérer le problème d'écrasement
            inputFile.MoveTo(destFileName);
        }

        return newFileName;
    }

    private static string ComputeApplicationPackageFileName(Content content, AccuratePackageType accuratePackageType, IEnumerable<ApplicationPatternPart> patternParts, CancellationToken cancellationToken)
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

    private static string ComputePatchPackageFileName(Content content, AccuratePackageType accuratePackageType, IEnumerable<PatchPatternPart> patternParts, CancellationToken cancellationToken)
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

    private async Task<string> ComputeAddonPackageFileName(Content content, AccuratePackageType accuratePackageType, IEnumerable<AddonPatternPart> patternParts, CancellationToken cancellationToken)
    {
        var newFileName = "";

        foreach (var patternPart in patternParts)
        {
            switch (patternPart)
            {
                case StaticTextAddonPatternPart staticText:
                    newFileName += staticText.Text;
                    break;
                case DynamicTextAddonPatternPart dynamicText:
                    switch (dynamicText.Keyword)
                    {
                        case AddonKeyword.TitleIdL:
                            newFileName += content.TitleId.ToLower();
                            break;
                        case AddonKeyword.TitleIdU:
                            newFileName += content.TitleId.ToUpper();
                            break;
                        case AddonKeyword.OnlineTitleName:
                            //TODO: ne pas appeler à chaque fois
                            var onlineTitleInfo = await _onlineTitleInfoService.GetTitleInfoAsync(content.TitleId);

                            if (onlineTitleInfo != null)
                                newFileName += onlineTitleInfo.Name;
                            else
                                newFileName += "NO_TITLE";

                            break;
                        case AddonKeyword.PackageTypeL:
                            newFileName += accuratePackageType.ToString().ToLower();
                            break;
                        case AddonKeyword.PackageTypeU:
                            newFileName += accuratePackageType.ToString().ToUpper();
                            break;
                        case AddonKeyword.VersionNum:
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

