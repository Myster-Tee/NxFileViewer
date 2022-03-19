using System.Windows;
using System.Windows.Controls;

namespace Emignatik.NxFileViewer.Views.CustomControls;

public class ProgressBarExt : ProgressBar
{

    public static readonly DependencyProperty ValueTextProperty = DependencyProperty.Register(
        "ValueText", typeof(string), typeof(ProgressBarExt), new PropertyMetadata(""));

    public string ValueText
    {
        get => (string) GetValue(ValueTextProperty);
        set => SetValue(ValueTextProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        "CornerRadius", typeof(CornerRadius), typeof(ProgressBarExt), new PropertyMetadata(new CornerRadius(0)));

    public CornerRadius CornerRadius
    {
        get => (CornerRadius) GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

}