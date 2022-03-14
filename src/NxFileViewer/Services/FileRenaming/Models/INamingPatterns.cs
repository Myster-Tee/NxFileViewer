using System.Collections.Generic;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Addon;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Patch;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Models;

public interface INamingSettings
{
    /// <summary>
    /// Pattern for a package with a single content of «Application» type
    /// </summary>
    public IEnumerable<ApplicationPatternPart> ApplicationPattern { get; }

    /// <summary>
    /// Pattern for a package with a single content of «Patch» type
    /// </summary>
    public IEnumerable<PatchPatternPart> PatchPattern { get; }

    /// <summary>
    /// Pattern for a package with a single content of «Addon» type
    /// </summary>
    public IEnumerable<AddonPatternPart> AddonPattern { get; }

    /// <summary>
    /// Get the string to use for replacing invalid Windows chars
    /// </summary>
    public string? InvalidWindowsCharsReplacement { get; }
}