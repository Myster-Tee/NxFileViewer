using System;
using System.Globalization;
using System.Windows.Data;

namespace Emignatik.NxFileViewer.Utils.MVVM.Converters;


/// <summary>
/// Converts integer (long or int) to string and back
/// </summary>
public class IntegerConverter : IValueConverter
{

    /// <summary>
    /// ViewModel to View
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (targetType != typeof(string))
            throw new InvalidOperationException("String target type expected to View.");

        if (value is int i)
            return i.ToString();

        if (value is long l)
            return l.ToString();

        throw new InvalidOperationException($"Integer value type expected from ViewModel, found {value?.GetType().Name ?? "null"}.");
    }

    /// <summary>
    /// View to ViewModel
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string valueStrRaw)
            throw new InvalidOperationException("String value type expected from View.");

        var valueStr = "";
        foreach (var c in valueStrRaw)
        {
            if (c is >= '0' and <= '9')
                valueStr += c;
        }

        if (valueStr.Length <= 0)
            return 0;

        if (targetType == typeof(int))
        {
            if (int.TryParse(valueStr, out var i))
                return i;
            return int.MaxValue;
        }

        if (targetType == typeof(long))
        {
            if (long.TryParse(valueStr, out var l))
                return l;
            return long.MaxValue;
        }

        throw new InvalidOperationException($"Integer target type expected from ViewModel, found {targetType.Name}.");
    }
}
