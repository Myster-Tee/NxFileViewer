using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings;

public interface IAppSettings: INotifyPropertyChanged
{
    string? AppLanguage { get; set; }

    string LastUsedDir { get; set; }

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

    string? RenamingFileFilters { get; set; }

    bool RenameIncludeSubdirectories { get; set; }

    int ProgressBufferSize { get; set; }
}