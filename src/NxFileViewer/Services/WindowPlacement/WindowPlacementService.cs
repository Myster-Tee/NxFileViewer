using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.Logging;
using static Emignatik.NxFileViewer.Utils.Interop.NativeMethods;

namespace Emignatik.NxFileViewer.Services.WindowPlacement;

/// <summary>
/// Saves and restores the window placement using the Win32 window placement API.
/// All the coordinates handled here are physical pixels, which makes the whole thing
/// DPI and multi monitor friendly (no conversion from/to WPF device independent units).
/// </summary>
public class WindowPlacementService : IWindowPlacementService
{
    /// <summary>
    /// Minimum size (in pixels) kept when a restored window is adjusted to the work area
    /// </summary>
    private const int MIN_SIZE = 120;

    private readonly IAppSettings _appSettings;
    private readonly ILogger _logger;

    public WindowPlacementService(IAppSettings appSettings, ILoggerFactory loggerFactory)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
    }

    public void Track(Window window)
    {
        if (window == null)
            throw new ArgumentNullException(nameof(window));

        void OnSourceInitialized(object? sender, EventArgs args)
        {
            window.SourceInitialized -= OnSourceInitialized;
            Restore(window);
        }

        void OnClosing(object? sender, CancelEventArgs args)
        {
            Save(window);
        }

        if (window.IsInitialized && GetHandle(window) != IntPtr.Zero)
            Restore(window);
        else
            window.SourceInitialized += OnSourceInitialized;

        window.Closing += OnClosing;
    }

    public void Restore(Window window)
    {
        if (window == null)
            throw new ArgumentNullException(nameof(window));

        try
        {
            var handle = GetHandle(window);
            if (handle == IntPtr.Zero)
                return;

            var windowPlacement = new WINDOWPLACEMENT { Length = Marshal.SizeOf<WINDOWPLACEMENT>() };
            if (!GetWindowPlacement(handle, ref windowPlacement))
                return;

            var savedPlacement = _appSettings.MainWindowPlacement;
            var restoreSavedPlacement = _appSettings.RememberWindowPlacement && savedPlacement.IsDefined;

            // When nothing has been saved yet, the current placement of the window is kept
            // and simply adjusted to make sure it is entirely visible.
            var bounds = restoreSavedPlacement
                ? new RECT(savedPlacement.Left, savedPlacement.Top, savedPlacement.Left + savedPlacement.Width, savedPlacement.Top + savedPlacement.Height)
                : windowPlacement.NormalPosition;

            windowPlacement.Length = Marshal.SizeOf<WINDOWPLACEMENT>();
            windowPlacement.Flags = 0;
            windowPlacement.ShowCmd = SW_SHOWNORMAL;
            windowPlacement.MinPosition = new POINT(-1, -1);
            windowPlacement.MaxPosition = new POINT(-1, -1);
            windowPlacement.NormalPosition = EnsureVisible(bounds);

            if (!SetWindowPlacement(handle, ref windowPlacement))
                _logger.LogDebug("Window placement couldn't be restored.");

            if (ShouldStartMaximized(_appSettings))
                window.WindowState = WindowState.Maximized;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to restore the window placement: {message}", ex.Message);
        }
    }

    public static bool ShouldStartMaximized(IAppSettings settings) =>
        !settings.RememberWindowPlacement || !settings.MainWindowPlacement.IsDefined || settings.MainWindowPlacement.IsMaximized;

    public void Save(Window window)
    {
        if (window == null)
            throw new ArgumentNullException(nameof(window));

        try
        {
            if (!_appSettings.RememberWindowPlacement)
                return;

            var handle = GetHandle(window);
            if (handle == IntPtr.Zero)
                return;

            var windowPlacement = new WINDOWPLACEMENT { Length = Marshal.SizeOf<WINDOWPLACEMENT>() };
            if (!GetWindowPlacement(handle, ref windowPlacement))
                return;

            // NormalPosition is the restored (non maximized) position, which is exactly what has to be saved
            var normalPosition = windowPlacement.NormalPosition;
            if (normalPosition.Width <= 0 || normalPosition.Height <= 0)
                return;

            var savedPlacement = _appSettings.MainWindowPlacement;
            savedPlacement.Left = normalPosition.Left;
            savedPlacement.Top = normalPosition.Top;
            savedPlacement.Width = normalPosition.Width;
            savedPlacement.Height = normalPosition.Height;
            savedPlacement.IsMaximized = windowPlacement.ShowCmd == SW_SHOWMAXIMIZED || window.WindowState == WindowState.Maximized;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to save the window placement: {message}", ex.Message);
        }
    }

    /// <summary>
    /// Adjusts the given bounds to make sure the window fits in, and stays visible on,
    /// the work area of the closest connected monitor.
    /// This is what prevents the window from being spawned partially (or totally) outside
    /// of the desktop, typically after a monitor has been disconnected or its resolution changed.
    /// </summary>
    private static RECT EnsureVisible(RECT bounds)
    {
        var width = Math.Max(bounds.Width, MIN_SIZE);
        var height = Math.Max(bounds.Height, MIN_SIZE);

        if (!TryGetNearestWorkArea(bounds, out var workArea))
            return new RECT(bounds.Left, bounds.Top, bounds.Left + width, bounds.Top + height);

        // The window can't be bigger than the monitor work area
        width = Math.Min(width, workArea.Width);
        height = Math.Min(height, workArea.Height);

        var left = bounds.Left;
        var top = bounds.Top;

        if (left + width > workArea.Right)
            left = workArea.Right - width;
        if (left < workArea.Left)
            left = workArea.Left;

        if (top + height > workArea.Bottom)
            top = workArea.Bottom - height;
        if (top < workArea.Top)
            top = workArea.Top;

        return new RECT(left, top, left + width, top + height);
    }

    private static IntPtr GetHandle(Window window)
    {
        return new WindowInteropHelper(window).Handle;
    }
}
