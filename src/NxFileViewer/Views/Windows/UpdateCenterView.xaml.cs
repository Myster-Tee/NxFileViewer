using System.Windows.Controls;

namespace Emignatik.NxFileViewer.Views.Windows;

public partial class UpdateCenterView : UserControl
{
    public UpdateCenterView()
    {
        InitializeComponent();
        Loaded += (_, _) => { if (DataContext is UpdateCenterViewModel model) model.RefreshInstalledData(); };
    }
}
