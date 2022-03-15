namespace Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;

public class DynamicTextPatternPart : PatternPart
{
    public PatternKeyword Keyword { get; }

    public StringOperation StringOperation { get; }

    public DynamicTextPatternPart(PatternKeyword keyword, StringOperation stringOperation)
    {
        Keyword = keyword;
        StringOperation = stringOperation;
    }
}



public enum StringOperation
{
    Untouched,
    ToLower,
    ToUpper,
}