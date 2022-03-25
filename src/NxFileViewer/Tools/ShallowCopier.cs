using System;
using System.Reflection;

namespace Emignatik.NxFileViewer.Tools;

public class ShallowCopier : IShallowCopier
{
    public void Copy<T>(T? source, T? dest) where T : class
    {
        CopyInternal(typeof(T), source, dest);
    }

    private void CopyInternal(IReflect type, object? source, object? dest)
    {
        if (source == null || dest == null)
            return;

        var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var propertyInfo in propertyInfos)
        {
            // Simple properties copy
            var propType = propertyInfo.PropertyType;
            if (IsSimpleType(propType))
            {
                var getMethod = propertyInfo.GetGetMethod();
                var setMethod = propertyInfo.GetSetMethod();
                if (getMethod == null || setMethod == null) // Property should have both a getter and a setter to be copied
                    continue;

                // Get source property value
                var propValue = getMethod.Invoke(source, null);

                if (propValue != null)
                    // Recopy property value to destination
                    setMethod.Invoke(dest, new[] { propValue });
            }
            else
            {
                // Deep copy
                var getMethod = propertyInfo.GetGetMethod();
                if (getMethod == null)
                    continue;

                var newSource = getMethod.Invoke(source, null);
                var newDest = getMethod.Invoke(dest, null);

                CopyInternal(propType, newSource, newDest);
            }
        }
    }

    /// <summary>
    /// Returns true if type is a value type (int, bool, etc.), a nullable value type (int?, bool?, etc.) or is string
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsSimpleType(Type type)
    {
        if (type.IsValueType || type == typeof(string))
            return true;

        var underlyingType = Nullable.GetUnderlyingType(type);
        return underlyingType is { IsValueType: true };
    }
}