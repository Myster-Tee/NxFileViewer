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
        var allowedApplicationKeywords = PatternKeywordHelper.GetAllowedApplicationKeywords();

        return ParsePatternsInternal(pattern, allowedApplicationKeywords, PatternType.Application).ToList();
    }

    public List<PatternPart> ParsePatchPattern(string pattern)
    {
        var allowedPatchKeywords = PatternKeywordHelper.GetAllowedPatchKeywords();
        return ParsePatternsInternal(pattern, allowedPatchKeywords, PatternType.Patch).ToList();
    }

    public List<PatternPart> ParseAddonPattern(string pattern)
    {
        var allowedAddonKeywords = PatternKeywordHelper.GetAllowedAddonKeywords();
        return ParsePatternsInternal(pattern, allowedAddonKeywords, PatternType.Addon).ToList();
    }

    private IEnumerable<PatternPart> ParsePatternsInternal(string pattern, IReadOnlyList<PatternKeyword> allowedKeywords, PatternType patternType)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new EmptyPatternException();

        foreach (var (text, isDelimited) in _keywordsParser.Parse(pattern))
        {
            if (!isDelimited)
            {
                // Static part
                yield return new StaticTextPatternPart(text);
            }
            else
            {
                // Dynamic part
                var parts = text.Split(':', 2);
                var keywordRaw = parts[0];
                var operatorRaw = parts.Length == 2 ? parts[1] : null;

                if (!PatternKeywordHelper.TryParse(keywordRaw, out var nullablePatternKeyword))
                {
                    var allowedKeywordsStr = PatternKeywordHelper.GetCorrespondingSerializedValues(allowedKeywords)
                        .Select(type => _keywordsParser.StartDelimiter + type.ToString() + _keywordsParser.EndDelimiter);

                    throw new KeywordUnknownException(keywordRaw, allowedKeywordsStr);
                }
                var patternKeyword = nullablePatternKeyword.Value;

                if (!allowedKeywords.Contains(patternKeyword))
                    throw new KeywordNotAllowedException(patternKeyword, patternType);


                StringOperator stringOperator;
                if (operatorRaw != null)
                {
                    if (!StringOperatorHelper.TryParse(operatorRaw, out var nullableStringOperator))
                        throw new StringOperatorUnknownException(operatorRaw);

                    stringOperator = nullableStringOperator.Value;
                }
                else
                {
                    stringOperator = StringOperator.Untouched;
                }

                yield return new DynamicTextPatternPart(patternKeyword, stringOperator);
            }
        }
    }

}