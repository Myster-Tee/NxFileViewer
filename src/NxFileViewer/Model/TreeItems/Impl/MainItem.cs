using System;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Loader;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class MainItem : DirectoryEntryItem
    {

        public MainItem(NsoHeader nsoHeader, SectionItem parentItem, DirectoryEntryEx directoryEntry)
            : base(parentItem, directoryEntry)
        {
            ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
            NsoHeader = nsoHeader;
        }

        public override SectionItem ParentItem { get; }

        public NsoHeader NsoHeader { get; }

        public override string LibHacUnderlyingTypeName => "Nso";

        public Buffer32 ModuleId => NsoHeader.ModuleId;

    }
}
