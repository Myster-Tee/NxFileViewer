using System;
using Emignatik.NxFileViewer.Utils;
using LibHac.Loader;
using LibHac.Tools.Fs;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public class MainItem : DirectoryEntryItem
{

    public MainItem(NsoHeader nsoHeader, FsSectionItem parentItem, DirectoryEntryEx directoryEntry)
        : base(parentItem, directoryEntry)
    {
        ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
        NsoHeader = nsoHeader;
    }

    public new FsSectionItem ParentItem { get; }

    public NsoHeader NsoHeader { get; }

    public sealed override string Format => "Nso";

    public string ModuleId => NsoHeader.ModuleId.Items.ToStrId();

}