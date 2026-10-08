using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services.Integrity;
using Emignatik.NxFileViewer.Services.Nsz;

namespace Emignatik.NxFileViewer.Views.Windows;

// Embedded workspace page; its view model survives navigation.
public partial class BatchIntegrityWindow : UserControl
{
    public BatchIntegrityWindow() => InitializeComponent();

    private void OpenFileActions(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DataGridRow { Item: BatchIntegrityResult file } row ||
            DataContext is not BatchIntegrityWindowViewModel model) return;
        ResultsGrid.SelectedItem = file;
        var keys = LocalizationManager.Instance.Current.Keys;
        var menu = new ContextMenu { PlacementTarget = row };
        void Add(string title, bool enabled, System.Action action)
        {
            var item = new MenuItem { Header = title, IsEnabled = enabled };
            item.Click += (_, _) => action();
            menu.Items.Add(item);
        }
        Add(keys.BatchIntegrity_OpenSelected, model.CanOpenInSingle(file), () => model.OpenInSingle(file));
        Add(keys.Nsz_Compress, model.CanActOnFile(file, NszOperation.Compress), () => model.ConvertSelected(file, NszOperation.Compress));
        Add(keys.Nsz_Decompress, model.CanActOnFile(file, NszOperation.Decompress), () => model.ConvertSelected(file, NszOperation.Decompress));
        Add(keys.MenuItem_OpenTitleWebPage, model.CanOpenTitle(file), () => model.OpenSelectedTitle(file));
        Add(keys.MenuItem_CheckIntegrity, !file.IsNand && !model.BackgroundTask.IsRunning &&
            (Directory.Exists(file.FilePath) || File.Exists(PackageZip.ArchivePath(file.FilePath))), () => model.VerifySelected(file));
        Add(keys.BatchNaming_Check, model.CanCheckNaming(file), () => model.CheckSelectedNaming(file));
        Add(keys.RenamingTool_Button_Rename, model.CanCheckNaming(file), () => model.RenameSelected(file));
        Add(keys.BatchIntegrity_MoveSelected, model.CanActOnFile(file), () => model.MoveSelected(file));
        row.ContextMenu = menu;
        menu.IsOpen = true;
        e.Handled = true;
    }

    private void OpenColumns(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        button.ContextMenu = CreateColumnsMenu(button);
        button.ContextMenu.IsOpen = true;
    }

    private ContextMenu CreateColumnsMenu(Button button)
    {
        var menu = new ContextMenu { PlacementTarget = button };
        foreach (var column in ResultsGrid.Columns)
        {
            var item = new MenuItem { IsCheckable = true, IsChecked = column.Visibility == Visibility.Visible, StaysOpenOnClick = true };
            item.SetBinding(HeaderedItemsControl.HeaderProperty, new Binding(nameof(DataGridColumn.Header)) { Source = column });
            item.Click += (_, _) => column.Visibility = item.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            menu.Items.Add(item);
        }
        return menu;
    }

    private void ScrollTable(object sender, MouseWheelEventArgs e)
    {
        if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0) return;
        var scroll = FindScrollViewer(ResultsGrid);
        if (scroll == null) return;
        scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset - e.Delta);
        e.Handled = true;
    }
    private static ScrollViewer? FindScrollViewer(DependencyObject root)
    {
        if (root is ScrollViewer scroll) return scroll;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            if (FindScrollViewer(VisualTreeHelper.GetChild(root, i)) is { } found) return found;
        return null;
    }
    private void ResetSort(object sender, RoutedEventArgs e)
    {
        ResultsGrid.Items.SortDescriptions.Clear();
        foreach (var column in ResultsGrid.Columns) column.SortDirection = null;
    }
}
