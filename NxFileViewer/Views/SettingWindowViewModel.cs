using System;
using System.Windows.Input;
using Emignatik.NxFileViewer.Properties;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM;
using Microsoft.Win32;

namespace Emignatik.NxFileViewer.Views
{
    public class SettingWindowViewModel : ViewModelBase
    {
        private readonly IAppSettings _appSettings;
        private string _prodKeysFilePath;
        private string _consoleKeysFilePath;
        private string _titleKeysFilePath;


        public SettingWindowViewModel(IAppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            
            BrowseProdKeysCommand = new RelayCommand(OnBrowseProdKeys);
            BrowseConsoleKeysCommand = new RelayCommand(OnBrowseConsoleKeys);
            BrowseTitleKeysCommand = new RelayCommand(OnBrowseTitleKeys);
            SaveSettingsCommand = new RelayCommand(OnSaveSettings);

            ProdKeysFilePath = _appSettings.ProdKeysFilePath;
            ConsoleKeysFilePath = _appSettings.ConsoleKeysFilePath;
            TitleKeysFilePath = _appSettings.TitleKeysFilePath;
        }


        public ICommand BrowseProdKeysCommand { get; }

        public ICommand BrowseConsoleKeysCommand { get; }

        public ICommand BrowseTitleKeysCommand { get; }

        public ICommand SaveSettingsCommand { get; }


        public string ProdKeysFilePath
        {
            get => _prodKeysFilePath;
            set
            {
                _prodKeysFilePath = value;
                NotifyPropertyChanged(nameof(ProdKeysFilePath));
            }
        }

        public string ConsoleKeysFilePath
        {
            get => _consoleKeysFilePath;
            set
            {
                _consoleKeysFilePath = value;
                NotifyPropertyChanged(nameof(ConsoleKeysFilePath));
            }
        }

        public string TitleKeysFilePath
        {
            get => _titleKeysFilePath;
            set
            {
                _titleKeysFilePath = value;
                NotifyPropertyChanged(nameof(TitleKeysFilePath));
            }
        }

        private void OnBrowseProdKeys()
        {
            if (BrowseKeysFilePath(ProdKeysFilePath, Resources.BrowseKeysFile_ProdTitle, out var selectedFilePath))
            {
                ProdKeysFilePath = selectedFilePath;
            }
        }

        private void OnBrowseConsoleKeys()
        {
            if (BrowseKeysFilePath(ConsoleKeysFilePath, Resources.BrowseKeysFile_ConsoleTitle, out var selectedFilePath))
            {
                ConsoleKeysFilePath = selectedFilePath;
            }
        }

        private void OnBrowseTitleKeys()
        {
            if (BrowseKeysFilePath(TitleKeysFilePath, Resources.BrowseKeysFile_TitleTitle, out var selectedFilePath))
            {
                TitleKeysFilePath = selectedFilePath;
            }
        }

        private bool BrowseKeysFilePath(string initialFilePath, string title, out string selectedFilePath)
        {
            selectedFilePath = null;

            var openFileDialog = new OpenFileDialog
            {
                Title = title,
                FileName = initialFilePath,
                Filter = Resources.BrowseKeysFile_Filter,
            };

            var result = openFileDialog.ShowDialog();
            if (result != null && result.Value)
            {
                selectedFilePath = openFileDialog.FileName;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnSaveSettings()
        {
            _appSettings.ProdKeysFilePath = ProdKeysFilePath;
            _appSettings.ConsoleKeysFilePath = ConsoleKeysFilePath;
            _appSettings.TitleKeysFilePath = TitleKeysFilePath;

            _appSettings.Save();
        }

    }
}