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
            // TODO: charger les autres clés
            //string homeTitleKeyFile = Path.Combine(homeDirectoryPath, ".switch", "title.keys");
            //string homeConsoleKeyFile = Path.Combine(homeDirectoryPath, ".switch", "console.keys");

            _lastLoadedProdKeysFilePath = FindProdKeysFile(homeDir);

            try
            {
                _keyset = ExternalKeyReader.ReadKeyFile(_lastLoadedProdKeysFilePath);
                return _keyset;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.ErrKeysLoadingFailed, ex.Message));
            }
        }

        private string FindProdKeysFile(string homeDir)
        {
            var prodKeysFilePathFromSettings = _appSettings.ProdKeysFilePath;
            var ispPathDefinedInSettings = !string.IsNullOrWhiteSpace(prodKeysFilePathFromSettings);
            prodKeysFilePathFromSettings = ispPathDefinedInSettings ? prodKeysFilePathFromSettings.ToFullPath() : prodKeysFilePathFromSettings;

            if (ispPathDefinedInSettings && File.Exists(prodKeysFilePathFromSettings))
                return prodKeysFilePathFromSettings;

            if (ispPathDefinedInSettings)
                _logger.LogWarning("The prod.keys file defined in the settings doesn't exist.");

            var prodKeysFilePathFromHomeDir = Path.Combine(homeDir, ".switch", "prod.keys");
            if (!File.Exists(prodKeysFilePathFromHomeDir))
                throw new FileNotFoundException(Resources.ErrNoProdKeysFileFound);

            return prodKeysFilePathFromHomeDir;
        }
    }
}
