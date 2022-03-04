using System;

namespace Emignatik.NxFileViewer.Tools.Parsing;

public class UnexpectedDelimiterException : Exception
{
    public char Delimiter { get; }
    public int Position { get; }

    public UnexpectedDelimiterException(char delimiter, int position)
    {
        Delimiter = delimiter;
        Position = position;
    }
}