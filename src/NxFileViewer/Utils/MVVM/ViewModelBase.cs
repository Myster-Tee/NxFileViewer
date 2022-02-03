using System.ComponentModel;

namespace Emignatik.NxFileViewer.Utils.MVVM;

public abstract class ViewModelBase : NotifyPropertyChangedBase, IViewModelBase
{
}

public interface IViewModelBase : INotifyPropertyChanged
{
}