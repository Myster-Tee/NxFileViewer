using System;
using System.Text;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.Tools.DelimitedTextParsing;

public abstract class DelimitedTextParserBase
{
    private readonly char _startDelimiter;
    private readonly char _escapeChar;
    private readonly char _endDelimiter;

    public DelimitedTextParserBase(char startDelimiter, char endDelimiter, char escapeChar)
    {
        _startDelimiter = startDelimiter;
        _endDelimiter = endDelimiter;
        if (escapeChar == '\0')
            throw new ArgumentException($"{nameof(escapeChar)} can't be '\0'", nameof(escapeChar));
        _escapeChar = escapeChar;
    }

    protected void ParseInternal(string inputText)
    {
        var stringReader = new StringReader(inputText);

        var isInKeyword = false;
        var buffer = new StringBuilder();
        while (stringReader.ReadChar(out var c))
        {
            if (stringReader.PrevChar == _escapeChar)
            {
                if (c == _startDelimiter)
                {
                    buffer[^1] = _startDelimiter;
                    continue;
                }
                if (c == _endDelimiter)
                {
                    buffer[^1] = _endDelimiter;
                    continue;
                }
            }

            if (!isInKeyword)
            {
                if (c == _startDelimiter)
                {
                    if (buffer.Length > 0)
                        OnOuterTextFound(buffer.ToString());
                    buffer.Clear();
                    isInKeyword = true;
                }
                else if (c == _endDelimiter)
                {
                    throw new UnexpectedDelimiterException(c, stringReader.Position, _escapeChar);
                }
                else
                {
                    buffer.Append(c);
                }
            }
            else// if (isInKeyword)
            {
                if (c == _endDelimiter)
                {
                    OnDelimitedTextFound(buffer.ToString());
                    buffer.Clear();
                    isInKeyword = false;
                }
                else if (c == _startDelimiter)
                {
                    throw new UnexpectedDelimiterException(c, stringReader.Position, _escapeChar);
                }
                else
                {
                    buffer.Append(c);
                }
            }
        }

        if (isInKeyword)
            throw new EndDelimiterMissingException(_endDelimiter);

        if (buffer.Length > 0)
            OnOuterTextFound(buffer.ToString());

    }

    protected abstract void OnOuterTextFound(string text);

    protected abstract void OnDelimitedTextFound(string text);

}