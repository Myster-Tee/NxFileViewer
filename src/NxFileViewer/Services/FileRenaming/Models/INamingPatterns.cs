using System.Collections.Generic;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Patch;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Models;

public interface INamingPatterns
{
    /// <summary>
    /// Pattern for a package with a single content of «Application» type
    /// </summary>
    public IReadOnlyList<ApplicationPatternPart> ApplicationPattern { get; }

    /// <summary>
    /// Pattern for a package with a single content of «Patch» type
    /// </summary>
    public IReadOnlyList<PatchPatternPart> PatchPattern { get; }
}