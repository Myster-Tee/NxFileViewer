using System.Collections.Generic;
using Emignatik.NxFileViewer.Tools.Parsing;
using Xunit;

namespace NxFileViewer.Test.Tools.Parsing;

public class DelimitedTextParserBaseTest
{
    private readonly DelimitedTextParser _delimitedTextParser;

    public DelimitedTextParserBaseTest()
    {
        _delimitedTextParser = new DelimitedTextParser();
    }

    [Fact]
    public void EmptyStringReturnsEmptyArray()
    {
        var textParts = _delimitedTextParser.Parse("");
        Assert.Empty(textParts);
    }

    [Fact]
    public void SimpleTestCase()
    {
        AssertTextParts(
            _delimitedTextParser.Parse("A[B]C"),
            new("A", false),
            new("B", true),
            new("C", false)
        );
    }

    [Fact]
    public void EmptyDelimitedStringsAreFound()
    {
        AssertTextParts(
            _delimitedTextParser.Parse("A[]C"),
            new("A", false),
            new("", true),
            new("C", false)
        );

        AssertTextParts(
            _delimitedTextParser.Parse("A[]B[]C"),
            new("A", false),
            new("", true),
            new("B", false),
            new("", true),
            new("C", false)
        );
    }

    [Fact]
    public void EmptyOuterStringsAreNotFound()
    {
        AssertTextParts(
            _delimitedTextParser.Parse("[AaA][bBb][Ccc]"),
            new("AaA", true),
            new("bBb", true),
            new("Ccc", true)
        );
    }

    [Fact]
    public void StartDelimitersCanBeEscaped()
    {
        AssertTextParts(
            _delimitedTextParser.Parse(@"\[\[\["),
            new TextPart("[[[", false)
        );
        AssertTextParts(
            _delimitedTextParser.Parse(@"a\[b\[\ccc"),
            new TextPart(@"a[b[\ccc", false)
        );
        AssertTextParts(
            _delimitedTextParser.Parse(@"a\[[hello]\[b"),
            new("a[", false),
            new("hello", true),
            new("[b", false)
        );
    }
    
    [Fact]
    public void EndDelimitersCanBeEscaped()
    {
        AssertTextParts(
            _delimitedTextParser.Parse(@"\]\]\]"),
            new TextPart("]]]", false)
        );
        AssertTextParts(
            _delimitedTextParser.Parse(@"a[\]]b"),
            new("a", false),
            new("]", true),
            new("b", false)
        );
        AssertTextParts(
            _delimitedTextParser.Parse(@"a[hello]\]b"),
            new("a", false),
            new("hello", true),
            new("]b", false)
        );
    }

    [Fact]
    public void EscapeCharIsTakenAsIsWhenNotPrecedingADelimiter()
    {
        AssertTextParts(
            _delimitedTextParser.Parse(@"\\\"),
            new TextPart(@"\\\", false)
        );
    }

    private static void AssertTextParts(IReadOnlyList<TextPart> actualTextParts, params TextPart[] expectedTextParts)
    {
        Assert.Equal(expectedTextParts.Length, actualTextParts.Count);

        for (var index = 0; index < expectedTextParts.Length; index++)
        {
            var expectedTextPart = expectedTextParts[index];
            var actualTextPart = actualTextParts[index];
            Assert.Equal(expectedTextPart.Text, actualTextPart.Text);
            Assert.Equal(expectedTextPart.IsDelimited, actualTextPart.IsDelimited);
        }
    }

    private class DelimitedTextParser : DelimitedTextParserBase
    {
        private readonly List<TextPart> _parts = new();
        public DelimitedTextParser() : base('[', ']', '\\')
        {
        }

        public TextPart[] Parse(string inputText)
        {
            _parts.Clear();
            base.ParseInternal(inputText);
            return _parts.ToArray();
        }

        protected override void OnOuterTextFound(string text)
        {
            _parts.Add(new TextPart(text, false));
        }

        protected override void OnDelimitedTextFound(string text)
        {
            _parts.Add(new TextPart(text, true));
        }


    }

    private class TextPart
    {
        public string Text { get; }
        public bool IsDelimited { get; }

        public TextPart(string text, bool isDelimited)
        {
            Text = text;
            IsDelimited = isDelimited;
        }
    }

}


