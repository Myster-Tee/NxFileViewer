using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Emignatik.NxFileViewer.Styling.Theme;

namespace Emignatik.NxFileViewer.Settings;

/// <summary>
/// Exposes the list of application settings
/// </summary>
public interface IAppSettings : INotifyPropertyChanged
{
    string NandExecutablePath { get; set; }
    string NandBisKeysPath { get; set; }
    bool CheckViewerUpdatesOnStartup { get; set; }
    bool IncludeViewerPrereleases { get; set; }
    string NszExecutablePath { get; set; }
    bool NszCheckUpdates { get; set; }
    int NszCompressionLevel { get; set; }
    NszCompressionMode NszCompressionMode { get; set; }
    int NszBlockSizeExponent { get; set; }

    bool EnableBatchHistory { get; set; }
    int LogFileRetentionCount { get; set; }
    string AppLanguage { get; set; }

    AppTheme Theme { get; set; }

    bool RememberWindowPlacement { get; set; }

    IWindowPlacementSettings MainWindowPlacement { get; }

    string LastUsedDir { get; set; }

    string LastOpenedFile { get; set; }

    string ProdKeysFilePath { get; set; }

    string ProdKeysDownloadUrl { get; set; }
    string KeysDownloadHost { get; set; }

    string TitleKeysFilePath { get; set; }

    string TitleKeysDownloadUrl { get; set; }

    bool OpenBlocklessCompressionNCZ { get; set; }

    bool IgnoreMissingDeltaFragments { get; set; }

    LogLevel LogLevel { get; set; }

    bool AlwaysReloadKeysBeforeOpen { get; set; }

    bool InjectTicketKeys { get; set; }
    bool SaveTicketKeys { get; set; }

    string TitlePageUrl { get; set; }

    string TitleInfoApiUrl { get; set; }
    TitleInfoProvider TitleInfoProvider { get; set; }
    string TitleDbRegion { get; set; }
    string NLibApiUrl { get; set; }

    IRenamingOptions RenamingOptions { get; }

    int ProgressBufferSize { get; }
}

public interface IRenamingOptions : INotifyPropertyChanged
{
    bool AutoCloseOpenedFile { get; set; }

    string LastRenamePath { get; set; }
    string TargetDirectory { get; set; }

    string? FileFilters { get; set; }

    bool IncludeSubdirectories { get; set; }

    string ApplicationPattern { get; set; }

    string PatchPattern { get; set; }

    string AddonPattern { get; set; }

    bool IsSimulation { get; set; }

    string InvalidFileNameCharsReplacement { get; set; }

    bool ReplaceWhiteSpaceChars { get; set; }

    string WhiteSpaceCharsReplacement { get; set; }

}

public interface IWindowPlacementSettings : INotifyPropertyChanged
{
    int Left { get; set; }
    int Top { get; set; }
    int Width { get; set; }
    int Height { get; set; }
    bool IsMaximized { get; set; }
    bool IsDefined { get; }
}
