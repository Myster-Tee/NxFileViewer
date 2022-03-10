namespace Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Addon;

public class DynamicTextAddonPatternPart : AddonPatternPart
{
    public AddonKeyword Keyword { get; }

    public DynamicTextAddonPatternPart(AddonKeyword keyword)
    {
        Keyword = keyword;
    }
}