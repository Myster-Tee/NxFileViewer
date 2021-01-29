using System;
using System.Collections.Generic;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace Emignatik.NxFileViewer.Views
{
    public class SettingsWindowViewModel : ViewModelBase
    {
        private readonly IAppSettingsManager _appSettingsManager;
        private readonly IAppSettings _appSettings;
        private string _prodKeysFilePath;
        private string _consoleKeysFilePath;
        private string _titleKeysFilePath;
        private StructureLoadingMode _selectedStructureLoadingMode;
        private LogLevel _selectedLogLevel;
        private string _prodKeysDownloadUrl;


        public SettingsWindowViewModel(IAppSettingsManager appSettingsManager)
        {
            _appSettingsManager = appSettingsManager ?? throw new ArgumentNullException(nameof(appSettingsManager));
            _appSettings = appSettingsManager.Settings;

            BrowseProdKeysCommand = new RelayCommand(OnBrowseProdKeys);
            BrowseConsoleKeysCommand = new RelayCommand(OnBrowseConsoleKeys);
            BrowseTitleKeysCommand = new RelayCommand(OnBrowseTitleKeys);
            SaveSettingsCommand = new RelayCommand(OnSaveSettings);
            CancelSettingsCommand = new RelayCommand(OnCancelSettings);

            ProdKeysFilePath = _appSettings.ProdKeysFilePath;
            ConsoleKeysFilePath = _appSettings.ConsoleKeysFilePath;
            TitleKeysFilePath = _appSettings.TitleKeysFilePath;
            SelectedStructureLoadingMode = _appSettings.StructureLoadingMode;
            SelectedLogLevel = _appSettings.LogLevel;
            ProdKeysDownloadUrl = _appSettings.ProdKeysDownloadUrl;
        }

        public event EventHandler? OnQueryCloseView;

        public ICommand BrowseProdKeysCommand { get; }

        public ICommand BrowseConsoleKeysCommand { get; }

        public ICommand BrowseTitleKeysCommand { get; }

        public ICommand SaveSettingsCommand { get; }

        public ICommand CancelSettingsCommand { get; }


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

        public IEnumerable<StructureLoadingMode> StructureLoadingModes => Enum.GetValues<StructureLoadingMode>();

        public StructureLoadingMode SelectedStructureLoadingMode
        {
            get => _selectedStructureLoadingMode;
            set
            {
                _selectedStructureLoadingMode = value;
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<LogLevel> LogLevels => Enum.GetValues<LogLevel>();

        public LogLevel SelectedLogLevel
        {
            get => _selectedLogLevel;
            set
            {
                _selectedLogLevel = value;
                NotifyPropertyChanged();
            }
        }

        public string ProdKeysDownloadUrl
        {
            get => _prodKeysDownloadUrl;
            set
            {
                _prodKeysDownloadUrl = value;
                NotifyPropertyChanged();
            }
        }

        private void OnBrowseProdKeys()
        {
            if (BrowseKeysFilePath(ProdKeysFilePath, LocalizationManager.Instance.Current.Keys.BrowseKeysFile_ProdTitle, out var selectedFilePath))
            {
                ProdKeysFilePath = selectedFilePath;
            }
        }

        private void OnBrowseConsoleKeys()
        {
            if (BrowseKeysFilePath(ConsoleKeysFilePath, LocalizationManager.Instance.Current.Keys.BrowseKeysFile_ConsoleTitle, out var selectedFilePath))
            {
                ConsoleKeysFilePath = selectedFilePath;
            }
        }

        private void OnBrowseTitleKeys()
        {
            if (BrowseKeysFilePath(TitleKeysFilePath, LocalizationManager.Instance.Current.Keys.BrowseKeysFile_TitleTitle, out var selectedFilePath))
            {
                TitleKeysFilePath = selectedFilePath;
            }
        }

        private static bool BrowseKeysFilePath(string initialFilePath, string title, out string? selectedFilePath)
        {
            selectedFilePath = null;

            var openFileDialog = new OpenFileDialog
            {
                Title = title,
                FileName = initialFilePath,
                Filter = LocalizationManager.Instance.Current.Keys.BrowseKeysFile_Filter,
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
            _appSettings.StructureLoadingMode = SelectedStructureLoadingMode;
            _appSettings.LogLevel = SelectedLogLevel;
            _appSettings.ProdKeysDownloadUrl = ProdKeysDownloadUrl;

            _appSettingsManager.Save();
            NotifyQueryCloseView();
        }

        private void OnCancelSettings()
        {
            NotifyQueryCloseView();
        }

        private void NotifyQueryCloseView()
        {
            OnQueryCloseView?.Invoke(this, EventArgs.Empty);
        }
    }
}