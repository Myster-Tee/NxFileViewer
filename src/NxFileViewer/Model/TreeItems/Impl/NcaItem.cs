using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class NcaItem : PartitionFileEntryItem
    {
        private IReadOnlyList<SectionItem>? _sections;

        public NcaItem(Nca nca, PartitionFileEntry partitionFileEntry, IFile openFile, PartitionFileSystemItem parentPartitionFileSystemItem, IChildItemsBuilder childItemsBuilder)
            : base(partitionFileEntry, openFile, parentPartitionFileSystemItem, childItemsBuilder)
        {
            PartitionFileEntry = partitionFileEntry ?? throw new ArgumentNullException(nameof(partitionFileEntry));
            Nca = nca ?? throw new ArgumentNullException(nameof(nca));
            ParentPartitionFileSystemItem = parentPartitionFileSystemItem ?? throw new ArgumentNullException(nameof(parentPartitionFileSystemItem));

            Id = PartitionFileEntry.Name.Split('.')[0];
        }

        public string Id { get; }

        public Nca Nca { get; }

        [PropertiesView]
        public string UnderlyingType => nameof(Nca);

        [PropertiesView]
        public NcaContentType NcaType => Nca.Header.ContentType;

        [PropertiesView]
        public TitleVersion SdkVersion => Nca.Header.SdkVersion;

        [PropertiesView]
        public DistributionType DistributionType => Nca.Header.DistributionType;

        [PropertiesView]
        public byte KeyGeneration => Nca.Header.KeyGeneration;

        [PropertiesView]
        public int ContentIndex => Nca.Header.ContentIndex;

        [PropertiesView]
        public NcaVersion FormatVersion => Nca.Header.FormatVersion;

        [PropertiesView]
        public bool IsEncrypted => Nca.Header.IsEncrypted;

        public NcaContentType ContentType => Nca.Header.ContentType;

        public string FileName => PartitionFileEntry.Name;

        public override string DisplayName => $"{FileName} ({Nca.Header.ContentType})";

        public PartitionFileSystemItem ParentPartitionFileSystemItem { get; }

        public override IItem ParentItem => ParentPartitionFileSystemItem;

        public IReadOnlyList<SectionItem> Sections => GetSections(force: false);

        public PartitionFileEntry PartitionFileEntry { get; }


        public override IReadOnlyList<IItem> LoadChildItems(bool force)
        {
            return GetSections(force);
        }

        private IReadOnlyList<SectionItem> GetSections(bool force)
        {
            if (_sections == null || force)
                _sections = ChildItemsBuilder.Build(this);
            return _sections;
        }
    }
}