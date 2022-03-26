using System;
using System.Collections.Generic;
using System.Linq;

namespace Emignatik.NxFileViewer.Utils;

public static class IdHelper
{
    public static string ToStrId(this int num)
    {
        return ToIdFromConvertedNumBytes(BitConverter.GetBytes(num));
    }

    public static string ToStrId(this uint num)
    {
        return ToIdFromConvertedNumBytes(BitConverter.GetBytes(num));
    }

    public static string ToStrId(this long num)
    {
        return ToIdFromConvertedNumBytes(BitConverter.GetBytes(num));
    }

    public static string ToStrId(this ulong num)
    {
        return ToIdFromConvertedNumBytes(BitConverter.GetBytes(num));
    }

    public static string ToStrId(this IEnumerable<byte> bytes)
    {
        return bytes.Aggregate("", (current, b) => current + b.ToString("X2"));
    }  
    
    public static string ToStrId(this Span<byte> bytes)
    {
        return bytes.ToArray().ToStrId();
    }

    private static string ToIdFromConvertedNumBytes(IEnumerable<byte> getBytes)
    {
        return ToStrId(getBytes.Reverse());
    }
}