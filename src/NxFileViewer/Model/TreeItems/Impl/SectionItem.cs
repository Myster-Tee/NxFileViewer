using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.Fs.Fsa;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class SectionItem : ItemBase
    {
        private readonly IChildItemsBuilder _childItemsBuilder;
        private List<DirectoryEntryItem>? _dirEntries;

        public SectionItem(int sectionIndex, IFileSystem fileSystem, NcaItem parentNcaItem, IChildItemsBuilder childItemsBuilder)
        {
            _childItemsBuilder = childItemsBuilder ?? throw new ArgumentNullException(nameof(childItemsBuilder));
            SectionIndex = sectionIndex;
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            ParentNcaItem = parentNcaItem ?? throw new ArgumentNullException(nameof(parentNcaItem));
        }

        public override string ObjectType => "Section";

        public override string DisplayName => $"Section {SectionIndex}";

        public NcaItem ParentNcaItem { get; }

        public override IItem ParentItem => ParentNcaItem;

        public IReadOnlyList<DirectoryEntryItem> ChildDirectoryEntryItems => GetChildDirectoryEntryItems();

        [PropertiesView]
        public int SectionIndex { get; }

        public IFileSystem FileSystem { get; }

        protected override IEnumerable<IItem> LoadChildItems()
        {
            return GetChildDirectoryEntryItems();
        }

        private List<DirectoryEntryItem> GetChildDirectoryEntryItems()
        {
            return _dirEntries ??= _childItemsBuilder.Build(this).ToList();
        }
    }
}