using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<SectionItem>? _sections;

        public NcaItem(Nca nca, PartitionFileEntry partitionFileEntry, IFile openFile, PartitionFileSystemItem parentPartitionFileSystemItem, IChildItemsBuilder childItemsBuilder)
            : base(partitionFileEntry, openFile, parentPartitionFileSystemItem, childItemsBuilder)
        {
            PartitionFileEntry = partitionFileEntry ?? throw new ArgumentNullException(nameof(partitionFileEntry));
            Nca = nca ?? throw new ArgumentNullException(nameof(nca));
            ParentPartitionFileSystemItem = parentPartitionFileSystemItem ?? throw new ArgumentNullException(nameof(parentPartitionFileSystemItem));
            Id = PartitionFileEntry.Name.Split('.')[0];
        }

        [PropertiesView]
        public string UnderlyingType => nameof(Nca);

        [PropertiesView]
        public string NcaType => Nca.Header.ContentType.ToString();

        [PropertiesView]
        public string SdkVersion => Nca.Header.SdkVersion.ToString();

        [PropertiesView]
        public string DistributionType => Nca.Header.DistributionType.ToString();

        [PropertiesView]
        public string KeyGeneration => Nca.Header.KeyGeneration.ToString();

        [PropertiesView]
        public int ContentIndex => Nca.Header.ContentIndex;

        [PropertiesView]
        public string FormatVersion => Nca.Header.FormatVersion.ToString();

        [PropertiesView]
        public bool IsEncrypted => Nca.Header.IsEncrypted;

        public Nca Nca { get; }

        public NcaContentType ContentType => Nca.Header.ContentType;

        public string FileName => PartitionFileEntry.Name;

        public override string DisplayName => $"{FileName} ({Nca.Header.ContentType})";

        public PartitionFileSystemItem ParentPartitionFileSystemItem { get; }

        public override IItem ParentItem => ParentPartitionFileSystemItem;

        public IReadOnlyList<SectionItem> Sections => GetSections();

        public PartitionFileEntry PartitionFileEntry { get; }

        public string Id { get; }

        protected override IEnumerable<IItem> LoadChildItems()
        {
            return GetSections();
        }

        private IReadOnlyList<SectionItem> GetSections()
        {
            return _sections ??= ChildItemsBuilder.Build(this).ToList();
        }
    }
}