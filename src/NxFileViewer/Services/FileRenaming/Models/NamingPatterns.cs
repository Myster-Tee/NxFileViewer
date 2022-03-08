using System.Collections.Generic;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Models;

public class NamingPatterns : INamingPatterns
{
    IReadOnlyList<ApplicationPatternPart> INamingPatterns.ApplicationPattern => ApplicationPattern;

    public List<ApplicationPatternPart> ApplicationPattern { get; set; } = new();

    public string PatchPattern { get; set; }

    public string AddonPattern { get; set; }
}