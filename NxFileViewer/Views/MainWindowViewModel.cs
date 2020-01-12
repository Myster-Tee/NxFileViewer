using System;
using System.Linq;
using System.Reflection;
using System.Timers;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Logging;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.BindingExtensions.DragAndDrop;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Views
{
    public class MainWindowViewModel : ViewModelBase, IFilesDropped
    {
        private readonly ILogger _logger;
        private readonly IFileOpenerService _fileOpenerService;

        private OpenedFileViewModel? _openedFile;
        private readonly string _appNameAndVersion;
        private string _title;
        private readonly IOpenedFileService _openedFileService;
        private bool _errorAnimationEnabled;
        private readonly Timer _animationDurationTimer;

        public MainWindowViewModel(
            ILoggerFactory loggerFactory,
            IOpenedFileService openedFileService,
            IOpenFileCommand openFileCommand,
            IOpenLastFileCommand openLastFileCommand,
            ICloseFileCommand closeFileCommand,
            IExitAppCommand exitAppCommand,
            IShowSettingsViewCommand showSettingsViewCommand,
            IFileOpenerService fileOpenerService,
            IServiceProvider serviceProvider,
            ILogSource logSource)
        {
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
            _fileOpenerService = fileOpenerService ?? throw new ArgumentNullException(nameof(fileOpenerService));
            _openedFileService = openedFileService ?? throw new ArgumentNullException(nameof(openedFileService));
            OpenFileCommand = openFileCommand ?? throw new ArgumentNullException(nameof(openFileCommand));
            ExitAppCommand = exitAppCommand ?? throw new ArgumentNullException(nameof(exitAppCommand));
            ShowSettingsViewCommand = showSettingsViewCommand ?? throw new ArgumentNullException(nameof(showSettingsViewCommand));
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            LogSource = logSource ?? throw new ArgumentNullException(nameof(logSource));
            OpenLastFileCommand = openLastFileCommand ?? throw new ArgumentNullException(nameof(openLastFileCommand));
            CloseFileCommand = closeFileCommand ?? throw new ArgumentNullException(nameof(closeFileCommand));

            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            var assemblyVersion = (assemblyName.Version ?? new Version());
            _appNameAndVersion = $"{assemblyName.Name} v{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Revision}";

            UpdateTitle();
            _openedFileService.OpenedFileChanged += OnOpenedFileChanged;
            LogSource.Log += OnLog;

            _animationDurationTimer = new Timer(3000);
            _animationDurationTimer.Elapsed += OnAnimationDurationTimerElapsed;
        }

        public IServiceProvider ServiceProvider { get; }

        public IOpenFileCommand OpenFileCommand { get; }

        public IExitAppCommand ExitAppCommand { get; }

        public IShowSettingsViewCommand ShowSettingsViewCommand { get; }

        public IOpenLastFileCommand OpenLastFileCommand { get; }

        public ICloseFileCommand CloseFileCommand { get; }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyPropertyChanged();
            }
        }

        public OpenedFileViewModel? OpenedFile
        {
            get => _openedFile;
            set
            {
                _openedFile = value;
                NotifyPropertyChanged();
            }
        }

        public ILogSource LogSource { get; }

        public bool ErrorAnimationEnabled
        {
            get => _errorAnimationEnabled;
            set
            {
                _errorAnimationEnabled = value;
                NotifyPropertyChanged();
            }
        }

        private void OnAnimationDurationTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.ErrorAnimationEnabled = false;
        }

        private void OnLog(LogLevel logLevel, string message)
        {
            if (logLevel == LogLevel.Error)
            {
                this.ErrorAnimationEnabled = true;
                _animationDurationTimer.Stop();
                _animationDurationTimer.Start();
            }
        }

        private void OnOpenedFileChanged(object sender, OpenedFileChangedHandlerArgs args)
        {
            var newFile = args.NewFile;
            OpenedFile = newFile != null ? new OpenedFileViewModel(newFile, ServiceProvider) : null;
            UpdateTitle();
        }

        public void OnFilesDropped(string[] files)
        {
            if (files.Length >= 1)
            {
                if (files.Length > 1)
                    _logger.LogWarning(LocalizationManager.Instance.Current.Keys.MultipleFilesDragAndDropNotSupported);
                _fileOpenerService.OpenFile(files.First());
            }
        }

        private void UpdateTitle()
        {
            var openedFile = _openedFileService.OpenedFile;

            if (openedFile == null)
                Title = _appNameAndVersion;
            else
                Title = $"{_appNameAndVersion} - {openedFile.FileName}";
        }

    }
}
