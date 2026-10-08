using System.Windows.Controls;

namespace Emignatik.NxFileViewer.Views.Windows;

// Embedded workspace page; its view model survives navigation.
public partial class SettingsWindow : UserControl
{
    public SettingsWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => RefreshKeyLocations();
    }

    private void RefreshKeyLocations() => (DataContext as SettingsWindowViewModel)?.RefreshKeyLocations();
    private void KeyLocationsExpanded(object sender, System.Windows.RoutedEventArgs e) => RefreshKeyLocations();
}
