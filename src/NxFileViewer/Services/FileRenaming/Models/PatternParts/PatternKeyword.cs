using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Emignatik.NxFileViewer.FileLoading.QuickFileInfoLoading;
using Emignatik.NxFileViewer.Utils.CustomAttributes;
using LibHac.Ncm;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;

public enum PatternKeyword
{
    /// <summary>
    /// Id of the current package
    /// </summary>
    [SerializedValue("TitleId")]
    TitleId,

    /// <summary>
    /// Id of the corresponding application.
    /// If content is an <see cref="ContentMetaType.Application"/>, this property is equal to <see cref="TitleId"/>.
    /// If content is a <see cref="ContentMetaType.Patch"/> or an <see cref="ContentMetaType.AddOnContent"/>, this property is equal to the id of the corresponding application.
    /// </summary>
    [SerializedValue("AppId")]
    ApplicationTitleId,

    /// <summary>
    /// If content is an <see cref="ContentMetaType.Application"/>, this property is equal to id of a patch content, otherwise zero
    /// </summary>
    [SerializedValue("PatchId")]
    PatchTitleId,

    /// <summary>
    /// The patch number.
    /// If content is an <see cref="ContentMetaType.Application"/>, value is normally 0.
    /// If content is a <see cref="ContentMetaType.Patch"/>, value corresponds to the patch number.
    /// If content is an <see cref="ContentMetaType.AddOnContent"/>, value corresponds to the addon patch number.
    /// </summary>
    [SerializedValue("PatchNum")]
    PatchNumber,

    /// <summary>
    /// The first title name in the list of declared titles.
    /// Exists only for <see cref="ContentMetaType.Application"/> and <see cref="ContentMetaType.Patch"/> but not for <see cref="ContentMetaType.AddOnContent"/>
    /// </summary>
    [SerializedValue("Title")]
    TitleName,

    /// <summary>
    /// The detected package type (<see cref="AccuratePackageType"/>)
    /// </summary>
    [SerializedValue("Ext")]
    PackageType,

    /// <summary>
    /// The content version number.
    /// </summary>
    [SerializedValue("VerNum")]
    VersionNumber,   
    
    /// <summary>
    /// The title name retrieved from the online API, based on the content Title Id.
    /// </summary>
    [SerializedValue("WTitle")]
    OnlineTitleName,   
    
    /// <summary>
    /// The title name retrieved from the online API, based on the content Application Title Id.
    /// </summary>
    [SerializedValue("WAppTitle")]
    OnlineAppTitleName,

    /// <summary>
    /// The update display version number.
    /// </summary>
    [SerializedValue("DisplayVer")]
    DisplayVersion,

    /// <summary>
    /// The update display version number.
    /// </summary>
    [SerializedValue("XCIUpdateVer")]
    XCIUpdateVersion,
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