using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Emignatik.NxFileViewer.Utils.MVVM.CustomControls;

public class ToggleButtonEx
{

    public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.RegisterAttached(
        "IsReadOnly", typeof(bool), typeof(ToggleButtonEx), new PropertyMetadata(default(bool), OnPropertyChanged));

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ToggleButton toggleButton)
            throw new InvalidOperationException($"This property may only be set on {nameof(ToggleButton)}.");

        if ((bool)e.NewValue)
        {
            toggleButton.Checked += OnCheckChanged;
            toggleButton.Unchecked += OnCheckChanged;
        }
        else
        {
            toggleButton.Checked -= OnCheckChanged;
            toggleButton.Unchecked -= OnCheckChanged;
        }
    }

    private static void OnCheckChanged(object sender, RoutedEventArgs e)
    {
        var checkedBinding = ((CheckBox)sender).GetBindingExpression(ToggleButton.IsCheckedProperty);
        checkedBinding?.UpdateTarget();
    }

    public static void SetIsReadOnly(DependencyObject element, bool value)
    {
        element.SetValue(IsReadOnlyProperty, value);
    }

    public static bool GetIsReadOnly(DependencyObject element)
    {
        return (bool)element.GetValue(IsReadOnlyProperty);
    }
}