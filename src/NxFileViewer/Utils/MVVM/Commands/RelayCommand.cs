using System;

namespace Emignatik.NxFileViewer.Utils.MVVM.Commands
{
    public delegate void ExecuteNoArgHandler();
    public delegate bool CanExecuteNoArgHandler();

    public delegate void ExecuteHandler(object? parameter);
    public delegate bool CanExecuteHandler(object? parameter);

    public class RelayCommand : CommandBase
    {

        private readonly ExecuteHandler _execute;
        private readonly CanExecuteHandler? _canExecute;

        private static ExecuteHandler WrapExecute(ExecuteNoArgHandler executeNoArgHandler)
        {
            if (executeNoArgHandler == null) throw new ArgumentNullException(nameof(executeNoArgHandler));
            return parameter => executeNoArgHandler();
        }

        private static CanExecuteHandler? WrapCanExecute(CanExecuteNoArgHandler? canExecuteNoArgHandler)
        {
            if (canExecuteNoArgHandler == null)
                return null;
            return parameter => canExecuteNoArgHandler();
        }

        public RelayCommand(ExecuteNoArgHandler execute, CanExecuteNoArgHandler? canExecute = null) :
            this(WrapExecute(execute), WrapCanExecute(canExecute))
        {
        }

        public RelayCommand(ExecuteHandler execute, CanExecuteHandler? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }


        public override bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// <inheritdoc cref="CommandBase.TriggerCanExecuteChanged()"/>
        /// (exposes publicly the protected method <see cref="CommandBase.TriggerCanExecuteChanged()"/>)
        /// </summary>
        /// <param name="uiThreadSafe"></param>
        public new void TriggerCanExecuteChanged(bool uiThreadSafe = false)
        {
            base.TriggerCanExecuteChanged(uiThreadSafe);
        }
    }
}
