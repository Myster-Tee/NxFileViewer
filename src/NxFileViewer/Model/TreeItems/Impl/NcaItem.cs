using System;
using System.Collections.Generic;
using LibHac.Common;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.Tools.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    /// <summary>
    /// <see cref="Nca"/> wrapper
    /// </summary>
    public class NcaItem : PartitionFileEntryItemBase
    {
        private Validity _headerSignatureValidity;

        public static int MaxSections { get; } = 4;

        public NcaItem(Nca nca, PartitionFileEntry partitionFileEntry, IFile file, PartitionFileSystemItemBase parentItem)
            : base(partitionFileEntry, file, parentItem)
        {
            Nca = nca ?? throw new ArgumentNullException(nameof(nca));
            Id = PartitionFileEntry.Name.Split('.', 2)[0];
        }

        public sealed override List<SectionItem> ChildItems { get; } = new();

        public Nca Nca { get; }

        public override string LibHacUnderlyingTypeName => nameof(Nca);

        public string Id { get; }

        public TitleVersion SdkVersion => Nca.Header.SdkVersion;

        public NcaContentType ContentType => Nca.Header.ContentType;

        public DistributionType DistributionType => Nca.Header.DistributionType;

        public int ContentIndex => Nca.Header.ContentIndex;

        public byte KeyGeneration => Nca.Header.KeyGeneration;

        public NcaVersion FormatVersion => Nca.Header.FormatVersion;

        public bool IsNca0 => Nca.Header.IsNca0();

        public bool IsEncrypted => Nca.Header.IsEncrypted;

        public string FileName => PartitionFileEntry.Name;

        public override string DisplayName => $"{FileName} ({Nca.Header.ContentType})";

        public Validity HeaderSignatureValidity
        {
            get => _headerSignatureValidity;
            internal set
            {
                _headerSignatureValidity = value;
                NotifyPropertyChanged();
            }
        }

    }
}