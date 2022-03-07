using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings
{
    public interface IAppSettings: INotifyPropertyChanged
    {
        string? AppLanguage { get; set; }

        string LastSaveDir { get; set; }

        string LastOpenedFile { get; set; }

        string ProdKeysFilePath { get; set; }

        string ProdKeysDownloadUrl { get; set; }

        string TitleKeysFilePath { get; set; }

        string TitleKeysDownloadUrl { get; set; }

        string ConsoleKeysFilePath { get; set; }

        LogLevel LogLevel { get; set; }

        bool AlwaysReloadKeysBeforeOpen { get; set; }

        string TitlePageUrl { get; set; }

        int ProgressBufferSize { get; set; }
    }
}