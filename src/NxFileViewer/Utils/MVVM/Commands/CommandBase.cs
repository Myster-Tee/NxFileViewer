using System;
using System.Windows;
using System.Windows.Input;

namespace Emignatik.NxFileViewer.Utils.MVVM.Commands
{
    public abstract class CommandBase : ICommand
    {

        public CommandBase(bool uiThreadSafe = true)
        {
            UiThreadSafe = uiThreadSafe;
        }

        public bool UiThreadSafe { get; set; }

        public virtual bool CanExecute(object? parameter)
        {
            return true;
        }

        public abstract void Execute(object? parameter);

        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Triggers <see cref="CanExecuteChanged"/> event taking into account the <see cref="UiThreadSafe"/> boolean
        /// </summary>
        protected void TriggerCanExecuteChanged()
        {
            TriggerCanExecuteChanged(UiThreadSafe);
        }

        protected virtual void TriggerCanExecuteChanged(bool uiThreadSafe)
        {
            if (uiThreadSafe)
            {
                var application = Application.Current;
                if (application != null && !application.CheckAccess())
                {
                    application.Dispatcher.InvokeAsync(TriggerCanExecuteChangedInternal);
                    return;
                }
            }

            TriggerCanExecuteChangedInternal();
        }


        private void TriggerCanExecuteChangedInternal()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
