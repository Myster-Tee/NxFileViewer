using Emignatik.NxFileViewer.FileLoading;
using LibHac;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Loader;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class MainItem : DirectoryEntryItem
    {

        public MainItem(NsoHeader nsoHeader, SectionItem parentSectionItem, DirectoryEntry directoryEntry, string name, string path, IChildItemsBuilder childItemsBuilder)
            : base(parentSectionItem, directoryEntry, name, path, childItemsBuilder)
        {
            NsoHeader = nsoHeader;
        }

        public NsoHeader NsoHeader { get; }

        public override string LibHacUnderlyingTypeName => nameof(Nso);

        public Buffer32 ModuleId => NsoHeader.ModuleId;

    }
}
