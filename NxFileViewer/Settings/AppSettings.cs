using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Emignatik.NxFileViewer.Settings.Model;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings
{
    public class AppSettings : IAppSettings
    {
        private AppSettingsModel _appSettingsModel = new AppSettingsModel();

        public event SettingChangedHandler? SettingChanged;

        public string LastSaveDir
        {
            get => _appSettingsModel.LastSaveDir;
            set
            {
                _appSettingsModel.LastSaveDir = value;
                NotifySettingChanged();
            }
        }

        public string LastOpenedFile
        {
            get => _appSettingsModel.LastOpenedFile;
            set
            {
                _appSettingsModel.LastOpenedFile = value;
                NotifySettingChanged();
            }
        }

        public string ProdKeysFilePath
        {
            get => _appSettingsModel.KeysFilePath;
            set
            {
                _appSettingsModel.KeysFilePath = value;
                NotifySettingChanged();
            }
        }

        public string ConsoleKeysFilePath
        {
            get => _appSettingsModel.ConsoleKeysFilePath;
            set
            {
                _appSettingsModel.ConsoleKeysFilePath = value;
                NotifySettingChanged();
            }
        }

        public string TitleKeysFilePath
        {
            get => _appSettingsModel.TitleKeysFilePath;
            set
            {
                _appSettingsModel.TitleKeysFilePath = value;
                NotifySettingChanged();
            }
        }

        public LogLevel LogLevel
        {
            get => _appSettingsModel.LogLevel;
            set
            {
                _appSettingsModel.LogLevel = value;
                NotifySettingChanged();
            }
        }

        public string? ProdKeysDownloadUrl => _appSettingsModel.ProdKeysDownloadUrl;

        public void Update(AppSettingsModel newSettings)
        {
            _appSettingsModel = newSettings ?? throw new ArgumentNullException(nameof(newSettings));

            NotifyAllPropertiesChanged();
        }

        private void NotifyAllPropertiesChanged()
        {
            var properties = typeof(IAppSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                NotifySettingChanged(property.Name);
            }
        }

        protected virtual void NotifySettingChanged([CallerMemberName] string settingName = null!)
        {
            SettingChanged?.Invoke(this, new SettingChangedHandlerArgs(settingName));
        }
    }
}
