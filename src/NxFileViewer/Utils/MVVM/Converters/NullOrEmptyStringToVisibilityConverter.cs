using System;
using System.Globalization;
using System.Windows;

namespace Emignatik.NxFileViewer.Utils.MVVM.Converters;

public class NullOrEmptyStringToVisibilityConverter : ValueConverterBase<Visibility, string>
{
    public Visibility StringNullOrEmptyVisibility { get; set; } = Visibility.Collapsed;

    public Visibility StringNeitherNullNorEmptyVisibility { get; set; } = Visibility.Visible;

    protected override Visibility ConvertForView(string? value, object? parameter, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(value))
            return StringNullOrEmptyVisibility;
        else
            return StringNeitherNullNorEmptyVisibility;
    }

    protected override string? ConvertForViewModel(Visibility value, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}