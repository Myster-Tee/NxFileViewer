using System;
using System.Collections.Generic;
using System.Linq;

namespace Emignatik.NxFileViewer.Utils
{
    public static class NumExt
    {
        public static string ToHex(this int num)
        {
            return ToHex(BitConverter.GetBytes(num));
        }

        public static string ToHex(this uint num)
        {
            return ToHex(BitConverter.GetBytes(num));
        }

        public static string ToHex(this long num)
        {
            return ToHex(BitConverter.GetBytes(num));
        }

        public static string ToHex(this ulong num)
        {
            return ToHex(BitConverter.GetBytes(num));
        }

        private static string ToHex(IEnumerable<byte> bytes)
        {
            return bytes.Reverse().Aggregate("", (current, b) => current + b.ToString("X2"));
        }
    }
}
