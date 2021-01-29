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
        private readonly IAppSettingsWrapper<AppSettingsModel> _appSettingsWrapper;

        static AppSettingsManager()
        {
            SettingsFileName = $"{AppDomain.CurrentDomain.FriendlyName}.settings.json";
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
                if (!File.Exists(SettingsFileName))
                    return;

                var bytes = File.ReadAllBytes(SettingsFileName);
                var appSettings = JsonSerializer.Deserialize<AppSettingsModel>(new ReadOnlySpan<byte>(bytes)) ?? new AppSettingsModel();
                _appSettingsWrapper.Update(appSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(LocalizationManager.Instance.Current.Keys.SettingsLoadingError, ex.Message));
            }
        }

        public void Save()
        {
            try
            {
                using var stream = File.Create(SettingsFileName);
                JsonSerializer.Serialize(new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }), _appSettingsWrapper.WrappedModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(LocalizationManager.Instance.Current.Keys.SettingsSavingError, ex.Message));
            }
        }
    }
}