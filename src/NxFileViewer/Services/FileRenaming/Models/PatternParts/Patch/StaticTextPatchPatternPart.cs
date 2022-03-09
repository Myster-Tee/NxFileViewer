namespace Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Patch;

public class StaticTextPatchPatternPart : PatchPatternPart
{
    public string Text { get; }

    public StaticTextPatchPatternPart(string text)
    {
        Text = text;
    }
}