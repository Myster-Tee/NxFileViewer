using System.Collections.Generic;
using System.Linq;

namespace Emignatik.NxFileViewer.Utils
{
    public static class ByteArrayExt
    {

        public static string ToHexString(this IEnumerable<byte> bytes, bool lowerCase = false)
        {
            var format = lowerCase ? "x2" : "X2";
            return bytes?.Aggregate("", (current, b) => current + b.ToString(format));
        }
        
    }
}