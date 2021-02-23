using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.FileLoading;
using LibHac.Fs.Fsa;
using LibHac.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class SectionItem : ItemBase
    {
        private IReadOnlyList<DirectoryEntryItem>? _dirEntries;

        public SectionItem(int sectionIndex, NcaFsHeader ncaFsHeader, IFileSystem fileSystem, NcaItem parentNcaItem, IChildItemsBuilder childItemsBuilder)
            : base(childItemsBuilder)
        {
            FsHeader = ncaFsHeader;
            SectionIndex = sectionIndex;
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            ParentNcaItem = parentNcaItem ?? throw new ArgumentNullException(nameof(parentNcaItem));
        }

        public NcaFsHeader FsHeader { get; }

        public override string LibHacTypeName => "Section";

        public override string? LibHacUnderlyingTypeName => FsHeader.FormatType.ToString();

        public override string Name => SectionIndex.ToString();

        public override string DisplayName => $"Section {SectionIndex}";

        public NcaItem ParentNcaItem { get; }

        public override IItem ParentItem => ParentNcaItem;

        public IReadOnlyList<DirectoryEntryItem> ChildDirectoryEntryItems => GetChildDirectoryEntryItems();

        public IFileSystem FileSystem { get; }

        public NcaEncryptionType EncryptionType => FsHeader.EncryptionType;

        public NcaFormatType FormatType => FsHeader.FormatType;

        public NcaHashType HashType => FsHeader.HashType;

        public short Version => FsHeader.Version;

        public int SectionIndex { get; }

        protected sealed override IReadOnlyList<IItem> SafeLoadChildItemsInternal()
        {
            return GetChildDirectoryEntryItems();
        }

        private IReadOnlyList<DirectoryEntryItem> GetChildDirectoryEntryItems()
        {
            return _dirEntries ??= ChildItemsBuilder.Build(this);
        }
    }
}