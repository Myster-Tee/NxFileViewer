namespace Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;

public abstract class PatternPart
{
    public abstract string Serialize();

    public override string ToString()
    {
        return Serialize();
    }
}