using System.Windows;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Styling.Theme;
using Microsoft.Extensions.DependencyInjection;
namespace Emignatik.NxFileViewer.Views.Windows;
public static class ThemedDialog {
    public static MessageBoxResult Confirm(string title, string message, bool allowCancel = true, bool allowNo = true)
    {
        var keys = LocalizationManager.Instance.Current.Keys;
        var result = MessageBoxResult.Cancel;
        var panel = new System.Windows.Controls.StackPanel { Margin = new Thickness(20) };
        var dialog = new Window { Title = title, Content = panel, Width = 650,
            SizeToContent = SizeToContent.Height, ResizeMode = ResizeMode.NoResize,
            WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = Application.Current.MainWindow };
        dialog.Style = Application.Current.TryFindResource("WindowStyle") as Style;
        panel.Children.Add(new System.Windows.Controls.ScrollViewer { MaxHeight = 400,
            VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
            Margin = new Thickness(0, 0, 0, 20),
            Content = new System.Windows.Controls.TextBlock { Text = message, TextWrapping = TextWrapping.Wrap } });
        var buttons = new System.Windows.Controls.StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right };
        panel.Children.Add(buttons);
        foreach (var option in new[] { (keys.Dialog_Yes, MessageBoxResult.Yes),
            (keys.Dialog_No, MessageBoxResult.No), (keys.Nsz_Cancel, MessageBoxResult.Cancel) })
        {
            if (!allowCancel && option.Item2 == MessageBoxResult.Cancel) continue;
            if (!allowNo && option.Item2 == MessageBoxResult.No) continue;
            var button = new System.Windows.Controls.Button { Content = option.Item1, MinWidth = 90,
                Padding = new Thickness(12, 5, 12, 5), Margin = new Thickness(8, 0, 0, 0),
                IsDefault = option.Item2 == MessageBoxResult.No, IsCancel = option.Item2 == MessageBoxResult.Cancel };
            button.Click += (_, _) => { result = option.Item2; dialog.Close(); };
            buttons.Children.Add(button);
        }
        App.ServiceProvider.GetRequiredService<IThemeService>().RegisterWindow(dialog);
        dialog.ShowDialog();
        return result;
    }
    public static void Notice(string message, string title, MessageBoxImage image)
    {
        var panel = new System.Windows.Controls.StackPanel { Margin = new Thickness(20) };
        var dialog = new Window { Title = title, Content = panel, Width = 650,
            SizeToContent = SizeToContent.Height, ResizeMode = ResizeMode.NoResize,
            WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = Application.Current.MainWindow };
        dialog.Style = Application.Current.TryFindResource("WindowStyle") as Style;
        var text = new System.Windows.Controls.TextBlock { Text = message, TextWrapping = TextWrapping.Wrap };
        text.SetResourceReference(System.Windows.Controls.TextBlock.ForegroundProperty,
            image == MessageBoxImage.Error ? "FontBrush.Error" : image == MessageBoxImage.Warning ? "FontBrush.Warning" : "FontBrush.Default");
        panel.Children.Add(new System.Windows.Controls.ScrollViewer { Content = text, MaxHeight = 400,
            VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto });
        var ok = new System.Windows.Controls.Button { Content = "OK", IsDefault = true, IsCancel = true,
            MinWidth = 90, Padding = new Thickness(12, 5, 12, 5), HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 20, 0, 0) };
        ok.Click += (_, _) => dialog.Close();
        panel.Children.Add(ok);
        App.ServiceProvider.GetRequiredService<IThemeService>().RegisterWindow(dialog);
        dialog.ShowDialog();
    }
}
