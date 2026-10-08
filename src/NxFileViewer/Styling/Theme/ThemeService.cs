using System;
using System.Windows;
using System.Windows.Interop;
using Emignatik.NxFileViewer.Services.GlobalEvents;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.Interop;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace Emignatik.NxFileViewer.Styling.Theme;

/// <summary>
/// Applies the light or the dark theme by swapping the theme resource dictionary
/// merged in the application resources.
/// All the themed resources must be referenced with DynamicResource for the switch to be live.
/// </summary>
public class ThemeService : IThemeService
{
    private const string LIGHT_THEME_SOURCE = "Styling/Theme/LightTheme.xaml";
    private const string DARK_THEME_SOURCE = "Styling/Theme/DarkTheme.xaml";
    private const string PERSONALIZE_KEY = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string APPS_USE_LIGHT_THEME_VALUE = "AppsUseLightTheme";

    private readonly IAppSettings _appSettings;
    private readonly ILogger _logger;
    private readonly IAppEvents _appEvents;
    private bool _initialized;

    public ThemeService(IAppSettings appSettings, IAppEvents appEvents, ILoggerFactory loggerFactory)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _appEvents = appEvents ?? throw new ArgumentNullException(nameof(appEvents));
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
    }

    public bool IsDarkTheme { get; private set; }

    public event Action? ThemeChanged;

    public void Initialize()
    {
        if (_initialized)
            return;
        _initialized = true;

        _appSettings.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(IAppSettings.Theme))
                ApplyFromSettings();
        };

        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        _appEvents.AppShuttingDown += () => SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;

        ApplyFromSettings();
    }

    public void RegisterWindow(Window window)
    {
        if (window == null)
            throw new ArgumentNullException(nameof(window));

        void ApplyTitleBar(object? sender, EventArgs args) => ApplyTitleBarTheme(window);

        window.SourceInitialized += ApplyTitleBar;
        window.Closed += (_, _) => window.SourceInitialized -= ApplyTitleBar;

        ApplyTitleBarTheme(window);
    }

    private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs args)
    {
        if (args.Category != UserPreferenceCategory.General)
            return;

        // Raised from a system thread, the theme has to be applied on the UI thread
        Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
        {
            if (_appSettings.Theme == AppTheme.System)
                ApplyFromSettings();
        }));
    }

    private void ApplyFromSettings()
    {
        var theme = _appSettings.Theme;
        var useDarkTheme = theme == AppTheme.System ? IsWindowsUsingDarkMode() : theme == AppTheme.Dark;
        Apply(useDarkTheme);
    }

    private void Apply(bool useDarkTheme)
    {
        var application = Application.Current;
        if (application == null)
            return;

        var mergedDictionaries = application.Resources.MergedDictionaries;
        var newThemeDictionary = new ResourceDictionary { Source = new Uri(useDarkTheme ? DARK_THEME_SOURCE : LIGHT_THEME_SOURCE, UriKind.Relative) };

        var themeDictionaryIndex = -1;
        for (var i = 0; i < mergedDictionaries.Count; i++)
        {
            var source = mergedDictionaries[i].Source?.OriginalString;
            if (source == null)
                continue;
            if (source.EndsWith("LightTheme.xaml", StringComparison.OrdinalIgnoreCase) || source.EndsWith("DarkTheme.xaml", StringComparison.OrdinalIgnoreCase))
            {
                themeDictionaryIndex = i;
                break;
            }
        }

        if (themeDictionaryIndex < 0)
            mergedDictionaries.Insert(0, newThemeDictionary);
        else
            mergedDictionaries[themeDictionaryIndex] = newThemeDictionary;

        IsDarkTheme = useDarkTheme;

        foreach (Window window in application.Windows)
        {
            ApplyTitleBarTheme(window);
        }

        ThemeChanged?.Invoke();
    }

    private void ApplyTitleBarTheme(Window window)
    {
        try
        {
            NativeMethods.SetWindowDarkTitleBar(new WindowInteropHelper(window).Handle, IsDarkTheme);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to apply the theme to the window title bar: {message}", ex.Message);
        }
    }

    /// <summary>
    /// Returns true when Windows is configured to display the applications in dark mode
    /// </summary>
    private bool IsWindowsUsingDarkMode()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(PERSONALIZE_KEY);
            return key?.GetValue(APPS_USE_LIGHT_THEME_VALUE) is int appsUseLightTheme && appsUseLightTheme == 0;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to read the Windows app mode: {message}", ex.Message);
            return false;
        }
    }
}
