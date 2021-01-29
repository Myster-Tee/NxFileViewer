using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.Fs.Fsa;
using LibHac.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class SectionItem : ItemBase
    {
        private readonly NcaFsHeader _ncaFsHeader;
        private readonly IChildItemsBuilder _childItemsBuilder;
        private IReadOnlyList<DirectoryEntryItem>? _dirEntries;

        public SectionItem(int sectionIndex, NcaFsHeader ncaFsHeader, IFileSystem fileSystem, NcaItem parentNcaItem,
            IChildItemsBuilder childItemsBuilder)
        {
            _ncaFsHeader = ncaFsHeader;
            _childItemsBuilder = childItemsBuilder ?? throw new ArgumentNullException(nameof(childItemsBuilder));
            SectionIndex = sectionIndex;
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            ParentNcaItem = parentNcaItem ?? throw new ArgumentNullException(nameof(parentNcaItem));
        }

        public override string ObjectType => "Section";

        public override string DisplayName => $"Section {SectionIndex}";

        public NcaItem ParentNcaItem { get; }

        public override IItem ParentItem => ParentNcaItem;

        public IReadOnlyList<DirectoryEntryItem> ChildDirectoryEntryItems => GetChildDirectoryEntryItems(force: false);

        [PropertiesView]
        public int SectionIndex { get; }

        [PropertiesView]
        public string EncryptionType => _ncaFsHeader.EncryptionType.ToString();

        [PropertiesView]
        public string FormatType => _ncaFsHeader.FormatType.ToString();

        [PropertiesView]
        public string HashType => _ncaFsHeader.HashType.ToString();

        [PropertiesView]
        public string Version => _ncaFsHeader.Version.ToString();

        public IFileSystem FileSystem { get; }

        public override IReadOnlyList<IItem> LoadChildItems(bool force)
        {
            return GetChildDirectoryEntryItems(force);
        }

        private IReadOnlyList<DirectoryEntryItem> GetChildDirectoryEntryItems(bool force)
        {
            if (_dirEntries == null || force)
                _dirEntries = _childItemsBuilder.Build(this);
            return _dirEntries;
        }
    }
}