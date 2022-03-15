using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.FileLoading.QuickFileInfoLoading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.FileRenaming.Exceptions;
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

    public FileRenamerService(IPackageInfoLoader packageInfoLoader, IOnlineTitleInfoService onlineTitleInfoService)
    {
        _packageInfoLoader = packageInfoLoader ?? throw new ArgumentNullException(nameof(packageInfoLoader));
        _onlineTitleInfoService = onlineTitleInfoService ?? throw new ArgumentNullException(nameof(onlineTitleInfoService));
    }

    public async Task RenameFromDirectoryAsync(string inputDirectory, string? fileFilters, bool includeSubdirectories, INamingSettings namingSettings, bool isSimulation, ILogger? logger, IProgressReporter progressReporter, CancellationToken cancellationToken)
    {
        CheckInvalidWindowsFileNameChar(namingSettings.InvalidFileNameCharsReplacement);

        var searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        var directoryInfo = new DirectoryInfo(inputDirectory);

        var fileFiltersRegex = fileFilters?.Split(';').Select(filter => new Regex(filter.Replace("*", ".*"), RegexOptions.IgnoreCase | RegexOptions.Singleline)).ToArray();

        var matchingFiles = directoryInfo.GetFiles("*", searchOption).Where(file =>
        {
            return fileFiltersRegex == null || fileFiltersRegex.Any(regex => regex.IsMatch(file.Name));
        }).ToArray();


        logger?.LogInformation(LocalizationManager.Instance.Current.Keys.RenamingTool_LogNbFilesToRename.SafeFormat(matchingFiles.Length));

        var logPrefix = isSimulation ? $"{LocalizationManager.Instance.Current.Keys.RenamingTool_LogSimulationMode}" : "";

        for (var index = 0; index < matchingFiles.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var matchingFile = matchingFiles[index];
            progressReporter.SetText(matchingFile.Name);

            try
            {
                var renamingResult = await RenameFileAsyncInternal(matchingFile, namingSettings, isSimulation, cancellationToken);

                if (renamingResult.IsRenamed)
                {
                    var message = LocalizationManager.Instance.Current.Keys.RenamingTool_LogFileRenamed.SafeFormat(logPrefix, renamingResult.OldFileName, renamingResult.NewFileName);
                    logger?.LogWarning(message);
                }
                else
                {
                    logger?.LogInformation(LocalizationManager.Instance.Current.Keys.RenamingTool_LogFileAlreadyNamedProperly.SafeFormat(logPrefix, renamingResult.OldFileName));
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, LocalizationManager.Instance.Current.Keys.RenamingTool_FailedToRenameFile.SafeFormat(matchingFile.FullName, ex.Message));
            }

            progressReporter.SetPercentage((index + 1) / (double)matchingFiles.Length);
        }
        progressReporter.SetText("");
    }

    public Task<RenamingResult> RenameFileAsync(string inputFile, INamingSettings namingSettings, bool isSimulation, CancellationToken cancellationToken)
    {
        CheckInvalidWindowsFileNameChar(namingSettings.InvalidFileNameCharsReplacement);
        return RenameFileAsyncInternal(new FileInfo(inputFile), namingSettings, isSimulation, cancellationToken);
    }

    private static void CheckInvalidWindowsFileNameChar(string? invalidFileNameCharsReplacement)
    {
        if (invalidFileNameCharsReplacement == null)
            return;

        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (var invalidChar in invalidChars)
        {
            if (invalidFileNameCharsReplacement.Contains(invalidChar))
                throw new BadInvalidFileNameCharReplacementException(invalidFileNameCharsReplacement, invalidChar);
        }
    }

    private async Task<RenamingResult> RenameFileAsyncInternal(FileInfo inputFile, INamingSettings namingSettings, bool isSimulation, CancellationToken cancellationToken)
    {
        string newFileName;

        var packageInfo = _packageInfoLoader.GetPackageInfo(inputFile.FullName);
        var oldFileName = inputFile.Name;

        if (packageInfo.Contents.Count == 1)
        {
            var content = packageInfo.Contents[0];

            switch (content.Type)
            {
                case ContentMetaType.Application:
                    newFileName = ComputeApplicationPackageFileName(content, packageInfo.AccuratePackageType, namingSettings.ApplicationPattern, cancellationToken);
                    break;
                case ContentMetaType.Patch:
                    newFileName = ComputePatchPackageFileName(content, packageInfo.AccuratePackageType, namingSettings.PatchPattern, cancellationToken);
                    break;
                case ContentMetaType.AddOnContent:
                    newFileName = await ComputeAddonPackageFileName(content, packageInfo.AccuratePackageType, namingSettings.AddonPattern, cancellationToken);
                    break;
                default:
                    throw new ContentTypeNotSupportedException(content.Type);
            }
        }
        else
        {
            //TODO: supporter les super NSP/XCI
            throw new SuperPackageNotSupportedException();
        }

        if (namingSettings.ReplaceWhiteSpaceChars)
        {
            var replacement = namingSettings.WhiteSpaceCharsReplacement ?? "";
            var whiteSpaceCharsToRemove = newFileName.Where(char.IsWhiteSpace).Distinct().ToArray();
            foreach (var whiteSpaceChar in whiteSpaceCharsToRemove)
            {
                newFileName = newFileName.Replace(whiteSpaceChar.ToString(), replacement);
            }
        }

        var invalidFileNameChars = Path.GetInvalidFileNameChars();
        var invalidCharReplacement = namingSettings.InvalidFileNameCharsReplacement ?? "";
        foreach (var invalidFileNameChar in invalidFileNameChars)
        {
            newFileName = newFileName.Replace(invalidFileNameChar.ToString(), invalidCharReplacement);
        }

        var shouldBeRenamed = !string.Equals(newFileName, oldFileName);
        if (!isSimulation && shouldBeRenamed)
        {
            var destFileName = Path.Combine(inputFile.DirectoryName!, newFileName);
            inputFile.MoveTo(destFileName, false);
        }

        return new RenamingResult
        {
            IsSimulation = isSimulation,
            IsRenamed = shouldBeRenamed,
            OldFileName = oldFileName,
            NewFileName = newFileName,
        };
    }

    private static string ComputeApplicationPackageFileName(Content content, AccuratePackageType accuratePackageType, IEnumerable<ApplicationPatternPart> patternParts, CancellationToken cancellationToken)
    {
        var newFileName = "";

        foreach (var patternPart in patternParts)
        {
            cancellationToken.ThrowIfCancellationRequested();
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
            cancellationToken.ThrowIfCancellationRequested();

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
            cancellationToken.ThrowIfCancellationRequested();

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