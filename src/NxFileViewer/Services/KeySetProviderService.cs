using System;
using System.ComponentModel;
using System.IO;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils;
using LibHac;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services
{
    public class KeySetProviderService : IKeySetProviderService
    {
        public const string DefaultProdKeysFileName = "prod.keys";
        public const string DefaultConsoleKeysFileName = "console.keys";
        public const string DefaultTitleKeysFileName = "title.keys";

        private readonly IAppSettings _appSettings;
        private Keyset? _keySet = null;

        private string? _prodKeysFilePath;
        private string? _consoleKeysFilePath;
        private string? _titleKeysFilePath;

        private readonly ILogger _logger;

        public KeySetProviderService(IAppSettings appSettings, ILoggerFactory loggerFactory)
        {
            AppDirProdKeysFilePath = Path.Combine(PathHelper.CurrentAppDir, DefaultProdKeysFileName);

            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger(this.GetType());

            appSettings.PropertyChanged += OnSettingChanged;
        }

        public string AppDirProdKeysFilePath { get; }

        public bool ProdKeysFileFound => GetProdKeysFilePath() != null;

        public Keyset GetKeySet(bool forceReload = false)
        {
            if (forceReload)
                UnloadCurrentKeySet();
            else if (_keySet != null)
                return _keySet;

            _keySet = LoadKeySet();
            return _keySet;
        }

        private void OnSettingChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(IAppSettings.ProdKeysFilePath)
                || args.PropertyName == nameof(IAppSettings.ConsoleKeysFilePath)
                || args.PropertyName == nameof(IAppSettings.TitleKeysFilePath))
            {
                UnloadCurrentKeySet();
            }
        }

        public void UnloadCurrentKeySet()
        {
            _prodKeysFilePath = null;
            _consoleKeysFilePath = null;
            _titleKeysFilePath = null;
            _keySet = null;
        }

        private Keyset LoadKeySet()
        {
            _prodKeysFilePath = GetProdKeysFilePath();
            _consoleKeysFilePath = GetConsoleKeysFilePath();
            _titleKeysFilePath = GetTitleKeysFilePath();

            try
            {
                _logger.LogInformation(LocalizationManager.Instance.Current.Keys.KeysFileUsed.SafeFormat(DefaultProdKeysFileName, _prodKeysFilePath ?? LocalizationManager.Instance.Current.Keys.NoneKeysFile));
                _logger.LogInformation(LocalizationManager.Instance.Current.Keys.KeysFileUsed.SafeFormat(DefaultConsoleKeysFileName, _consoleKeysFilePath ?? LocalizationManager.Instance.Current.Keys.NoneKeysFile));
                _logger.LogInformation(LocalizationManager.Instance.Current.Keys.KeysFileUsed.SafeFormat(DefaultTitleKeysFileName, _titleKeysFilePath ?? LocalizationManager.Instance.Current.Keys.NoneKeysFile));

                var keySet = new Keyset();
                // DEBT: ReadKeyFile accepts the first path to be null, should notify libhac owner?
                ExternalKeyReader.ReadKeyFile(keySet, _prodKeysFilePath, _titleKeysFilePath, _consoleKeysFilePath);
                return keySet;
            }
            catch (Exception ex)
            {
                throw new Exception(LocalizationManager.Instance.Current.Keys.ErrKeysLoadingFailed.SafeFormat(ex.Message));
            }
        }

        private string? GetProdKeysFilePath()
        {
            var findProdKeysFile = _prodKeysFilePath ??= FindKeysFile(_appSettings.ProdKeysFilePath, DefaultProdKeysFileName);

            if (findProdKeysFile == null)
                _logger.LogWarning(LocalizationManager.Instance.Current.Keys.WarnNoProdKeysFileFound);

            return findProdKeysFile;
        }

        private string? GetConsoleKeysFilePath()
        {
            return _consoleKeysFilePath ??= FindKeysFile(_appSettings.ConsoleKeysFilePath, DefaultConsoleKeysFileName);
        }


        private string? GetTitleKeysFilePath()
        {
            return _titleKeysFilePath ??= FindKeysFile(_appSettings.TitleKeysFilePath, DefaultTitleKeysFileName);
        }

        private string? FindKeysFile(string? keysFilePathRawFromSettings, string keysFileName)
        {

            // 1. Check from settings (if defined)
            if (!string.IsNullOrWhiteSpace(keysFilePathRawFromSettings))
            {
                var keysFilePathTemp = keysFilePathRawFromSettings.ToFullPath();
                if (File.Exists(keysFilePathTemp))
                    return keysFilePathTemp;
                _logger.LogWarning(LocalizationManager.Instance.Current.Keys.InvalidSetting_KeysFileNotFound.SafeFormat(keysFilePathRawFromSettings));
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
