namespace Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;

public class DynamicTextApplicationPatternPart : ApplicationPatternPart
{
    public ApplicationKeyword Keyword { get; }

    public DynamicTextApplicationPatternPart(ApplicationKeyword keyword)
    {
        Keyword = keyword;
    }
}