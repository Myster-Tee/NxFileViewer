using System;
using System.Windows;

namespace Emignatik.NxFileViewer.Styling.Theme;

public interface IThemeService
{
    /// <summary>True when the dark theme is currently applied</summary>
    bool IsDarkTheme { get; }

    event Action? ThemeChanged;

    /// <summary>
    /// Applies the theme defined in the settings and keeps it synchronized with
    /// the settings and with the Windows app mode (when the System theme is selected).
    /// </summary>
    void Initialize();

    /// <summary>
    /// Applies the current theme to the given window (including its title bar).
    /// </summary>
    void RegisterWindow(Window window);
}
