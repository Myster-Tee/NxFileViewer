using System;
using System.Windows;
using System.Windows.Input;

namespace Emignatik.NxFileViewer.Utils.MVVM.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null) :
            this(WrapExecute(execute), WrapCanExecute(canExecute))
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }


        private static Action<object> WrapExecute(Action execute)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            return o => { execute(); };
        }

        private static Predicate<object>? WrapCanExecute(Func<bool>? canExecute)
        {
            if (canExecute == null)
                return null;
            return o => canExecute();
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public virtual event EventHandler? CanExecuteChanged;

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public virtual void TriggerCanExecuteChanged(bool triggerUiThreadSafe = false)
        {
            var handler = CanExecuteChanged;
            if (handler == null)
                return;

            var triggerEventAction = new Action(() => { handler(this, new EventArgs()); });

            var uiDispatcher = Application.Current?.Dispatcher;
            if (triggerUiThreadSafe && uiDispatcher != null && !uiDispatcher.CheckAccess())
            {
                uiDispatcher.Invoke(triggerEventAction);
            }
            else
            {
                triggerEventAction();
            }
        }
    }
}
