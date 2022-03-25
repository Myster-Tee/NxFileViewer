using System;
using Emignatik.NxFileViewer.Localization;

namespace Emignatik.NxFileViewer.Tools.DelimitedTextParsing;

public class EndDelimiterMissingException : Exception
{
    public char MissingDelimiter { get; }

    public EndDelimiterMissingException(char missingDelimiter)
    {
        MissingDelimiter = missingDelimiter;
    }
    
    public override string Message => LocalizationManager.Instance.Current.Keys.Exception_EndDelimiterMissing.SafeFormat(MissingDelimiter);
}