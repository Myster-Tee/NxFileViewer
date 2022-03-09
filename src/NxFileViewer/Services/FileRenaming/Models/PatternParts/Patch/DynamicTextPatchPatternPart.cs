namespace Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Patch;

public class DynamicTextPatchPatternPart : PatchPatternPart
{
    public PatchKeyword Keyword { get; }

    public DynamicTextPatchPatternPart(PatchKeyword keyword)
    {
        Keyword = keyword;
    }
}