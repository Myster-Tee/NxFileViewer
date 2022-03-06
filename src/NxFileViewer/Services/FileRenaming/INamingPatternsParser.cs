using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Localization;

namespace Emignatik.NxFileViewer.Services.FileRenaming
{
    public interface INamingPatternsParser
    {
        NamingPatterns Parse(string basePattern);
    }

    public class NamingPatternsParser : INamingPatternsParser
    {
        public NamingPatterns Parse(string basePattern)
        {
            var namingPatterns = new NamingPatterns();


            var keywordsParser = new KeywordsParser();

            keywordsParser.OnKeywordFound = keyword =>
            {

                if (!Enum.TryParse<DynamicTextBaseType>(keyword, true, out var dynamicTextBaseType))
                {
                    var allowedKeywords = Enum.GetValues<DynamicTextBaseType>().Select(type => keywordsParser.StartDelimiter + type.ToString() + keywordsParser.EndDelimiter);
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

    public class KeywordUnknownException : Exception
    {
        private readonly string _unknownKeyword;
        private readonly string _allowedKeywords;

        public KeywordUnknownException(string unknownKeyword, IEnumerable<string> allowedKeywords)
        {
            _unknownKeyword = unknownKeyword;


            _allowedKeywords = string.Join(", ", allowedKeywords);
        }

        public override string Message => LocalizationManager.Instance.Current.Keys.FileRenaming_PatternKeywordUnknown.SafeFormat(_unknownKeyword, _allowedKeywords);
    }
}
