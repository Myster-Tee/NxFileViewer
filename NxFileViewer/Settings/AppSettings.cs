using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Settings.Model;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings
{
    public class AppSettings : IAppSettings
    {
        private readonly ILogger _logger;
        private AppSettingsModel _appSettings = new AppSettingsModel();

        private static readonly string _settingsFileName;

        static AppSettings()
        {
            _settingsFileName = $"{AppDomain.CurrentDomain.FriendlyName}.settings.json";
        }

        public AppSettings(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger(this.GetType());
        }

        public event SettingChangedHandler SettingChanged;

        public string LastOpenedFile
        {
            get => _appSettings.LastOpenedFile;
            set
            {
                _appSettings.LastOpenedFile = value;
                NotifySettingChanged(nameof(LastOpenedFile));
            }
        }

        public string ProdKeysFilePath
        {
            get => _appSettings.KeysFilePath;
            set
            {
                _appSettings.KeysFilePath = value;
                NotifySettingChanged(nameof(ProdKeysFilePath));
            }
        }

        public string ConsoleKeysFilePath
        {
            get => _appSettings.ConsoleKeysFilePath;
            set
            {
                _appSettings.ConsoleKeysFilePath = value;
                NotifySettingChanged(nameof(ConsoleKeysFilePath));
            }
        }

        public string TitleKeysFilePath
        {
            get => _appSettings.TitleKeysFilePath;
            set
            {
                _appSettings.TitleKeysFilePath = value;
                NotifySettingChanged(nameof(TitleKeysFilePath));
            }
        }

        public void Load()
        {
            if (!File.Exists(_settingsFileName))
                return;

            using var stream = File.OpenRead(_settingsFileName);
            _appSettings = JsonSerializer.DeserializeAsync<AppSettingsModel>(stream).Result;
        }

        public async Task Save()
        {
            try
            {
                await using var stream = File.Create(_settingsFileName);
                await JsonSerializer.SerializeAsync(stream, _appSettings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save settings.");
            }
        }

        protected virtual void NotifySettingChanged(string settingName)
        {
            SettingChanged?.Invoke(this, new SettingChangedHandlerArgs(settingName));
        }
    }
}
