using System.Windows.Media;


namespace Emignatik.NxFileViewer.Styling.Theme
{
    public interface IBrushesProvider
    {
        Brush FontBrushDefault { get; }

        Brush FontBrushSuccess { get; }

        Brush FontBrushWarning { get; }

        Brush FontBrushError { get; }
    }
}
