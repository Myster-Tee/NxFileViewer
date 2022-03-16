using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Utils.CustomAttributes;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;

public class DynamicTextPatternPart : PatternPart
{
    public PatternKeyword Keyword { get; }

    public StringOperator StringOperator { get; }

    public DynamicTextPatternPart(PatternKeyword keyword, StringOperator stringOperator)
    {
        Keyword = keyword;
        StringOperator = stringOperator;
    }
}



public enum StringOperator
{
    Untouched,
    [SerializedValue("L")]
    ToLower,
    [SerializedValue("U")]
    ToUpper,
}

public static class StringOperatorHelper
{

    private static List<Mapping<StringOperator>>? _serializationMapping;

    public static IReadOnlyList<string> GetAllowedSerializedValues()
    {
        return GetSerializedValuesMapping()
            .Select(mapping => mapping.SerializedValue)
            .ToArray();
    }

    public static bool TryParse(string serializedValue, [NotNullWhen(true)] out StringOperator? stringOperator)
    {
        var serializedValuesMapping = GetSerializedValuesMapping();

        var foundMapping = serializedValuesMapping
            .FirstOrDefault(mapping => string.Equals(mapping.SerializedValue, serializedValue, StringComparison.OrdinalIgnoreCase));

        if (foundMapping == null)
        {
            stringOperator = null;
            return false;
        }

        stringOperator = foundMapping.EnumValue;
        return true;
    }

    private static IReadOnlyList<Mapping<StringOperator>> GetSerializedValuesMapping()
    {
        if (_serializationMapping != null)
            return _serializationMapping;

        _serializationMapping = SerializedValueAttributeHelper.GetSerializationMapping<StringOperator>().ToList();

        return _serializationMapping;
    }
}