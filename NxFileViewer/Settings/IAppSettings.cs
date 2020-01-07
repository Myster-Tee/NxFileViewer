using System.Threading.Tasks;

namespace Emignatik.NxFileViewer.Settings
{
    public interface IAppSettings
    {

        event SettingChangedHandler SettingChanged;

        string LastOpenedFile { get; set; }

        string KeysFilePath { get; set; }

        Task Save();

        void Load();
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