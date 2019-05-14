using System;
using System.Collections.Generic;

namespace Emignatik.NxFileViewer.KeysParsing
{
    public class KeySet
    {
        private readonly Dictionary<string, byte[]> _dictionary;

        public KeySet(Dictionary<string, byte[]> dictionary)
        {
            _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public byte[] GetKey(string keyName)
        {
            if (!TryGetKey(keyName, out var bytes)) throw new Exception($"Key \"{keyName}\" not found.");
            if (bytes == null) throw new Exception($"Key \"{keyName}\" is undefined.");
            return bytes;
        }

        public bool TryGetKey(string keyName, out byte[] bytes)
        {
            foreach (var kvp in _dictionary)
            {
                if (!string.Equals(keyName, kvp.Key, StringComparison.InvariantCultureIgnoreCase)) continue;

                bytes = kvp.Value;
                return bytes != null;
            }

            bytes = null;
            return false;
        }
    }
}