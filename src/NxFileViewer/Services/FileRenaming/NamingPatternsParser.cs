using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Services.FileRenaming.Exceptions;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;
using Emignatik.NxFileViewer.Tools.DelimitedTextParsing;

namespace Emignatik.NxFileViewer.Services.FileRenaming;

public class NamingPatternsParser : INamingPatternsParser
{

    private readonly DelimitedTextParser _keywordsParser = new('{', '}', '\\');

    public List<PatternPart> ParseApplicationPattern(string pattern)
    {
        var allowedApplicationKeywords = PatternKeywords.GetAllowedApplicationKeywords();

        return ParsePatternsInternal(pattern, allowedApplicationKeywords, PatternType.Application).ToList();
    }

    public List<PatternPart> ParsePatchPattern(string pattern)
    {
        var allowedPatchKeywords = PatternKeywords.GetAllowedPatchKeywords();
        return ParsePatternsInternal(pattern, allowedPatchKeywords, PatternType.Patch).ToList();
    }

    public List<PatternPart> ParseAddonPattern(string pattern)
    {
        var allowedAddonKeywords = PatternKeywords.GetAllowedAddonKeywords();
        return ParsePatternsInternal(pattern, allowedAddonKeywords, PatternType.Addon).ToList();
    }

    private IEnumerable<PatternPart> ParsePatternsInternal(string pattern, IReadOnlyList<PatternKeyword> allowedKeywords, PatternType patternType)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new EmptyPatternException();

        foreach (var (text, isDelimited) in _keywordsParser.Parse(pattern))
        {
            if (isDelimited)
            {
                if (!Enum.TryParse<PatternKeyword>(text, true, out var patternKeyword))
                {
                    var allowedKeywordFormatted = allowedKeywords.Select(type => _keywordsParser.StartDelimiter + type.ToString() + _keywordsParser.EndDelimiter);

                    throw new KeywordUnknownException(text, allowedKeywordFormatted);
                }

                if (!allowedKeywords.Contains(patternKeyword))
                    throw new KeywordNotAllowedException(patternKeyword, patternType);

                yield return new DynamicTextPatternPart(patternKeyword);
            }
            else
            {
                yield return new StaticTextPatternPart(text);
            }
        }
    }

}