using Emignatik.NxFileViewer.Settings.Model;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings
{
    public interface IAppSettings
    {
        event SettingChangedHandler SettingChanged;

        string LastSaveDir { get; set; }

        string LastOpenedFile { get; set; }

        string ProdKeysFilePath { get; set; }

        string ConsoleKeysFilePath { get; set; }

        string TitleKeysFilePath { get; set; }

        LogLevel LogLevel { get; set; }

        string? ProdKeysDownloadUrl { get; }

        void Update(AppSettingsModel newSettings);
    }

    public delegate void SettingChangedHandler(object sender, SettingChangedHandlerArgs args);

    public class SettingChangedHandlerArgs
    {
        public SettingChangedHandlerArgs(string settingName)
        {
            SettingName = settingName;
        }

        public string SettingName { get; }
    }
}