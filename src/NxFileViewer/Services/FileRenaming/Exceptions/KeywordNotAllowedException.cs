using System;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Exceptions;

public class KeywordNotAllowedException : Exception
{
    public PatternKeyword NotAllowedKeyword { get; }

    public PatternType PatternType { get; }

    public KeywordNotAllowedException(PatternKeyword notAllowedKeyword, PatternType patternType)
    {
        NotAllowedKeyword = notAllowedKeyword;
        PatternType = patternType;
    }

    public override string Message => LocalizationManager.Instance.Current.Keys.FileRenaming_PatternKeywordNotAllowed.SafeFormat(NotAllowedKeyword, PatternType);
}

public enum PatternType
{
    Application,
    Patch,
    Addon,
}