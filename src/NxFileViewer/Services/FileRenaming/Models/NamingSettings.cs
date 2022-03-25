using System.Collections.Generic;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Models;

public class NamingSettings : INamingSettings
{
    IReadOnlyList<PatternPart> INamingSettings.ApplicationPattern => ApplicationPattern;

    public List<PatternPart> ApplicationPattern { get; set; } = new();

    IReadOnlyList<PatternPart> INamingSettings.PatchPattern => PatchPattern;

    public List<PatternPart> PatchPattern { get; set; } = new();

    IReadOnlyList<PatternPart> INamingSettings.AddonPattern => AddonPattern;

    public List<PatternPart> AddonPattern { get; set; } = new();

    public string? InvalidFileNameCharsReplacement { get; set; }

    public bool ReplaceWhiteSpaceChars { get; set; }

    public string? WhiteSpaceCharsReplacement { get; set; }

}