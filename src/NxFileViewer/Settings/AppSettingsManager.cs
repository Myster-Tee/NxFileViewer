using System;
using System.IO;
using System.Text.Json;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services.GlobalEvents;
using Emignatik.NxFileViewer.Tools;
using Emignatik.NxFileViewer.Utils;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings
{
    public class AppSettingsManager : IAppSettingsManager
    {
        private static readonly string _settingsFilePath;

        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;
        private readonly IShallowCopier _shallowCopier;

        static AppSettingsManager()
        {
            _settingsFilePath = Path.Combine(PathHelper.CurrentAppDir, $"{AppDomain.CurrentDomain.FriendlyName}.settings.json");
        }

        public AppSettingsManager(ILoggerFactory loggerFactory, AppSettings appSettings, IAppEvents appEvents, IShallowCopier shallowCopier)
        {
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _shallowCopier = shallowCopier ?? throw new ArgumentNullException(nameof(shallowCopier));
            (appEvents ?? throw new ArgumentNullException(nameof(appEvents))).AppShuttingDown += OnAppShuttingDown;

            RestoreDefaultSettings();
        }

        public IAppSettings Settings => _appSettings;

        public IAppSettings Clone()
        {
            var clone = new AppSettings();
            _shallowCopier.Copy(this.Settings, clone);
            return clone;
        }

        public void RestoreDefaultSettings()
        {
            _shallowCopier.Copy(new AppSettings(), _appSettings);
        }

        public bool LoadSafe()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                    return false;

                var bytes = File.ReadAllBytes(_settingsFilePath);
                var settingsModel = JsonSerializer.Deserialize<AppSettings>(new ReadOnlySpan<byte>(bytes));
                if (settingsModel == null)
                    return false;

                _shallowCopier.Copy(settingsModel, _appSettings);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SettingsLoadingError.SafeFormat(ex.Message));
                return false;
            }
        }


        public void SaveSafe()
        {
            try
            {
                using var stream = File.Create(_settingsFilePath);

                JsonSerializer.Serialize(new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }), _appSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SettingsSavingError.SafeFormat(ex.Message));
            }
        }

        private void OnAppShuttingDown()
        {
            SaveSafe();
        }
    }
}