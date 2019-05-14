using System;
using System.Collections.Generic;
using System.Linq;

namespace Emignatik.NxFileViewer.KeysParsing
{
    public static class KeyParser
    {
        public static void ParseAesXtsKey(string hexKey, out byte[] key1, out byte[] key2)
        {
            var mid = hexKey.Length / 2;
            key1 = HexStringToByteArray(hexKey.Substring(0, mid));
            key2 = HexStringToByteArray(hexKey.Substring(mid));
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        public static string ByteArrayToHexString(IEnumerable<byte> bytes)
        {
            return bytes.Aggregate("", (current, b) => current + b.ToString("X2"));
        }
    }
}