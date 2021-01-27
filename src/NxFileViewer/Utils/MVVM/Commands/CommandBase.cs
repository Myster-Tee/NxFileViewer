using System;
using System.Windows.Input;

namespace Emignatik.NxFileViewer.Utils.MVVM.Commands
{
    public abstract class CommandBase : ICommand
    {
        public virtual bool CanExecute(object? parameter)
        {
            return true;
        }

        public abstract void Execute(object? parameter);

        public event EventHandler? CanExecuteChanged;

        protected void TriggerCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
