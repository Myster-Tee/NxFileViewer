using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.NSP.Models;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Views.NSP;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Views
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private FileViewModelBase _fileViewModel;
        private readonly string _currentAppVersion;
        private string _appTitle;

        public MainWindowViewModel(IOpenedFileService openedFileService, ISupportedFilesOpenerService supportedFilesOpenerService, ILoggerFactory loggerFactory,
            OpenFileCommand openFileCommand,
            OpenLastFileCommand openLastFileCommand,
            CloseFileCommand closeFileCommand,
            ExitAppCommand exitAppCommand
            )
        {
            if (openedFileService == null) throw new ArgumentNullException(nameof(openedFileService));
            if (supportedFilesOpenerService == null) throw new ArgumentNullException(nameof(supportedFilesOpenerService));
            if (openFileCommand == null) throw new ArgumentNullException(nameof(openFileCommand));
            if (openLastFileCommand == null) throw new ArgumentNullException(nameof(openLastFileCommand));
            if (closeFileCommand == null) throw new ArgumentNullException(nameof(closeFileCommand));
            if (exitAppCommand == null) throw new ArgumentNullException(nameof(exitAppCommand));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger(this.GetType());

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            _currentAppVersion = $"{version.Major}.{version.Minor}.{version.Revision}";
            UpdateAppTitle();

            openedFileService.OpenedFileChanged += OnOpenedFileChanged;

            OpenFileCommand = openFileCommand;
            OpenLastFileCommand = openLastFileCommand;
            CloseFileCommand = closeFileCommand;
            ExitAppCommand = exitAppCommand;
        }

        public ICommand OpenFileCommand { get; }

        public ICommand OpenLastFileCommand { get; }

        public ICommand CloseFileCommand { get; }

        public ICommand ExitAppCommand { get; }

        /// <summary>
        /// Gets or sets the view model of the file being displayed
        /// </summary>
        public FileViewModelBase FileViewModel
        {
            get => _fileViewModel;
            set
            {
                _fileViewModel = value;
                UpdateAppTitle();
                NotifyPropertyChanged();
            }
        }

        public string AppTitle
        {
            get => _appTitle;
            set
            {
                _appTitle = value;
                NotifyPropertyChanged();
            }
        }

        private void OnOpenedFileChanged(object sender, OpenedFileChangedHandlerArgs args)
        {
            var newFile = args.NewFile;
            if (newFile != null)
            {
                if (newFile.FileData is NspInfo nspInfo)
                {
                    this.FileViewModel = new NspInfoViewModel(nspInfo, new FileViewModelFactory(), _loggerFactory);
                }
                else
                {
                    Debug.Assert(true, "File not supported");
                }
            }
            else
            {
                this.FileViewModel = null;
            }
        }

        private void UpdateAppTitle()
        {
            var fileViewModel = FileViewModel;

            var title = $"NX File Info v{_currentAppVersion}";

            var openedFilePath = fileViewModel?.Location;
            if (!string.IsNullOrEmpty(openedFilePath))
                title += $" - {Path.GetFileName(openedFilePath)}";

            AppTitle = title;
        }
    }
}
