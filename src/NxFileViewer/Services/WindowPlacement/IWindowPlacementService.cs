using System.Windows;

namespace Emignatik.NxFileViewer.Services.WindowPlacement;

/// <summary>
/// Restores and saves the position and the size of the main window.
/// </summary>
public interface IWindowPlacementService
{
    /// <summary>
    /// Starts tracking the given window: its placement is restored when the window is
    /// initialized and saved back to the settings when the window is closed.
    /// The restored placement is always adjusted to remain visible on one of the
    /// currently connected monitors.
    /// </summary>
    void Track(Window window);

    /// <summary>
    /// Saves the current placement of the given window into the application settings.
    /// </summary>
    void Save(Window window);

    /// <summary>
    /// Restores the placement of the given window from the application settings.
    /// </summary>
    void Restore(Window window);
}
