using System;
using Emignatik.NxFileViewer.Localization;

namespace Emignatik.NxFileViewer.Services.FileRenaming.Exceptions;

public class EmptyDirectoryException : Exception
{
    public override string Message => LocalizationManager.Instance.Current.Keys.FileRenaming_EmptyDirectoryNotAllowed;

}