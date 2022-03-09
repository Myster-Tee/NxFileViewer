using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Services.FileRenaming.Exceptions;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Patch;
using Emignatik.NxFileViewer.Tools.DelimitedTextParsing;

namespace Emignatik.NxFileViewer.Services.FileRenaming;

public class NamingPatternsParser : INamingPatternsParser
{

    private readonly DelimitedTextParser _keywordsParser = new('{', '}', '\\');

    public List<ApplicationPatternPart> ParseApplicationPattern(string pattern)
    {
        return ParseApplicationPatternsInternal(pattern).ToList();
    }

    public List<PatchPatternPart> ParsePatchPattern(string pattern)
    {
        return ParsePatchPatternInternal(pattern).ToList();
    }

    private IEnumerable<ApplicationPatternPart> ParseApplicationPatternsInternal(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new EmptyPatternException();

        foreach (var (text, isDelimited) in _keywordsParser.Parse(pattern))
        {
            if (isDelimited)
            {
                if (!Enum.TryParse<ApplicationKeyword>(text, true, out var applicationKeyword))
                {
                    var allowedKeywords = Enum.GetValues<ApplicationKeyword>().Select(type => _keywordsParser.StartDelimiter + type.ToString() + _keywordsParser.EndDelimiter);

                    throw new KeywordUnknownException(text, allowedKeywords);
                }

                yield return new DynamicTextApplicationPatternPart(applicationKeyword);
            }
            else
            {
                yield return new StaticTextApplicationPatternPart(text);
            }
        }
    }

    private IEnumerable<PatchPatternPart> ParsePatchPatternInternal(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new EmptyPatternException();

        foreach (var (text, isDelimited) in _keywordsParser.Parse(pattern))
        {
            if (isDelimited)
            {
                if (!Enum.TryParse<PatchKeyword>(text, true, out var patchKeyword))
                {
                    var allowedKeywords = Enum.GetValues<PatchKeyword>().Select(type => _keywordsParser.StartDelimiter + type.ToString() + _keywordsParser.EndDelimiter);

                    throw new KeywordUnknownException(text, allowedKeywords);
                }

                yield return new DynamicTextPatchPatternPart(patchKeyword);
            }
            else
            {
                yield return new StaticTextPatchPatternPart(text);
            }
        }
    }

}