using System;
using System.IO;
using System.Text.Json;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Settings.Model;
using Emignatik.NxFileViewer.Utils;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings
{
    public class AppSettingsManager : IAppSettingsManager
    {
        private static readonly string SettingsFilePath;

        private readonly ILogger _logger;
        private readonly IAppSettingsWrapper<AppSettingsModel> _appSettingsWrapper;

        static AppSettingsManager()
        {
            SettingsFilePath = Path.Combine(PathHelper.CurrentAppDir, $"{AppDomain.CurrentDomain.FriendlyName}.settings.json");
        }

        public AppSettingsManager(ILoggerFactory loggerFactory, IAppSettingsWrapper<AppSettingsModel> appSettingsWrapper)
        {
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
            _appSettingsWrapper = appSettingsWrapper ?? throw new ArgumentNullException(nameof(appSettingsWrapper));
        }

        public IAppSettings Settings => _appSettingsWrapper;

        public void Load()
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                    return;

                var bytes = File.ReadAllBytes(SettingsFilePath);
                var appSettings = JsonSerializer.Deserialize<AppSettingsModel>(new ReadOnlySpan<byte>(bytes)) ?? new AppSettingsModel();
                _appSettingsWrapper.Update(appSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SettingsLoadingError.SafeFormat(ex.Message));
            }
        }

        public void Save()
        {
            try
            {
                using var stream = File.Create(SettingsFilePath);
                JsonSerializer.Serialize(new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }), _appSettingsWrapper.WrappedModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SettingsSavingError.SafeFormat(ex.Message));
            }
        }
    }
}