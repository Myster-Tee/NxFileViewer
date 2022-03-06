using System;
using System.Collections.Generic;

namespace Emignatik.NxFileViewer.Services.FileRenaming;

public interface IFileRenamerService
{
    void RenameFromDirectory(string inputDirectory, INamingPatterns namingPatterns, IReadOnlyCollection<string> fileExtensions, bool includeSubDirectories);

    void RenameFile(string inputFile, INamingPatterns namingPatterns);
}


public abstract class PatternPart
{
}

public interface INamingPatterns
{
    public IReadOnlyList<ApplicationPatternPart> ApplicationPattern { get; }
}

public class NamingPatterns : INamingPatterns
{
    IReadOnlyList<ApplicationPatternPart> INamingPatterns.ApplicationPattern => ApplicationPattern;

    public List<ApplicationPatternPart> ApplicationPattern { get; } = new();

    public string PatchPattern { get; set; }

    public string AddonPattern { get; set; }
}


public abstract class ApplicationPatternPart : PatternPart
{
}


public class StaticTextApplicationPatternPart : ApplicationPatternPart
{
    public string Text { get; }

    public StaticTextApplicationPatternPart(string text)
    {
        Text = text;
    }
}

public class DynamicTextApplicationPatternPart : ApplicationPatternPart
{
    public DynamicTextBaseType Type { get; }

    public DynamicTextApplicationPatternPart(DynamicTextBaseType type)
    {
        Type = type;
    }
}


public enum DynamicTextBaseType
{
    TitleId,
    FirstTitleName,
}