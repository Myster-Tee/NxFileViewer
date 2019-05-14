using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Emignatik.NxFileViewer.Utils
{
    public static class ByteArrayExt
    {
        private static readonly Encoding _defaultEncoding = Encoding.UTF8;

        /// <summary>
        /// Returns null if array is null
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string AsNullTerminatedString(this byte[] bytes, Encoding encoding)
        {
            if (bytes == null)
                return null;

            var terminatorIndex = Array.IndexOf(bytes, (byte)0);
            var strLen = terminatorIndex < 0 ? bytes.Length : terminatorIndex;
            return encoding.GetString(bytes, 0, strLen);
        }

        /// <summary>
        /// Returns null if array is null
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string AsNullTerminatedString(this byte[] bytes)
        {
            return bytes.AsNullTerminatedString(_defaultEncoding);
        }

        public static string ToHexString(this IEnumerable<byte> bytes, bool lowerCase = false)
        {
            var format = lowerCase ? "x2" : "X2";
            return bytes?.Aggregate("", (current, b) => current + b.ToString(format));
        }

        public static T ToStruct<T>(this byte[] bytes, int offset = 0) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                var pinnedObjectAddress = handle.AddrOfPinnedObject() + offset;
                var structure = (T)Marshal.PtrToStructure(pinnedObjectAddress, typeof(T));
                return structure;
            }
            finally
            {
                handle.Free();
            }
        }
    }
}