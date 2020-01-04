using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.NSP.Models;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Views.NSP;

namespace Emignatik.NxFileViewer.Views
{
    public class MainWindowViewModel : ViewModelBase
    {
        private FileViewModelBase _fileViewModel;
        private readonly string _currentAppVersion;
        private string _appTitle;

        public MainWindowViewModel(OpenedFileService openedFileService, SupportedFilesOpenerService supportedFilesOpenerService)
        {
            if (openedFileService == null) throw new ArgumentNullException(nameof(openedFileService));
            if (supportedFilesOpenerService == null) throw new ArgumentNullException(nameof(supportedFilesOpenerService));

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            _currentAppVersion = $"{version.Major}.{version.Minor}.{version.Revision}";
            UpdateAppTitle();

            openedFileService.OpenedFileChanged += OnOpenedFileChanged;

            OpenFileCommand = new OpenFileCommand(supportedFilesOpenerService);

            OpenLastFileCommand = new OpenLastFileCommand(supportedFilesOpenerService);

            CloseFileCommand = new CloseFileCommand(openedFileService);
        }

        public ICommand OpenFileCommand { get; }

        public ICommand OpenLastFileCommand { get; }

        public ICommand CloseFileCommand { get; }

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
                    this.FileViewModel = new NspInfoViewModel(nspInfo, new FileViewModelFactory());
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
