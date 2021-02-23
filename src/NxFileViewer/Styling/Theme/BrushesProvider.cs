using System.Windows;
using System.Windows.Media;

namespace Emignatik.NxFileViewer.Styling.Theme
{
    public class BrushesProvider : IBrushesProvider
    {
        public BrushesProvider()
        {
            FontBrushDefault = Application.Current.Resources["FontBrush.Normal"] as Brush ?? Brushes.Black;
            FontBrushSuccess = Application.Current.Resources["FontBrush.Success"] as Brush ?? Brushes.Black;
            FontBrushWarning = Application.Current.Resources["FontBrush.Warning"] as Brush ?? Brushes.Orange;
            FontBrushError = Application.Current.Resources["FontBrush.Error"] as Brush ?? Brushes.Red;
        }

        public Brush FontBrushDefault { get; }

        public Brush FontBrushSuccess { get; }

        public Brush FontBrushWarning { get; }

        public Brush FontBrushError { get; }

    }
}