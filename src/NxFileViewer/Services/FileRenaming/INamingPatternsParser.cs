using Emignatik.NxFileViewer.Services.FileRenaming.Exceptions;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
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
    Pattern ParseApplicationPattern(string pattern);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    /// <exception cref="KeywordUnknownException"></exception>
    /// <exception cref="UnexpectedDelimiterException"></exception>
    /// <exception cref="EndDelimiterMissingException"></exception>
    /// <exception cref="EmptyPatternException"></exception>
    Pattern ParsePatchPattern(string pattern);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    /// <exception cref="KeywordUnknownException"></exception>
    /// <exception cref="UnexpectedDelimiterException"></exception>
    /// <exception cref="EndDelimiterMissingException"></exception>
    /// <exception cref="EmptyPatternException"></exception>
    Pattern ParseAddonPattern(string pattern);
}