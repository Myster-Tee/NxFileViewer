using System.Collections.Generic;
using Emignatik.NxFileViewer.Services.FileRenaming.Exceptions;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Addon;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Patch;
using Emignatik.NxFileViewer.Tools.DelimitedTextParsing;

namespace Emignatik.NxFileViewer.Services.FileRenaming;

public interface INamingPatternsParser
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    /// <exception cref="KeywordUnknownException"></exception>
    /// <exception cref="UnexpectedDelimiterException"></exception>
    /// <exception cref="EndDelimiterMissingException"></exception>
    /// <exception cref="EmptyPatternException"></exception>
    List<ApplicationPatternPart> ParseApplicationPattern(string pattern);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    /// <exception cref="KeywordUnknownException"></exception>
    /// <exception cref="UnexpectedDelimiterException"></exception>
    /// <exception cref="EndDelimiterMissingException"></exception>
    /// <exception cref="EmptyPatternException"></exception>
    List<PatchPatternPart> ParsePatchPattern(string pattern);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    /// <exception cref="KeywordUnknownException"></exception>
    /// <exception cref="UnexpectedDelimiterException"></exception>
    /// <exception cref="EndDelimiterMissingException"></exception>
    /// <exception cref="EmptyPatternException"></exception>
    List<AddonPatternPart> ParseAddonPattern(string pattern);
}