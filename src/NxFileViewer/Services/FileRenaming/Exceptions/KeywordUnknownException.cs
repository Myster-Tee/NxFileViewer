using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Localization;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Exceptions;

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