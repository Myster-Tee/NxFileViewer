using System.IO;
using System.Reflection;

namespace Emignatik.NxFileViewer.Views
{
    public class MainWindowViewModel : ViewModelBase
    {
        private FileViewModelBase _fileViewModel;
        private readonly string _currentAppVersion;
        private string _appTitle;

        public MainWindowViewModel()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            _currentAppVersion = $"{version.Major}.{version.Minor}.{version.Revision}";
            UpdateAppTitle();
        }

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

        private void UpdateAppTitle()
        {
            var fileViewModel = FileViewModel;

            var title = $"NX File Info v{_currentAppVersion}";

            var openedFilePath = fileViewModel?.Source;
            if (!string.IsNullOrEmpty(openedFilePath))
                title += $" - {Path.GetFileName(openedFilePath)}";

            AppTitle = title;
        }
    }
}
