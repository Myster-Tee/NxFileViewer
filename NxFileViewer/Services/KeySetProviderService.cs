using System;
using System.IO;
using Emignatik.NxFileViewer.Properties;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils;
using LibHac;

namespace Emignatik.NxFileViewer.Services
{
    public class KeySetProviderService : IKeySetProviderService
    {
        private readonly IAppSettings _appSettings;

        public KeySetProviderService(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        private Keyset _keyset = null;

        public Keyset GetKeySet(bool forceReload = false)
        {
            if (_keyset != null && !forceReload)
            {
                return _keyset;
            }
            _keyset = LoadKeySet();
            return _keyset;
        }

        private Keyset LoadKeySet()
        {
            var keysFilePath = _appSettings.KeysFilePath;
            if (string.IsNullOrWhiteSpace(keysFilePath))
                throw new Exception(Resources.ErrKeysFilePathUndefined);

            var keysFullFilePath = keysFilePath.ToFullPath();

            if (!File.Exists(keysFullFilePath))
                throw new FileNotFoundException(string.Format(Resources.ErrKeysFilePathNotFound, keysFullFilePath));

            try
            {
                _keyset = ExternalKeyReader.ReadKeyFile(keysFullFilePath);
                return _keyset;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.ErrKeysLoadingFailed, keysFullFilePath, ex.Message));
            }
        }
    }
}
