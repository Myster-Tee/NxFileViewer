using System.Windows;

namespace Emignatik.NxFileViewer.Utils.MVVM;

public abstract class WindowViewModelBase : ViewModelBase, IWindowViewModelBase
{
    public Window? Window { get; set; }
}

public interface IWindowViewModelBase : IViewModelBase
{
    Window? Window { get; }
}