using System;
using System.IO;
using System.Text.Json;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Settings.Models;
using Emignatik.NxFileViewer.Utils;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings
{
    public class AppSettingsManager : IAppSettingsManager
    {
        private static readonly string _settingsFilePath;

        private readonly ILogger _logger;
        private readonly AppSettingsWrapper _appSettingsWrapper;

        static AppSettingsManager()
        {
            _settingsFilePath = Path.Combine(PathHelper.CurrentAppDir, $"{AppDomain.CurrentDomain.FriendlyName}.settings.json");
        }

        public AppSettingsManager(ILoggerFactory loggerFactory, AppSettingsWrapper appSettingsWrapper)
        {
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
            _appSettingsWrapper = appSettingsWrapper ?? throw new ArgumentNullException(nameof(appSettingsWrapper));
        }

        public IAppSettings Settings => _appSettingsWrapper;

        public void SafeLoad()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                    return;

                var bytes = File.ReadAllBytes(_settingsFilePath);
                var appSettings = JsonSerializer.Deserialize<AppSettingsModel>(new ReadOnlySpan<byte>(bytes)) ?? new AppSettingsModel();
                _appSettingsWrapper.Update(appSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SettingsLoadingError.SafeFormat(ex.Message));
            }
        }

        public void SafeSave()
        {
            try
            {
                using var stream = File.Create(_settingsFilePath);
                JsonSerializer.Serialize(new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }), _appSettingsWrapper.Model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SettingsSavingError.SafeFormat(ex.Message));
            }
        }
    }
}