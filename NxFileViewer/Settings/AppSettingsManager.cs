using System;
using System.IO;
using System.Text.Json;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Settings.Model;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings
{
    public class AppSettingsManager : IAppSettingsManager
    {
        private static readonly string SettingsFileName;

        private readonly ILogger _logger;

        static AppSettingsManager()
        {
            SettingsFileName = $"{AppDomain.CurrentDomain.FriendlyName}.settings.json";
        }

        public AppSettingsManager(IAppSettings appSettings, ILoggerFactory loggerFactory)
        {
            Settings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
        }

        public IAppSettings Settings { get; }

        public void Load()
        {
            try
            {
                if (!File.Exists(SettingsFileName))
                    return;

                using var stream = File.OpenRead(SettingsFileName);
                var appSettings = JsonSerializer.DeserializeAsync<AppSettingsModel>(stream).Result;

                Settings.Update(appSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(LocalizationManager.Instance.Current.Keys.SettingsLoadingError,ex.Message));
            }
        }

        public void Save()
        {
            try
            {
                using var stream = File.Create(SettingsFileName);
                JsonSerializer.Serialize(new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }), Settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(LocalizationManager.Instance.Current.Keys.SettingsSavingError, ex.Message));
            }
        }
    }
}