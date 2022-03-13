using System;
using Emignatik.NxFileViewer.Localization;

namespace Emignatik.NxFileViewer.Services.FileRenaming;

public class SuperPackageNotSupportedException : Exception
{
    public override string Message => LocalizationManager.Instance.Current.Keys.RenamingTool_SuperPackageNotSupported;
}