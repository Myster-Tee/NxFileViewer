using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Emignatik.NxFileViewer.Utils.CustomAttributes;

public static class SerializedValueAttributeHelper
{
    public static IEnumerable<Mapping<T>> GetSerializationMapping<T>() where T : struct, Enum
    {

        foreach (var patternKeyword in Enum.GetValues<T>())
        {
            var serializedValue = GetSerializedValue(patternKeyword);
            if (serializedValue == null)
                continue;

            yield return new Mapping<T>
            {
                EnumValue = patternKeyword,
                SerializedValue = serializedValue
            };
        }
    }

    public static string? GetSerializedValue<T>(T enumValue) where T : struct, Enum
    {
        var memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).First();

        var attribute = memberInfo.GetCustomAttributes<SerializedValueAttribute>(false).FirstOrDefault();

        return attribute?.Value;
    }
}

public class Mapping<T>
{
    public T EnumValue { get; init; } = default!;

    public string SerializedValue { get; init; } = default!;
}