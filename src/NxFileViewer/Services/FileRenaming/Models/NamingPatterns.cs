using System.Collections.Generic;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Addon;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Patch;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Models;

public class NamingPatterns : INamingPatterns
{
    IEnumerable<ApplicationPatternPart> INamingPatterns.ApplicationPattern => ApplicationPattern;

    public List<ApplicationPatternPart> ApplicationPattern { get; set; } = new();

    IEnumerable<PatchPatternPart> INamingPatterns.PatchPattern => PatchPattern;

    public List<PatchPatternPart> PatchPattern { get; set; } = new();

    IEnumerable<AddonPatternPart> INamingPatterns.AddonPattern => AddonPattern;

    public List<AddonPatternPart> AddonPattern { get; set; } = new();
}