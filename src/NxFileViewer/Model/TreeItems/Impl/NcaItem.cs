using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.FileLoading;
using LibHac;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    /// <summary>
    /// <see cref="Nca"/> wrapper
    /// </summary>
    public class NcaItem : PartitionFileEntryItem
    {
        private IReadOnlyList<SectionItem>? _sections;
        private Validity _hashValidity = Validity.Unchecked;
        private Validity _headerSignatureValidity;

        public NcaItem(Nca nca, PartitionFileEntry partitionFileEntry, IFile file, PartitionFileSystemItem parentPartitionFileSystemItem, IChildItemsBuilder childItemsBuilder)
            : base(partitionFileEntry, file, parentPartitionFileSystemItem, childItemsBuilder)
        {
            Nca = nca ?? throw new ArgumentNullException(nameof(nca));

            Id = PartitionFileEntry.Name.Split('.', 2)[0];
        }

        public Nca Nca { get; }

        public override string LibHacUnderlyingTypeName => nameof(Nca);

        public string Id { get; }

        public TitleVersion SdkVersion => Nca.Header.SdkVersion;

        public NcaContentType ContentType => Nca.Header.ContentType;

        public DistributionType DistributionType => Nca.Header.DistributionType;

        public int ContentIndex => Nca.Header.ContentIndex;

        public byte KeyGeneration => Nca.Header.KeyGeneration;

        public NcaVersion FormatVersion => Nca.Header.FormatVersion;

        public bool IsEncrypted => Nca.Header.IsEncrypted;

        public string FileName => PartitionFileEntry.Name;

        public override string DisplayName => $"{FileName} ({Nca.Header.ContentType})";

        public override IItem ParentItem => ParentPartitionFileSystemItem;

        public IReadOnlyList<SectionItem> Sections => GetSections();

        public Validity HeaderSignatureValidity
        {
            get => _headerSignatureValidity;
            internal set
            {
                _headerSignatureValidity = value;
                NotifyPropertyChanged();
            }
        }

        public Validity HashValidity
        {
            get => _hashValidity;
            internal set
            {
                _hashValidity = value;
                NotifyPropertyChanged();
            }
        }

        protected sealed override IReadOnlyList<IItem> SafeLoadChildItemsInternal()
        {
            return GetSections();
        }

        private IReadOnlyList<SectionItem> GetSections()
        {
            return _sections ??= ChildItemsBuilder.Build(this);
        }
    }
}