using System;
using System.Collections.Generic;

namespace Emignatik.NxFileViewer.Utils
{
    public static class FlagEnumHelper
    {

        public static IEnumerable<string> ToFlags<T>(this T flagEnum) where T: struct, Enum
        {
            var values = Enum.GetValues<T>();
            foreach (var enumValue in values)
            {
                if (flagEnum.HasFlag(enumValue))
                    yield return enumValue.ToString();
            }
        }

        public static string ToFlagsString<T>(this T flagEnum, string? separator = ", ") where T : struct, Enum
        {
            return string.Join(separator, ToFlags(flagEnum));
        }

    }
}
