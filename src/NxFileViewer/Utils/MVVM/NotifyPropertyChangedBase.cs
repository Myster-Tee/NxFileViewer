using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Emignatik.NxFileViewer.Utils.MVVM;

public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}