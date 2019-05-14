using System;
using System.Linq;

namespace Emignatik.NxFileViewer.Crypto
{
    public static class XtsAes
    {
        private const int KEY_LEN_32 = 32;
        private const int KEY_LEN_64 = 64; 

        private static readonly int[] SUPPORTED_KEY_SIZES = new[] { KEY_LEN_32, KEY_LEN_64 };

        public static void SplitKey(byte[] key, out byte[] key1, out byte[] key2)
        {
            if (!SUPPORTED_KEY_SIZES.Contains(key.Length))
            {
                var allowedKeySizes = string.Join("-", SUPPORTED_KEY_SIZES);
                throw new ArgumentException($"Invalid key size, should be any of \"{allowedKeySizes }\".");
            }

            var subKeyLen = key.Length / 2;

            key1 = new byte[subKeyLen];
            key2 = new byte[subKeyLen];

            Buffer.BlockCopy(key, 0, key1, 0, subKeyLen);
            Buffer.BlockCopy(key, subKeyLen, key2, 0, subKeyLen);
        }

        public static IXtsAesCryptoTransform CreateEncryptor(byte[] key)
        {
            SplitKey(key, out var key1, out var key2);
            return new XtsAesCryptoTransform(key1, key2, TransformationMode.ENCRYPT);
        }

        public static IXtsAesCryptoTransform CreateDecryptor(byte[] key)
        {
            SplitKey(key, out var key1, out var key2);
            return new XtsAesCryptoTransform(key1, key2, TransformationMode.DECRYPT);
        }
    }
}