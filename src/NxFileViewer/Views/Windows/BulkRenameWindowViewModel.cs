using Emignatik.NxFileViewer.Utils.MVVM;

namespace Emignatik.NxFileViewer.Views.Windows;

public class BulkRenameWindowViewModel : WindowViewModelBase
{
    private string _inputDirectory;
    private string _namingPattern;

    public string InputDirectory
    {
        get => _inputDirectory;
        set
        {
            _inputDirectory = value;
            NotifyPropertyChanged();
        }
    }

    public string NamingPattern
    {
        get => _namingPattern;
        set
        {
            _namingPattern = value;
            NotifyPropertyChanged();
        }
    }
}