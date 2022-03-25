using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Emignatik.NxFileViewer.Utils.MVVM.BindingExtensions;

internal class TextBoxExtension
{
    /// <summary>
    /// This attached DependencyProperty allows to refresh a TextBox Text binding to Source when Enter key is pressed.
    /// </summary>
    public static readonly DependencyProperty FlushOnEnterKeyPressedProperty = DependencyProperty.RegisterAttached(
        "FlushOnEnterKeyPressed", typeof(bool), typeof(TextBoxExtension), new PropertyMetadata(default(bool), PropertyChangedCallback));

    private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

        if (d is not TextBox textBox)
            throw new InvalidOperationException($"Attached property «{nameof(FlushOnEnterKeyPressedProperty)}» should be used on a «{nameof(TextBox)}».");

        var newValue = (bool)e.NewValue;
        if (newValue)
        {
            textBox.KeyDown += OnKeyDown;
        }
        else
        {
            textBox.KeyDown -= OnKeyDown;
        }
    }

    private static void OnKeyDown(object sender, KeyEventArgs e)
    {
        if(e.Key != Key.Enter)
            return;

        e.Handled = true;

        var textBox = (TextBox)sender;
        textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
    }

    public static void SetFlushOnEnterKeyPressed(DependencyObject element, bool value)
    {
        element.SetValue(FlushOnEnterKeyPressedProperty, value);
    }

    public static bool GetFlushOnEnterKeyPressed(DependencyObject element)
    {
        return (bool)element.GetValue(FlushOnEnterKeyPressedProperty);
    }

}