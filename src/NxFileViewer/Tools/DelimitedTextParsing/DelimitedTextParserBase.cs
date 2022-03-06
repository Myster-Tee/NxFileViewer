using System;
using System.Text;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.Tools.DelimitedTextParsing;

public abstract class DelimitedTextParserBase
{
    public DelimitedTextParserBase(char startDelimiter, char endDelimiter, char escapeChar)
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

    protected void ParseInternal(string inputText)
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
                        OnOuterTextFound(buffer.ToString());
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
                    OnDelimitedTextFound(buffer.ToString());
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
            OnOuterTextFound(buffer.ToString());

    }

    protected abstract void OnOuterTextFound(string text);

    protected abstract void OnDelimitedTextFound(string text);

}