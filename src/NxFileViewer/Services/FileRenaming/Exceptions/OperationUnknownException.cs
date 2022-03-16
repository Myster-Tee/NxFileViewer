using System;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Exceptions;

public class StringOperatorUnknownException : Exception
{
    public string OperatorRaw { get; }

    public StringOperatorUnknownException(string operatorRaw)
    {
        OperatorRaw = operatorRaw;
    }

    public override string Message
    {
        get
        {
            var allowedValues = string.Join(", ",StringOperatorHelper.GetAllowedSerializedValues());
            return LocalizationManager.Instance.Current.Keys.FileRenaming_StringOperatorUnknown.SafeFormat(OperatorRaw, allowedValues);
        }
    }
}