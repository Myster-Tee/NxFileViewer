namespace Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;

public class StaticTextPatternPart : PatternPart
{
    public string Text { get; }

    public StaticTextPatternPart(string text)
    {
        Text = text;
    }

    public override string Serialize()
    {
        return Text;
    }
}