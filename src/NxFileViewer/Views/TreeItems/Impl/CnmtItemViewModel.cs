using System;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.Ncm;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl;

public class CnmtItemViewModel : DirectoryEntryItemViewModel
{
    private readonly CnmtItem _cnmtItem;

    public CnmtItemViewModel(CnmtItem cnmtItem, IServiceProvider serviceProvider)
        : base(cnmtItem, serviceProvider)
    {
        _cnmtItem = cnmtItem;
    }

    [PropertyView]
    public ContentMetaType ContentType => _cnmtItem.ContentType;

    [PropertyView]
    public string TitleId => _cnmtItem.TitleId;

    [PropertyView]
    public string ApplicationTitleId => _cnmtItem.ApplicationTitleId;

    [PropertyView]
    public string PatchTitleId => _cnmtItem.PatchTitleId;

    [PropertyView]
    public string? TitleVersion => _cnmtItem.TitleVersion;

    [PropertyView] 
    public int? PatchLevel => _cnmtItem.PatchNumber;

    [PropertyView]
    public string? MinimumApplicationVersion => _cnmtItem.MinimumApplicationVersion?.ToString();

    [PropertyView]
    public string? MinimumSystemVersion => _cnmtItem.MinimumSystemVersion?.ToString();

}