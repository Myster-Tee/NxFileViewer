using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;
using Emignatik.NxFileViewer.Services.OnlineServices;
using LibHac.Ncm;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.FileRenaming;

public class FileRenamerService : IFileRenamerService
{
    private readonly IPackageInfoLoader _packageInfoLoader;
    private readonly ICachedOnlineTitleInfoService _cachedOnlineTitleInfoService;

    public FileRenamerService(IPackageInfoLoader packageInfoLoader, ICachedOnlineTitleInfoService cachedOnlineTitleInfoService)
    {
        _packageInfoLoader = packageInfoLoader ?? throw new ArgumentNullException(nameof(packageInfoLoader));
        _cachedOnlineTitleInfoService = cachedOnlineTitleInfoService ?? throw new ArgumentNullException(nameof(cachedOnlineTitleInfoService));
    }

    public async Task<IList<RenamingResult>> RenameFromDirectoryAsync(string inputDirectory, string? fileFilters, bool includeSubdirectories, INamingSettings namingSettings, bool isSimulation, ILogger? logger, IProgressReporter progressReporter, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(inputDirectory))
            throw new ArgumentNullException(nameof(inputDirectory)); //TODO: faire un message 

        ValidateNamingSettings(namingSettings);

        var searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        var directoryInfo = new DirectoryInfo(inputDirectory);

        var fileFiltersRegex = fileFilters?.Split(';').Select(filter => new Regex(filter.Replace("*", ".*"), RegexOptions.IgnoreCase | RegexOptions.Singleline)).ToArray();

        var matchingFiles = directoryInfo.GetFiles("*", searchOption).Where(file =>
        {
            return fileFiltersRegex == null || fileFiltersRegex.Any(regex => regex.IsMatch(file.Name));
        }).ToArray();

        var renamingResults = new List<RenamingResult>();

        logger?.LogInformation(LocalizationManager.Instance.Current.Keys.RenamingTool_LogNbFilesToRename.SafeFormat(matchingFiles.Length));


        for (var index = 0; index < matchingFiles.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var matchingFile = matchingFiles[index];
            progressReporter.SetText(matchingFile.Name);

            var renamingResult = await RenameFileAsyncInternalSafe(matchingFile, namingSettings, isSimulation, logger, cancellationToken);
            renamingResults.Add(renamingResult);

            progressReporter.SetPercentage((index + 1) / (double)matchingFiles.Length);
        }
        progressReporter.SetText("");
        return renamingResults;
    }

    public async Task<RenamingResult> RenameFileAsync(string inputFile, INamingSettings namingSettings, bool isSimulation, ILogger? logger, CancellationToken cancellationToken)
    {
        ValidateNamingSettings(namingSettings);
        var renamingResult = await RenameFileAsyncInternalSafe(new FileInfo(inputFile), namingSettings, isSimulation, logger, cancellationToken);

        return renamingResult;
    }

    private static void ValidateNamingSettings(INamingSettings namingSettings)
    {
        ValidateInvalidWindowsFileNameChar(namingSettings.InvalidFileNameCharsReplacement);
        ValidateAllowedKeywords(namingSettings.ApplicationPattern, namingSettings.PatchPattern, namingSettings.AddonPattern);
    }

    private static void ValidateAllowedKeywords(IEnumerable<PatternPart> applicationPattern, IEnumerable<PatternPart> patchPattern, IEnumerable<PatternPart> addonPattern)
    {
        if (HasNotAllowedKeyword(applicationPattern, PatternKeywordHelper.GetAllowedApplicationKeywords(), out var firstNotAllowedApplicationKeyword))
            throw new KeywordNotAllowedException(firstNotAllowedApplicationKeyword.Value, PatternType.Application);

        if (HasNotAllowedKeyword(patchPattern, PatternKeywordHelper.GetAllowedPatchKeywords(), out var firstNotAllowedPatchKeyword))
            throw new KeywordNotAllowedException(firstNotAllowedPatchKeyword.Value, PatternType.Patch);

        if (HasNotAllowedKeyword(addonPattern, PatternKeywordHelper.GetAllowedAddonKeywords(), out var firstNotAllowedAddonKeyword))
            throw new KeywordNotAllowedException(firstNotAllowedAddonKeyword.Value, PatternType.Addon);
    }

    private static bool HasNotAllowedKeyword(IEnumerable<PatternPart> patternParts, IEnumerable<PatternKeyword> allowedKeywords, [NotNullWhen(true)] out PatternKeyword? firstNotAllowedKeyword)
    {
        firstNotAllowedKeyword = patternParts
            .OfType<DynamicTextPatternPart>()
            .Select(part => (PatternKeyword?)part.Keyword)
            .FirstOrDefault(keyword => !allowedKeywords.Contains(keyword!.Value));

        return firstNotAllowedKeyword != null;
    }

    private static void ValidateInvalidWindowsFileNameChar(string? invalidFileNameCharsReplacement)
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

    private async Task<RenamingResult> RenameFileAsyncInternalSafe(FileInfo inputFile, INamingSettings namingSettings, bool isSimulation, ILogger? logger, CancellationToken cancellationToken)
    {
        var oldFileName = inputFile.Name;
        var logPrefix = isSimulation ? $"{LocalizationManager.Instance.Current.Keys.RenamingTool_LogSimulationMode}" : "";

        try
        {
            var newFileName = await ComputeFileName(inputFile.FullName, namingSettings, cancellationToken);

            var shouldBeRenamed = !string.Equals(newFileName, oldFileName);
            if (!isSimulation && shouldBeRenamed)
            {
                var destFileName = Path.Combine(inputFile.DirectoryName!, newFileName);
                inputFile.MoveTo(destFileName, false);
            }

            if (shouldBeRenamed)
                logger?.LogWarning(LocalizationManager.Instance.Current.Keys.RenamingTool_LogFileRenamed.SafeFormat(logPrefix, oldFileName, newFileName));
            else
                logger?.LogInformation(LocalizationManager.Instance.Current.Keys.RenamingTool_LogFileAlreadyNamedProperly.SafeFormat(logPrefix, oldFileName));

            return new RenamingResult
            {
                IsSimulation = isSimulation,
                IsRenamed = shouldBeRenamed,
                OldFileName = oldFileName,
                NewFileName = newFileName,
            };
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, LocalizationManager.Instance.Current.Keys.RenamingTool_LogFailedToRenameFile.SafeFormat(logPrefix, oldFileName, ex.Message));

            return new RenamingResult
            {
                IsSimulation = isSimulation,
                IsRenamed = false,
                OldFileName = oldFileName,
                NewFileName = null,
                Exception = ex,
            };
        }
    }

    private async Task<string> ComputeFileName(string filePath, INamingSettings namingSettings, CancellationToken cancellationToken)
    {
        string newFileName;

        var packageInfo = _packageInfoLoader.GetPackageInfo(filePath);

        if (packageInfo.Contents.Count == 1)
        {
            var content = packageInfo.Contents[0];

            var patternParts = content.Type switch
            {
                ContentMetaType.Application => namingSettings.ApplicationPattern,
                ContentMetaType.Patch => namingSettings.PatchPattern,
                ContentMetaType.AddOnContent => namingSettings.AddonPattern,
                _ => throw new ContentTypeNotSupportedException(content.Type)
            };
            newFileName = await ComputePackageFileName(content, packageInfo.AccuratePackageType, patternParts, cancellationToken);
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

        return newFileName;
    }

    private async Task<string> ComputePackageFileName(Content content, AccuratePackageType accuratePackageType, IEnumerable<PatternPart> patternParts, CancellationToken cancellationToken)
    {
        var newFileName = "";

        foreach (var patternPart in patternParts)
        {
            cancellationToken.ThrowIfCancellationRequested();


            switch (patternPart)
            {
                case StaticTextPatternPart staticText:
                    newFileName += staticText.Text;
                    break;
                case DynamicTextPatternPart dynamicText:
                    string partValue;
                    switch (dynamicText.Keyword)
                    {
                        case PatternKeyword.TitleId:
                            partValue = content.TitleId;
                            break;
                        case PatternKeyword.ApplicationTitleId:
                            partValue = content.ApplicationTitleId;
                            break;
                        case PatternKeyword.PatchTitleId:
                            partValue = content.PatchTitleId;
                            break;
                        case PatternKeyword.TitleName:
                            var firstTitle = content.NacpData?.Titles.FirstOrDefault();
                            partValue = firstTitle != null ? firstTitle.Name : "NO_TITLE";
                            break;
                        case PatternKeyword.PackageType:
                            partValue = accuratePackageType.ToString();
                            break;
                        case PatternKeyword.VersionNumber:
                            partValue = content.Version.Version.ToString();
                            break;
                        case PatternKeyword.PatchNumber:
                            partValue = content.PatchNumber.ToString();
                            break;
                        case PatternKeyword.OnlineTitleName:
                            var onlineTitleInfo = await _cachedOnlineTitleInfoService.GetTitleInfoAsync(content.TitleId);
                            partValue = onlineTitleInfo != null ? onlineTitleInfo.Name : "NO_TITLE";
                            break;
                        case PatternKeyword.OnlineAppTitleName:
                            var onlineAppTitleInfo = await _cachedOnlineTitleInfoService.GetTitleInfoAsync(content.ApplicationTitleId);
                            partValue = onlineAppTitleInfo != null ? onlineAppTitleInfo.Name : "NO_TITLE";
                            break;

                        default:
                            throw new NotSupportedException($"Unknown application keyword «{dynamicText.Keyword}».");
                    }

                    switch (dynamicText.StringOperator)
                    {
                        case StringOperator.Untouched:
                            break;
                        case StringOperator.ToLower:
                            partValue = partValue.ToLower();
                            break;
                        case StringOperator.ToUpper:
                            partValue = partValue.ToUpper();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"Unknown operator «{dynamicText.StringOperator}».");
                    }

                    newFileName += partValue;

                    break;
                default:
                    throw new NotSupportedException($"Unknown part of type «{patternPart.GetType().Name}».");
            }
        }

        return newFileName;
    }


}