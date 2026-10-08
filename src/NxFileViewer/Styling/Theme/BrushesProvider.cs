using System.Windows;
using System.Windows.Media;

namespace Emignatik.NxFileViewer.Styling.Theme
{
    /// <summary>
    /// Provides the themed brushes used from the code behind.
    /// The brushes are resolved on each access so that they always match the current theme.
    /// </summary>
    public class BrushesProvider : IBrushesProvider
    {
        public Brush FontBrushDefault => FindBrush("FontBrush.Default", Brushes.Black);

        public Brush FontBrushSuccess => FindBrush("FontBrush.Success", Brushes.ForestGreen);

        public Brush FontBrushWarning => FindBrush("FontBrush.Warning", Brushes.Orange);

        public Brush FontBrushError => FindBrush("FontBrush.Error", Brushes.Red);

        private static Brush FindBrush(string resourceKey, Brush fallbackBrush)
        {
            return Application.Current?.TryFindResource(resourceKey) as Brush ?? fallbackBrush;
        }
    }
}
