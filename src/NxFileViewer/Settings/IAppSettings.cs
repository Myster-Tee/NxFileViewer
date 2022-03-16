using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings;

/// <summary>
/// Exposes the list of application settings
/// </summary>
public interface IAppSettings : INotifyPropertyChanged
{

    string AppLanguage { get; set; }

    string LastUsedDir { get; set; }

    string LastRenamePath { get; set; }

    string LastOpenedFile { get; set; }

    string ProdKeysFilePath { get; set; }

    string ProdKeysDownloadUrl { get; set; }

    string TitleKeysFilePath { get; set; }

    string TitleKeysDownloadUrl { get; set; }

    string ConsoleKeysFilePath { get; set; }

    LogLevel LogLevel { get; set; }

    bool AlwaysReloadKeysBeforeOpen { get; set; }

    string TitlePageUrl { get; set; }

    string ApplicationPattern { get; set; }

    string PatchPattern { get; set; }

    string AddonPattern { get; set; }

    string TitleInfoApiUrl { get; set; }

    IRenamingOptions RenamingOptions { get; }

    int ProgressBufferSize { get; }
}

public interface IRenamingOptions : INotifyPropertyChanged
{
    string? FileFilters { get; set; }

    bool IncludeSubdirectories { get; set; }

    bool IsSimulation { get; set; }

    string InvalidFileNameCharsReplacement { get; set; }

    bool ReplaceWhiteSpaceChars { get; set; }

    string WhiteSpaceCharsReplacement { get; set; }
}