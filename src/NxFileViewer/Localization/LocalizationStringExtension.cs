using System;
using System.Reflection;

namespace Emignatik.NxFileViewer.Localization;

public static class LocalizationStringExtension
{
    public static event FormatExceptionHandler? FormatException;

    public static string SafeFormat(this string str, params object?[] args)
    {
        try
        {
            return string.Format(str, args);
        }
        catch (Exception ex)
        {
            NotifyFormatException(new FormatExceptionHandlerArgs(str, args, ex));
            return str;
        }
    }

    private static void NotifyFormatException(FormatExceptionHandlerArgs args)
    {
        FormatException?.Invoke(MethodBase.GetCurrentMethod()?.DeclaringType, args);
    }
}

public delegate void FormatExceptionHandler(object? sender, FormatExceptionHandlerArgs args);

public class FormatExceptionHandlerArgs
{

    public FormatExceptionHandlerArgs(string keyValue, object?[] formatArgs, Exception ex)
    {
        KeyValue = keyValue;
        FormatArgs = formatArgs;
        Ex = ex;
    }

    public string KeyValue { get; }

    public Exception Ex { get; }

    public object?[] FormatArgs { get; }

}