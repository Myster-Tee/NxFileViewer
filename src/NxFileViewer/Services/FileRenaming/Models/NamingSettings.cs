using System.Collections.Generic;
using System.Text;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Models;

public class NamingSettings : INamingSettings
{
    IPattern INamingSettings.ApplicationPattern => ApplicationPattern;

    public Pattern ApplicationPattern { get; set; } = new();

    IPattern INamingSettings.PatchPattern => PatchPattern;

    public Pattern PatchPattern { get; set; } = new();

    IPattern INamingSettings.AddonPattern => AddonPattern;

    public Pattern AddonPattern { get; set; } = new();

    public string? InvalidFileNameCharsReplacement { get; set; }

    public bool ReplaceWhiteSpaceChars { get; set; }

    public string? WhiteSpaceCharsReplacement { get; set; }

}

public class Pattern : List<PatternPart>, IPattern
{
    public override string ToString()
    {
        return this.Serialize();
    }

    public string Serialize()
    {
        var sb = new StringBuilder();
        foreach (var part in this)
            sb.Append(part.Serialize());
        return sb.ToString();
    }
}