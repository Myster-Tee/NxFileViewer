using System;
using System.Linq;
using Emignatik.NxFileViewer.Services.FileRenaming.Exceptions;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;

namespace Emignatik.NxFileViewer.Services.FileRenaming;

public class NamingPatternsParser : INamingPatternsParser
{
    public INamingPatterns Parse(string basePattern)
    {
        var namingPatterns = new NamingPatterns();


        var keywordsParser = new KeywordsParser();

        keywordsParser.OnKeywordFound = keyword =>
        {
            if (!Enum.TryParse<ApplicationKeyword>(keyword, true, out var dynamicTextBaseType))
            {
                var allowedKeywords = Enum.GetValues<ApplicationKeyword>().Select(type => keywordsParser.StartDelimiter + type.ToString() + keywordsParser.EndDelimiter);
                throw new KeywordUnknownException(keyword, allowedKeywords);
            }

            namingPatterns.ApplicationPattern.Add(new DynamicTextApplicationPatternPart(dynamicTextBaseType));
        };

        keywordsParser.OnStaticTextFound = staticText =>
        {
            namingPatterns.ApplicationPattern.Add(new StaticTextApplicationPatternPart(staticText));
        };


        keywordsParser.Parse(basePattern);

        return namingPatterns;
    }
}