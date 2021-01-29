using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Loader;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class MainItem : DirectoryEntryItem
    {
        private readonly NsoHeader _nsoHeader;

        public MainItem(NsoHeader nsoHeader, SectionItem parentSectionItem, DirectoryEntry directoryEntry, string name, string path, IChildItemsBuilder childItemsBuilder)
            : base(parentSectionItem, directoryEntry, name, path, childItemsBuilder)
        {
            _nsoHeader = nsoHeader;
        }

        [PropertiesView]
        public string UnderlyingType => "NS0";

        [PropertiesView(DescriptionLocalizationKey = "MainModuleIdTooltip")]
        public Buffer32 ModuleId => _nsoHeader.ModuleId;

    }
}
