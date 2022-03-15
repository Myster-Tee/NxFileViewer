using System;
using Emignatik.NxFileViewer.Localization;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Exceptions;

public class BadInvalidFileNameCharReplacementException : Exception
{
    public string InvalidFileNameCharReplacement { get; }

    public char InvalidFileNameChar { get; }

    public BadInvalidFileNameCharReplacementException(string invalidFileNameCharReplacement, char invalidFileNameChar)
    {
        InvalidFileNameCharReplacement = invalidFileNameCharReplacement;
        InvalidFileNameChar = invalidFileNameChar;
    }

    public override string Message => LocalizationManager.Instance.Current.Keys.RenamingTool_BadInvalidFileNameCharReplacement.SafeFormat(InvalidFileNameCharReplacement, InvalidFileNameChar);
}