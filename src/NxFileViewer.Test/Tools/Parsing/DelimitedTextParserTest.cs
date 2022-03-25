using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Tools.DelimitedTextParsing;
using Xunit;

namespace NxFileViewer.Test.Tools.Parsing;

public class DelimitedTextParserTest
{
    private readonly DelimitedTextParser _delimitedTextParser;

    public DelimitedTextParserTest()
    {
        _delimitedTextParser = new DelimitedTextParser('[',']', '\\');
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

    private static void AssertTextParts(IEnumerable<TextPart> actualTextPartsRaw, params TextPart[] expectedTextParts)
    {
        var actualTextParts = actualTextPartsRaw.ToArray();

        Assert.Equal(expectedTextParts.Length, actualTextParts.Length);

        for (var index = 0; index < expectedTextParts.Length; index++)
        {
            var expectedTextPart = expectedTextParts[index];
            var actualTextPart = actualTextParts[index];
            Assert.Equal(expectedTextPart.Text, actualTextPart.Text);
            Assert.Equal(expectedTextPart.IsDelimited, actualTextPart.IsDelimited);
        }
    }

}


