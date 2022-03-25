using System.Windows;
using System.Windows.Media;

namespace Emignatik.NxFileViewer.Utils.MVVM;

public static class DependencyObjectExtension
{
    public static Window? FindAttachedWindow(this DependencyObject dependencyObject)
    {
        var parent = dependencyObject;
        while (parent != null)
        {
            if (parent is Window window)
                return window;

            parent = VisualTreeHelper.GetParent(parent);
        }

        return null;
    }
}