using System;

namespace Emignatik.NxFileViewer.Tools.Parsing;

public class EndDelimiterMissingException : Exception
{
    public char MissingDelimiter { get; }

    public EndDelimiterMissingException(char missingDelimiter)
    {
        MissingDelimiter = missingDelimiter;
    }
}