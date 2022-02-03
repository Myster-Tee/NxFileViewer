using System.Globalization;
using System.Windows;

namespace Emignatik.NxFileViewer.Utils.MVVM.Converters;

public class BoolToVisibilityConverter : ValueConverterBase<Visibility, bool>
{
    public Visibility TrueVisibility { get; set; }

    public Visibility FalseVisibility { get; set; }

    protected override Visibility ConvertForView(bool value, object parameter, CultureInfo culture)
    {
        return value ? TrueVisibility : FalseVisibility;
    }

    protected override bool ConvertForViewModel(Visibility value, object parameter, CultureInfo culture)
    {
        if (value == TrueVisibility)
            return true;
        if (value == FalseVisibility)
            return false;
        return default;
    }
}