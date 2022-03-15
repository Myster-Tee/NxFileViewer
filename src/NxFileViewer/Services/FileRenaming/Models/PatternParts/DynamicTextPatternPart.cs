namespace Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;

public class DynamicTextPatternPart : PatternPart
{
    public PatternKeyword Keyword { get; }

    public DynamicTextPatternPart(PatternKeyword keyword)
    {
        Keyword = keyword;
    }
}