using System.Collections.Generic;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Addon;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Patch;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Models;

public class NamingSettings : INamingSettings
{
    IEnumerable<ApplicationPatternPart> INamingSettings.ApplicationPattern => ApplicationPattern;

    public List<ApplicationPatternPart> ApplicationPattern { get; set; } = new();

    IEnumerable<PatchPatternPart> INamingSettings.PatchPattern => PatchPattern;

    public List<PatchPatternPart> PatchPattern { get; set; } = new();

    IEnumerable<AddonPatternPart> INamingSettings.AddonPattern => AddonPattern;

    public List<AddonPatternPart> AddonPattern { get; set; } = new();

    public string? InvalidFileNameCharsReplacement { get; set; }

    public bool ReplaceWhiteSpaceChars { get; set; }

    public string? WhiteSpaceCharsReplacement { get; set; }

}