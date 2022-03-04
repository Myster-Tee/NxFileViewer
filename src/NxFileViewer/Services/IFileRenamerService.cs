using System.Collections.Generic;

namespace Emignatik.NxFileViewer.Services;

public interface IFileRenamerService
{
    void Rename(string inputDirectory, string namingPattern, IReadOnlyCollection<string> fileExtensions, bool includeSubDirectories);
}


public abstract class Part
{
    public abstract string GetPart();
}

public class PatternSettings
{
    public string BasePattern { get; set; }

    public string PatchPattern { get; set; }

    public string AddonPattern { get; set; }
}