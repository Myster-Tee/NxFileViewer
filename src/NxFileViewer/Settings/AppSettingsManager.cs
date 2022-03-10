using System;
using System.IO;
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
        private readonly IAppSettings _appSettings;

        static AppSettingsManager()
        {
            _settingsFilePath = Path.Combine(PathHelper.CurrentAppDir, $"{AppDomain.CurrentDomain.FriendlyName}.settings.json");
        }

        public AppSettingsManager(ILoggerFactory loggerFactory, IAppSettings appSettings)
        {
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

            LoadDefault();
        }

        public IAppSettings Settings => _appSettings;

        public void LoadDefault()
        {
            LoadFromModel(new SettingsModel());
        }

        public bool LoadSafe()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                    return false;

                var bytes = File.ReadAllBytes(_settingsFilePath);
                var settingsModel = JsonSerializer.Deserialize<SettingsModel>(new ReadOnlySpan<byte>(bytes)) ?? new SettingsModel();

                LoadFromModel(settingsModel);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SettingsLoadingError.SafeFormat(ex.Message));
                return false;
            }
        }

        private void LoadFromModel(SettingsModel model)
        {
            if (model.AppLanguage != null)
                _appSettings.AppLanguage = model.AppLanguage;
            if (model.LastUsedDir != null)
                _appSettings.LastUsedDir = model.LastUsedDir;
            if (model.LastOpenedFile != null)
                _appSettings.LastOpenedFile = model.LastOpenedFile;
            if (model.ProdKeysFilePath != null)
                _appSettings.ProdKeysFilePath = model.ProdKeysFilePath;
            if (model.ProdKeysDownloadUrl != null)
                _appSettings.ProdKeysDownloadUrl = model.ProdKeysDownloadUrl;
            if (model.TitleKeysFilePath != null)
                _appSettings.TitleKeysFilePath = model.TitleKeysFilePath;
            if (model.TitleKeysDownloadUrl != null)
                _appSettings.TitleKeysDownloadUrl = model.TitleKeysDownloadUrl;
            if (model.ConsoleKeysFilePath != null)
                _appSettings.ConsoleKeysFilePath = model.ConsoleKeysFilePath;
            if (model.LogLevel != null)
                _appSettings.LogLevel = model.LogLevel.Value;
            if (model.AlwaysReloadKeysBeforeOpen != null)
                _appSettings.AlwaysReloadKeysBeforeOpen = model.AlwaysReloadKeysBeforeOpen.Value;
            if (model.TitlePageUrl != null)
                _appSettings.TitlePageUrl = model.TitlePageUrl;
            if (model.ApplicationPattern != null)
                _appSettings.ApplicationPattern = model.ApplicationPattern;    
            if (model.PatchPattern != null)
                _appSettings.PatchPattern = model.PatchPattern;       
            if (model.AddonPattern != null)
                _appSettings.AddonPattern = model.AddonPattern;
        }

        public void SaveSafe()
        {
            try
            {
                using var stream = File.Create(_settingsFilePath);
                var appSettings = this.Settings;
                var settingsModel = new SettingsModel
                {
                    AppLanguage = appSettings.AppLanguage,
                    LastUsedDir = appSettings.LastUsedDir,
                    LastOpenedFile = appSettings.LastOpenedFile,
                    ProdKeysFilePath = appSettings.ProdKeysFilePath,
                    ProdKeysDownloadUrl = appSettings.ProdKeysDownloadUrl,
                    TitleKeysFilePath = appSettings.TitleKeysFilePath,
                    TitleKeysDownloadUrl = appSettings.TitleKeysDownloadUrl,
                    ConsoleKeysFilePath = appSettings.ConsoleKeysFilePath,
                    LogLevel = appSettings.LogLevel,
                    AlwaysReloadKeysBeforeOpen = appSettings.AlwaysReloadKeysBeforeOpen,
                    TitlePageUrl = appSettings.TitlePageUrl,
                    ApplicationPattern = appSettings.ApplicationPattern,
                    PatchPattern = appSettings.PatchPattern,
                    AddonPattern = appSettings.AddonPattern,
                };

                JsonSerializer.Serialize(new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }), settingsModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SettingsSavingError.SafeFormat(ex.Message));
            }
        }
    }
}