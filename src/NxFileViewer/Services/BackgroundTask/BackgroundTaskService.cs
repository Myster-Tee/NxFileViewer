using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Services.BackgroundTask
{
    public class BackgroundTaskService : ViewModelBase, IBackgroundTaskService, IProgressReporter
    {
        private bool _isIndeterminate;
        private readonly RelayCommand _cancelCommand;
        private CancellationTokenSource? _cancellationTokenSource;
        private IRunnableBase? _runningRunnable;
        private bool _isRunning;

        private string? _progressText;
        private double _progressValue;
        private string _progressValueText = "";

        public BackgroundTaskService()
        {
            _cancelCommand = new RelayCommand(OnCancel, CanCancel);
            Reset();
        }

        public ICommand CancelCommand => _cancelCommand;

        public string? ProgressText
        {
            get => _progressText;
            private set
            {
                _progressText = value;
                NotifyPropertyChanged();
            }
        }

        public double ProgressValue
        {
            get => _progressValue;
            private set
            {
                _progressValue = value;
                NotifyPropertyChanged();
            }
        }

        public string ProgressValueText
        {
            get => _progressValueText;
            private set
            {
                _progressValueText = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                _isRunning = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            private set
            {
                _isIndeterminate = value;
                NotifyPropertyChanged();
            }
        }

        public Task RunAsync(IRunnable runnable)
        {
            if (_runningRunnable != null)
                throw new InvalidOperationException(LocalizationManager.Instance.Current.Keys.ATaskIsAlreadyRunning);

            var runnableWrapper = new RunnableWrapper(runnable);

            return RunAsync(runnableWrapper);
        }

        /// <summary>
        /// Wraps a Runnable which doesn't return a value
        /// </summary>
        private class RunnableWrapper : IRunnable<object?>
        {
            private readonly IRunnable _realRunnable;

            public RunnableWrapper(IRunnable realRunnable)
            {
                _realRunnable = realRunnable ?? throw new ArgumentNullException(nameof(realRunnable));
            }

            public object? Run(IProgressReporter progressReporter, CancellationToken cancellationToken)
            {
                _realRunnable.Run(progressReporter, cancellationToken);
                return null;
            }

            public bool SupportsCancellation => _realRunnable.SupportsCancellation;
            public bool SupportProgress => _realRunnable.SupportProgress;
        }

        public async Task<T> RunAsync<T>(IRunnable<T> runnable)
        {
            if (_runningRunnable != null)
                throw new InvalidOperationException(LocalizationManager.Instance.Current.Keys.ATaskIsAlreadyRunning);

            _runningRunnable = runnable;
            _cancelCommand.TriggerCanExecuteChanged();

            IsRunning = true;
            ProgressText = "";
            IsIndeterminate = !runnable.SupportProgress;

            _cancellationTokenSource = new CancellationTokenSource();

            Task<T> task;
            try
            {
                task = Task.Run(() => runnable.Run(this, _cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                Reset(ex);
                throw;
            }

            try
            {
                var t = await task;
                Reset();
                return t;
            }
            catch (Exception ex)
            {
                Reset(ex);
                throw;
            }
        }

        private void Reset(Exception? runException = null)
        {
            _cancellationTokenSource = null;
            _runningRunnable = null;

            IsIndeterminate = false;
            ProgressText = LocalizationManager.Instance.Current.Keys.Ready;
            ProgressValue = 0;
            ProgressValueText = "";
            IsRunning = false;
            _cancelCommand.TriggerCanExecuteChanged();
        }

        private bool CanCancel()
        {
            return _runningRunnable != null && _runningRunnable.SupportsCancellation;
        }

        private void OnCancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        void IProgressReporter.SetMode(bool isIndeterminate)
        {
            IsIndeterminate = isIndeterminate;
        }

        void IProgressReporter.SetText(string text)
        {
            ProgressText = text;
        }

        void IProgressReporter.SetPercentage(double value)
        {
            var valueThresholded = Math.Min(Math.Max(0, value * 100), 100);
            ProgressValue = valueThresholded;
            ProgressValueText = $"{(int)Math.Round(valueThresholded)}%";
        }
    }
}
