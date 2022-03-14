using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Utils;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings
{
    public class AppSettingsManager : IAppSettingsManager
    {
        private static readonly string _settingsFilePath;

        private readonly ILogger _logger;
        private readonly IAppSettingsWrapper _appSettingsWrapper;

        static AppSettingsManager()
        {
            _settingsFilePath = Path.Combine(PathHelper.CurrentAppDir, $"{AppDomain.CurrentDomain.FriendlyName}.settings.json");
        }

        public AppSettingsManager(ILoggerFactory loggerFactory, IAppSettingsWrapper appSettingsWrapper)
        {
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
            _appSettingsWrapper = appSettingsWrapper ?? throw new ArgumentNullException(nameof(appSettingsWrapper));

            LoadDefault();
        }

        public IAppSettings Settings => _appSettingsWrapper;

        public void LoadDefault()
        {
            _appSettingsWrapper.SerializedModel = new SerializeSettings();
        }

        public bool LoadSafe()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                    return false;

                var bytes = File.ReadAllBytes(_settingsFilePath);
                var settingsModel = JsonSerializer.Deserialize<SerializeSettings>(new ReadOnlySpan<byte>(bytes));
                if (settingsModel == null)
                    return false;

                _appSettingsWrapper.SerializedModel = settingsModel;

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

                JsonSerializer.Serialize(new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }), _appSettingsWrapper.SerializedModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SettingsSavingError.SafeFormat(ex.Message));
            }
        }
    }
}