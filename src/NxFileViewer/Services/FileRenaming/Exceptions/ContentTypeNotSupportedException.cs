using System;
using Emignatik.NxFileViewer.Localization;
using LibHac.Ncm;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Exceptions;

public class ContentTypeNotSupportedException : Exception
{
    public ContentMetaType ContentType { get; }

    public ContentTypeNotSupportedException(ContentMetaType contentType)
    {
        ContentType = contentType;
    }

    public override string Message => LocalizationManager.Instance.Current.Keys.RenamingTool_ContentTypeNotSupported.SafeFormat(ContentType);
}