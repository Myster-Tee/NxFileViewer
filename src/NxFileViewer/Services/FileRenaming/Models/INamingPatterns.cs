using System.Collections.Generic;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Models;

public interface INamingPatterns
{
    /// <summary>
    /// Pattern for a package with a single content of «application» type
    /// </summary>
    public IReadOnlyList<ApplicationPatternPart> ApplicationPattern { get; }
}