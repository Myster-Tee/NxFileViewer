using System;
using System.IO;
using Emignatik.NxFileViewer.Properties;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils;
using LibHac;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services
{
    public class KeySetProviderService : IKeySetProviderService
    {
        private readonly IAppSettings _appSettings;
        private Keyset _keyset = null;

        private string _lastLoadedProdKeysFilePath;
        private string _lastLoadedConsoleKeysFilePath;
        private string _lastLoadedTitleKeysFilePath;

        private bool _shouldReloadKeys = false;
        private ILogger _logger;

        public KeySetProviderService(IAppSettings appSettings, ILoggerFactory loggerFactory)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger(this.GetType());

            appSettings.SettingChanged += OnAppSettingChanged;
        }

        private void OnAppSettingChanged(object sender, SettingChangedHandlerArgs args)
        {

            if (args.SettingName == nameof(IAppSettings.ProdKeysFilePath)
                || args.SettingName == nameof(IAppSettings.ConsoleKeysFilePath)
                || args.SettingName == nameof(IAppSettings.TitleKeysFilePath))
            {
                _shouldReloadKeys = _appSettings.ProdKeysFilePath != _lastLoadedProdKeysFilePath
                                    || _appSettings.ConsoleKeysFilePath != _lastLoadedConsoleKeysFilePath
                                    || _appSettings.TitleKeysFilePath != _lastLoadedTitleKeysFilePath;

            }
        }



        public Keyset GetKeySet(bool forceReload = false)
        {
            if (_keyset != null && !forceReload && !_shouldReloadKeys)
            {
                return _keyset;
            }
            _keyset = LoadKeySet();
            return _keyset;
        }

        private Keyset LoadKeySet()
        {
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            _lastLoadedProdKeysFilePath = FindProdKeysFile(homeDir);
            _lastLoadedConsoleKeysFilePath = FindConsoleKeysFile(homeDir);
            _lastLoadedTitleKeysFilePath = FindTitleKeysFile(homeDir);

            try
            {
                _keyset = ExternalKeyReader.ReadKeyFile(_lastLoadedProdKeysFilePath, _lastLoadedTitleKeysFilePath, _lastLoadedConsoleKeysFilePath);
                return _keyset;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.ErrKeysLoadingFailed, ex.Message));
            }
        }

        private string FindProdKeysFile(string homeDir)
        {
            var keysFilePathFromSettings = _appSettings.ProdKeysFilePath;

            if (!string.IsNullOrWhiteSpace(keysFilePathFromSettings))
            {
                keysFilePathFromSettings = keysFilePathFromSettings.ToFullPath();
                if (File.Exists(keysFilePathFromSettings))
                    return keysFilePathFromSettings;
                _logger.LogWarning(Resources.InvalidSetting_ProdKeysNotFound);
            }

            var keysFilePathFromHomeDir = Path.Combine(homeDir, ".switch", "prod.keys").ToFullPath();
            if (!File.Exists(keysFilePathFromHomeDir))
                throw new FileNotFoundException(Resources.ErrNoProdKeysFileFound);

            return keysFilePathFromHomeDir;
        }

        private string FindConsoleKeysFile(string homeDir)
        {
            var keysFilePathFromSettings = _appSettings.ConsoleKeysFilePath;

            if (!string.IsNullOrWhiteSpace(keysFilePathFromSettings))
            {
                keysFilePathFromSettings = keysFilePathFromSettings.ToFullPath();
                if (File.Exists(keysFilePathFromSettings))
                    return keysFilePathFromSettings;
                _logger.LogWarning(Resources.InvalidSetting_ConsoleKeysNotFound);
            }

            var keysFilePathFromHomeDir = Path.Combine(homeDir, ".switch", "console.keys").ToFullPath();

            return File.Exists(keysFilePathFromHomeDir) ? keysFilePathFromHomeDir : null;
        }

        private string FindTitleKeysFile(string homeDir)
        {
            var keysFilePathFromSettings = _appSettings.TitleKeysFilePath;

            if (!string.IsNullOrWhiteSpace(keysFilePathFromSettings))
            {
                keysFilePathFromSettings = keysFilePathFromSettings.ToFullPath();
                if (File.Exists(keysFilePathFromSettings))
                    return keysFilePathFromSettings;
                _logger.LogWarning(Resources.InvalidSetting_TitleKeysNotFound);
            }

            var keysFilePathFromHomeDir = Path.Combine(homeDir, ".switch", "title.keys").ToFullPath();

            return File.Exists(keysFilePathFromHomeDir) ? keysFilePathFromHomeDir : null;
        }
    }
}
