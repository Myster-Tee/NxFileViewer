namespace Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Addon;

public class StaticTextAddonPatternPart : AddonPatternPart
{
    public string Text { get; }

    public StaticTextAddonPatternPart(string text)
    {
        Text = text;
    }
}