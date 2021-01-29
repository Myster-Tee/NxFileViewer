using System.ComponentModel;
using Emignatik.NxFileViewer.Settings.Model;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings
{
    public interface IAppSettings: INotifyPropertyChanged
    {
        string LastSaveDir { get; set; }

        string LastOpenedFile { get; set; }

        string ProdKeysFilePath { get; set; }

        string ConsoleKeysFilePath { get; set; }

        string TitleKeysFilePath { get; set; }

        LogLevel LogLevel { get; set; }

        string? ProdKeysDownloadUrl { get; }

        StructureLoadingMode StructureLoadingMode { get; set; }

        void Update(AppSettingsModel newSettings);
    }
}