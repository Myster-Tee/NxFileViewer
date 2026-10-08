using System;
namespace Emignatik.NxFileViewer.Styling.Theme;

// Theme events must not keep cached or closed file views alive.
internal static class ThemeObserver
{
    public static void Observe<T>(IThemeService? theme, T target, Action<T> refresh) where T : class
    {
        if (theme == null) return;
        var weak = new WeakReference<T>(target);
        Action? handler = null;
        handler = () =>
        {
            if (weak.TryGetTarget(out var current)) refresh(current);
            else theme.ThemeChanged -= handler;
        };
        theme.ThemeChanged += handler;
    }
}