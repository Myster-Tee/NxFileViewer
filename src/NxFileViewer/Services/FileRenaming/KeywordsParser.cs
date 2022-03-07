using System;
using Emignatik.NxFileViewer.Tools.DelimitedTextParsing;

namespace Emignatik.NxFileViewer.Services.FileRenaming;

public class KeywordsParser : DelimitedTextParserBase
{
    public KeywordsParser() : base('{', '}', '\\')
    {
    }
    public Action<string>? OnStaticTextFound { get; set; }

    public Action<string>? OnKeywordFound { get; set; }

    protected override void OnOuterTextFound(string text)
    {
        OnStaticTextFound?.Invoke(text);
    }

    protected override void OnDelimitedTextFound(string text)
    {
        OnKeywordFound?.Invoke(text);
    }

    public void Parse(string text)
    {
        base.ParseInternal(text);
    }
}