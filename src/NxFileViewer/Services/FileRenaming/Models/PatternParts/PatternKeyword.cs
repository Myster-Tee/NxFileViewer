using System;
using System.Collections.Generic;
using System.Linq;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;

public enum PatternKeyword
{
    TitleIdL,
    TitleIdU,
    FirstTitleName,
    PackageTypeL,
    PackageTypeU,
    VersionNum,
    OnlineTitleName,
}


public static class PatternKeywords
{
    private static PatternKeyword[]? _allowedApplicationKeywords;
    private static PatternKeyword[]? _allowedPatchKeywords;
    private static PatternKeyword[]? _allowedAddonKeywords;

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
        return _allowedAddonKeywords ??= Enum.GetValues<PatternKeyword>().Where(keyword => keyword != PatternKeyword.FirstTitleName).ToArray();
    }
}