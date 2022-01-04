using System;
using System.ComponentModel;
using System.IO;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Utils.MVVM;
using LibHac.Common.Keys;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services
{
    public class KeySetProviderService : NotifyPropertyChangedBase, IKeySetProviderService
    {


        private readonly object _lock = new();
        private readonly IAppSettings _appSettings;
        private KeySet? _keySet = null;

        private readonly ILogger _logger;
        private string? _actualProdKeysFilePath;
        private string? _actualTitleKeysFilePath;
        private string? _actualConsoleKeysFilePath;

        public KeySetProviderService(IAppSettings appSettings, ILoggerFactory loggerFactory)
        {
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory)))
                .CreateLogger(this.GetType());
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

            AppDirProdKeysFilePath = Path.Combine(PathHelper.CurrentAppDir, IKeySetProviderService.DefaultProdKeysFileName);
            AppDirTitleKeysFilePath = Path.Combine(PathHelper.CurrentAppDir, IKeySetProviderService.DefaultTitleKeysFileName);

            Reset();

            appSettings.PropertyChanged += OnSettingChanged;
        }

        public string AppDirProdKeysFilePath { get; }

        public string AppDirTitleKeysFilePath { get; }

        public string? ActualProdKeysFilePath
        {
            get => _actualProdKeysFilePath;
            private set
            {
                _actualProdKeysFilePath = value;
                NotifyPropertyChanged();
            }
        }

        public string? ActualTitleKeysFilePath
        {
            get => _actualTitleKeysFilePath;
            private set
            {
                _actualTitleKeysFilePath = value;
                NotifyPropertyChanged();
            }
        }

        public string? ActualConsoleKeysFilePath
        {
            get => _actualConsoleKeysFilePath;
            private set
            {
                _actualConsoleKeysFilePath = value;
                NotifyPropertyChanged();
            }
        }

        public KeySet GetKeySet(bool forceReload = false)
        {
            lock (_lock)
            {
                if (forceReload)
                    UnloadCurrentKeySet();
                else if (_keySet != null)
                    return _keySet;

                _keySet = LoadKeySet();
                return _keySet;
            }
        }

        private void OnSettingChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(IAppSettings.ProdKeysFilePath)
                || args.PropertyName == nameof(IAppSettings.TitleKeysFilePath)
                || args.PropertyName == nameof(IAppSettings.ConsoleKeysFilePath))
                Reset();

        }

        public void Reset()
        {
            UnloadCurrentKeySet();
            UpdateActualProdKeysFilePath();
            UpdateActualTitleKeysFilePath();
            UpdateActualConsoleKeysFilePath();
        }

        private void UnloadCurrentKeySet()
        {
            lock (_lock)
            {
                _keySet = null;
            }
        }

        /// <summary>
        /// Loads the KeySet with 
        /// </summary>
        /// <returns></returns>
        private KeySet LoadKeySet()
        {
            try
            {
                var actualProdKeysFilePath = ActualProdKeysFilePath;
                var actualTitleKeysFilePath = ActualTitleKeysFilePath;
                var actualConsoleKeysFilePath = ActualConsoleKeysFilePath;

                _logger.LogInformation(LocalizationManager.Instance.Current.Keys.KeysLoading_Starting_Log);

                _logger.LogInformation(LocalizationManager.Instance.Current.Keys.KeysFileUsed.SafeFormat(IKeySetProviderService.DefaultProdKeysFileName, actualProdKeysFilePath ?? LocalizationManager.Instance.Current.Keys.NoneKeysFile));
                _logger.LogInformation(LocalizationManager.Instance.Current.Keys.KeysFileUsed.SafeFormat(IKeySetProviderService.DefaultTitleKeysFileName, actualTitleKeysFilePath ?? LocalizationManager.Instance.Current.Keys.NoneKeysFile));
                _logger.LogInformation(LocalizationManager.Instance.Current.Keys.KeysFileUsed.SafeFormat(IKeySetProviderService.DefaultConsoleKeysFileName, actualConsoleKeysFilePath ?? LocalizationManager.Instance.Current.Keys.NoneKeysFile));

                var keySet = new KeySet();
                ExternalKeyReader.ReadKeyFile(keySet, filename: actualProdKeysFilePath, titleKeysFilename: actualTitleKeysFilePath, consoleKeysFilename: actualConsoleKeysFilePath,
                    new LibHacProgressReportRelay(
                        _ =>
                        {

                        },
                        message =>
                        {
                            _logger.LogWarning(message);
                        })
                );

                _logger.LogInformation(LocalizationManager.Instance.Current.Keys.KeysLoading_Successful_Log);

                return keySet;
            }
            catch (Exception ex)
            {
                Reset();
                throw new Exception(LocalizationManager.Instance.Current.Keys.KeysLoading_Error.SafeFormat(ex.Message));
            }
        }

        private void UpdateActualProdKeysFilePath()
        {
            var findProdKeysFile = FindKeysFile(_appSettings.ProdKeysFilePath, IKeySetProviderService.DefaultProdKeysFileName);

            if (findProdKeysFile == null)
                _logger.LogWarning(LocalizationManager.Instance.Current.Keys.WarnNoProdKeysFileFound);

            ActualProdKeysFilePath = findProdKeysFile;
        }

        private void UpdateActualTitleKeysFilePath()
        {
            ActualTitleKeysFilePath = FindKeysFile(_appSettings.TitleKeysFilePath, IKeySetProviderService.DefaultTitleKeysFileName);
        }

        private void UpdateActualConsoleKeysFilePath()
        {
            ActualConsoleKeysFilePath = FindKeysFile(_appSettings.ConsoleKeysFilePath, IKeySetProviderService.DefaultConsoleKeysFileName);
        }

        private string? FindKeysFile(string? keysFilePathRawFromSettings, string keysFileName)
        {

            // 1. Check from settings (if defined)
            if (!string.IsNullOrWhiteSpace(keysFilePathRawFromSettings))
            {
                var keysFilePathTemp = keysFilePathRawFromSettings.ToFullPath();
                if (File.Exists(keysFilePathTemp))
                    return keysFilePathTemp;
                _logger.LogWarning(
                    LocalizationManager.Instance.Current.Keys.InvalidSetting_KeysFileNotFound.SafeFormat(
                        keysFilePathRawFromSettings));
            }

            // 2. Try to load from the current app dir
            var appDirKeysFilePath = Path.Combine(PathHelper.CurrentAppDir, keysFileName);
            if (File.Exists(appDirKeysFilePath))
                return appDirKeysFilePath;

            // 3. Check from "userHomeDir/.switch" directory
            var homeUserDir = PathHelper.HomeUserDir;
            if (homeUserDir != null)
            {
                var homeDirKeysFilePath = Path.Combine(homeUserDir, ".switch", keysFileName).ToFullPath();
                if (File.Exists(homeDirKeysFilePath))
                    return homeDirKeysFilePath;
            }

            return null;
        }

    }
}
