using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Emignatik.NxFileViewer.Utils.CustomAttributes;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;

public enum PatternKeyword
{
    [SerializedValue("TitleId")]
    TitleId,
    [SerializedValue("Title")]
    TitleName,
    [SerializedValue("Ext")]
    PackageType,
    [SerializedValue("VerNum")]
    VersionNumber,
    [SerializedValue("WTitle")]
    OnlineTitleName,
}

public static class PatternKeywordHelper
{

    private static PatternKeyword[]? _allowedApplicationKeywords;
    private static PatternKeyword[]? _allowedPatchKeywords;
    private static PatternKeyword[]? _allowedAddonKeywords;
    private static List<Mapping<PatternKeyword>>? _serializationMapping;

    public static IReadOnlyList<PatternKeyword> GetAllowedApplicationKeywords()
    {
        return _allowedApplicationKeywords ??= Enum.GetValues<PatternKeyword>();
    }

    public static IReadOnlyList<PatternKeyword> GetAllowedPatchKeywords()
    {
        return _allowedPatchKeywords ??= Enum.GetValues<PatternKeyword>();
    }

    public static IReadOnlyList<PatternKeyword> GetAllowedAddonKeywords()
    {
        return _allowedAddonKeywords ??= Enum.GetValues<PatternKeyword>().Where(keyword => keyword != PatternKeyword.TitleName).ToArray();
    }

    public static bool TryParse(string serializedValue, [NotNullWhen(true)] out PatternKeyword? keyword)
    {
        var serializedValueTrimmed = serializedValue.Trim();
        var mapping = GetSerializedValuesMapping()
            .FirstOrDefault(mapping => string.Equals(mapping.SerializedValue, serializedValueTrimmed, StringComparison.OrdinalIgnoreCase));

        if (mapping == null)
        {
            keyword = null;
            return false;
        }

        keyword = mapping.EnumValue;
        return true;
    }

    public static IEnumerable<string> GetCorrespondingSerializedValues(IEnumerable<PatternKeyword> keywords)
    {
        return GetSerializedValuesMapping().Where(mapping => keywords.Contains(mapping.EnumValue)).Select(mapping => mapping.SerializedValue);
    }

    private static IReadOnlyList<Mapping<PatternKeyword>> GetSerializedValuesMapping()
    {
        if (_serializationMapping != null)
            return _serializationMapping;

        _serializationMapping = SerializedValueAttributeHelper.GetSerializationMapping<PatternKeyword>().ToList();

        return _serializationMapping;
    }

}