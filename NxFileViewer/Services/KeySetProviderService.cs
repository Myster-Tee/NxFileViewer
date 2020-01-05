using System;
using System.IO;
using Emignatik.NxFileViewer.Properties;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils;
using LibHac;

namespace Emignatik.NxFileViewer.Services
{
    public static class KeySetProviderService
    {

        private static Keyset _keyset = null;

        public static Keyset GetKeySet(bool forceReload = false)
        {
            if (_keyset != null && !forceReload)
            {
                return _keyset;
            }
            _keyset = LoadKeySet();
            return _keyset;
        }

        public static bool TryGetKeySet(out Keyset keyset, bool forceReload = false)
        {
            if (_keyset != null && !forceReload)
            {
                keyset = _keyset;
                return true;
            }
            try
            {
                keyset = LoadKeySet();
                _keyset = keyset;
                return true;
            }
            catch
            {
                keyset = null;
                return false;
            }
        }

        private static Keyset LoadKeySet()
        {
            var keysFilePath = AppSettings.Default.KeysFilePath;
            if (string.IsNullOrWhiteSpace(keysFilePath))
                throw new Exception(Resources.ErrKeysFilePathUndefined);

            var keysFullFilePath = keysFilePath.ToFullPath();

            if (!File.Exists(keysFullFilePath))
                throw new Exception(string.Format(Resources.ErrKeysFilePathNotFound, keysFullFilePath));

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
