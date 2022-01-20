using System;
using System.ComponentModel;
using System.Linq;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Localization;

public class LocalizationFromSettingsSynchronizerService : ILocalizationFromSettingsSynchronizerService
{
    private readonly IAppSettings _appSettings;
    private readonly ILogger _logger;

    public LocalizationFromSettingsSynchronizerService(IAppSettings appSettings, ILoggerFactory loggerFactory)
    {
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
        _appSettings = appSettings;
    }

    private void OnSettingChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(IAppSettings.AppLanguage))
            return;

        UpdateFromSettings();
    }

    private void UpdateFromSettings()
    {
        var appLanguage = _appSettings.AppLanguage;
        var newLocalization = LocalizationManager.Instance.AvailableLocalizations.FirstOrDefault(localization => localization.CultureName == appLanguage);
        if (newLocalization == null)
        {
            _logger.LogWarning(LocalizationManager.Instance.Current.Keys.InvalidSetting_LanguageNotFound.SafeFormat(appLanguage));
        }
        else
        {
            LocalizationManager.Instance.Current = newLocalization;
        }
    }

    public void Initialize()
    {
        _appSettings.PropertyChanged -= OnSettingChanged;
        _appSettings.PropertyChanged += OnSettingChanged;

        UpdateFromSettings();
    }

}