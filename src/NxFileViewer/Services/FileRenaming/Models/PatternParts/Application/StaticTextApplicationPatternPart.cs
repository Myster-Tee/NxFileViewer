namespace Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;

public class StaticTextApplicationPatternPart : ApplicationPatternPart
{
    public string Text { get; }

    public StaticTextApplicationPatternPart(string text)
    {
        Text = text;
    }
}