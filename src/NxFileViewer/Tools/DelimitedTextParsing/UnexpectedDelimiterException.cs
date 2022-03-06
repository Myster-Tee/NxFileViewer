using System;
using Emignatik.NxFileViewer.Localization;

namespace Emignatik.NxFileViewer.Tools.DelimitedTextParsing;

public class UnexpectedDelimiterException : Exception
{
    public char Delimiter { get; }

    public int Position { get; }

    public char EscapeChar { get; }

    public UnexpectedDelimiterException(char delimiter, int position, char escapeChar)
    {
        Delimiter = delimiter;
        Position = position;
        EscapeChar = escapeChar;
    }

    public override string Message => LocalizationManager.Instance.Current.Keys.Exception_UnexpectedDelimiter.SafeFormat(Delimiter, Position, EscapeChar);
}