using System;
using System.Collections.Generic;
using System.Text;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.Tools.DelimitedTextParsing;

public class DelimitedTextParser
{
    public DelimitedTextParser(char startDelimiter, char endDelimiter, char escapeChar)
    {
        StartDelimiter = startDelimiter;
        EndDelimiter = endDelimiter;
        if (escapeChar == '\0')
            throw new ArgumentException($"{nameof(escapeChar)} can't be '\0'", nameof(escapeChar));
        EscapeChar = escapeChar;
    }

    public char StartDelimiter { get; }

    public char EscapeChar { get; }

    public char EndDelimiter { get; }

    public IEnumerable<TextPart> Parse(string inputText)
    {
        var stringReader = new StringReader(inputText);

        var isInKeyword = false;
        var buffer = new StringBuilder();
        while (stringReader.ReadChar(out var c))
        {
            if (stringReader.PrevChar == EscapeChar)
            {
                if (c == StartDelimiter)
                {
                    buffer[^1] = StartDelimiter;
                    continue;
                }
                if (c == EndDelimiter)
                {
                    buffer[^1] = EndDelimiter;
                    continue;
                }
            }

            if (!isInKeyword)
            {
                if (c == StartDelimiter)
                {
                    if (buffer.Length > 0)
                        yield return new TextPart(buffer.ToString(), false);
                    buffer.Clear();
                    isInKeyword = true;
                }
                else if (c == EndDelimiter)
                {
                    throw new UnexpectedDelimiterException(c, stringReader.Position, EscapeChar);
                }
                else
                {
                    buffer.Append(c);
                }
            }
            else// if (isInKeyword)
            {
                if (c == EndDelimiter)
                {
                    yield return new TextPart(buffer.ToString(), true);

                    buffer.Clear();
                    isInKeyword = false;
                }
                else if (c == StartDelimiter)
                {
                    throw new UnexpectedDelimiterException(c, stringReader.Position, EscapeChar);
                }
                else
                {
                    buffer.Append(c);
                }
            }
        }

        if (isInKeyword)
            throw new EndDelimiterMissingException(EndDelimiter);

        if (buffer.Length > 0)
            yield return new TextPart(buffer.ToString(), false);

    }

}

public class TextPart
{
    public TextPart(string text, bool isDelimited)
    {
        Text = text;
        IsDelimited = isDelimited;
    }

    public string Text { get; }

    public bool IsDelimited { get; }

    // Return the first and last name.
    public void Deconstruct(out string text, out bool isDelimited)
    {
        text = Text;
        isDelimited = IsDelimited;
    }
}