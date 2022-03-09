using System.Collections.Generic;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Patch;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Models;

public class NamingPatterns : INamingPatterns
{
    IReadOnlyList<ApplicationPatternPart> INamingPatterns.ApplicationPattern => ApplicationPattern;

    public List<ApplicationPatternPart> ApplicationPattern { get; set; } = new();

    IReadOnlyList<PatchPatternPart> INamingPatterns.PatchPattern => PatchPattern;

    public List<PatchPatternPart> PatchPattern { get; set; } = new();

    public string AddonPattern { get; set; }
}