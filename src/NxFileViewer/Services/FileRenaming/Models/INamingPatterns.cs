using System.Collections.Generic;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Models;

public interface INamingSettings
{
    /// <summary>
    /// Pattern for a package with a single content of «Application» type
    /// </summary>
    public IReadOnlyList<PatternPart> ApplicationPattern { get; }

    /// <summary>
    /// Pattern for a package with a single content of «Patch» type
    /// </summary>
    public IReadOnlyList<PatternPart> PatchPattern { get; }

    /// <summary>
    /// Pattern for a package with a single content of «Addon» type
    /// </summary>
    public IReadOnlyList<PatternPart> AddonPattern { get; }

    /// <summary>
    /// Get the string to use for replacing invalid Windows chars
    /// </summary>
    public string? InvalidFileNameCharsReplacement { get; }

    /// <summary>
    /// Get a boolean indicating if white space chars should be replaced with the value defined by <see cref="WhiteSpaceCharsReplacement"/>
    /// </summary>
    public bool ReplaceWhiteSpaceChars { get; }

    /// <summary>
    /// Get the string to use for replacing white space chars
    /// </summary>
    string? WhiteSpaceCharsReplacement { get; }
}