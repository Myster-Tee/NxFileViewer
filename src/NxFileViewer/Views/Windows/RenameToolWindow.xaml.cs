using System.Windows.Controls;
using System.Windows;

namespace Emignatik.NxFileViewer.Views.Windows;

// Embedded workspace page; its view model survives navigation.
public partial class RenameToolWindow : UserControl
{
    public RenameToolWindow() => InitializeComponent();
    private void OpenNamingSettings(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this) is MainWindow main) main.NavigateNamingSettings();
    }
}
