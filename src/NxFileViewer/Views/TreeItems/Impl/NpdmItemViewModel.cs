using System;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Services.Security;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.Common;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl;

public class NpdmItemViewModel : DirectoryEntryItemViewModel
{
    private readonly NpdmItem _npdmItem;

    public NpdmItemViewModel(NpdmItem npdmItem, IServiceProvider serviceProvider)
        : base(npdmItem, serviceProvider)
    {
        _npdmItem = npdmItem;
    }

    [PropertyView]
    public ProgramSecurityLevel SecurityLevel => _npdmItem.SecurityLevel;

    [PropertyView]
    public string FileSystemPermissions => $"0x{_npdmItem.FileSystemPermissions:x16}";

    [PropertyView]
    public Validity AcidSignatureValidity => _npdmItem.AcidSignatureValidity;

    [PropertyView]
    public string Services => string.Join(", ", _npdmItem.Services);
}
