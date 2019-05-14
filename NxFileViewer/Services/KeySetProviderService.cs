using System;
using System.IO;
using Emignatik.NxFileViewer.KeysParsing;
using Emignatik.NxFileViewer.Properties;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.Services
{
    public static class KeySetProviderService
    {

        private static KeySet _keySet = null;


        public static bool TryGetKeySet(out KeySet keySet, out string errorMessage, bool forceReload = false)
        {
            if (_keySet != null && !forceReload)
            {
                keySet = _keySet;
                errorMessage = null;
                return true;
            }
            try
            {
                keySet = LoadKeySet();
                _keySet = keySet;
                errorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                keySet = null;
                errorMessage = ex.Message;
                return false;
            }
        }

        private static KeySet LoadKeySet()
        {
            var keysFilePath = Settings.Default.KeysFilePath;
            if (string.IsNullOrWhiteSpace(keysFilePath))
                throw new Exception("Keys file path defined in the settings is empty.");

            var keysFullFilePath = keysFilePath.ToFullPath();

            if (!File.Exists(keysFullFilePath))
                throw new Exception($"Keys file path defined in the settings \"{keysFullFilePath}\" couldn't be found.");

            try
            {
                _keySet = new KeysParser().ParseKeys(keysFullFilePath);
                return _keySet;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load keys from file \"{keysFullFilePath}\": {ex.Message}.");
            }
        }
    }
}
