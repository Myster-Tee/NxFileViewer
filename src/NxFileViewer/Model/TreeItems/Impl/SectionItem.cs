using System;
using System.Collections.Generic;
using LibHac.Common;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.Tools.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class SectionItem : ItemBase
    {
        private Validity _hashValidity = Validity.Unchecked;

        public SectionItem(int sectionIndex, NcaFsHeader ncaFsHeader, NcaItem parentItem)
        {
            FsHeader = ncaFsHeader;
            SectionIndex = sectionIndex;
            ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
        }

        public override NcaItem ParentItem { get; }

        public override List<DirectoryEntryItem> ChildItems { get; } = new();

        public NcaFsHeader FsHeader { get; }

        public override string LibHacTypeName => "Section";

        public override string? LibHacUnderlyingTypeName => FsHeader.FormatType.ToString();

        public override string Name => SectionIndex.ToString();

        public override string DisplayName => $"Section {SectionIndex}";

        public bool IsPatchSection => FsHeader.IsPatchSection();

        /// <summary>
        /// Get the FileSystem of this section.
        /// Can be null when the FileSystem of this section couldn't be opened.
        /// </summary>
        public IFileSystem? FileSystem { get; internal set; }

        public NcaEncryptionType EncryptionType => FsHeader.EncryptionType;

        public NcaFormatType FormatType => FsHeader.FormatType;

        public NcaHashType HashType => FsHeader.HashType;

        public short Version => FsHeader.Version;

        public int SectionIndex { get; }

        public Validity HashValidity
        {
            get => _hashValidity;
            internal set
            {
                _hashValidity = value;
                NotifyPropertyChanged();
            }
        }

        public override void Dispose()
        {
            FileSystem?.Dispose();
        }
    }
}