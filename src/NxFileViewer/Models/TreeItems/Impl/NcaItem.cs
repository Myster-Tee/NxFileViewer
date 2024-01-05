using System;
using System.Collections.Generic;
using LibHac.Common;
using LibHac.Fs.Fsa;
using LibHac.Tools.Fs;
using LibHac.Tools.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

/// <summary>
/// <see cref="Nca"/> wrapper
/// </summary>
public class NcaItem : PartitionFileEntryItemBase
{
    private Validity _headerSignatureValidity;

    public static int MaxSections { get; } = 4;

    public NcaItem(Nca nca, DirectoryEntryEx ncaFileEntry, IFile file, PartitionFileSystemItemBase parentItem)
        : base(ncaFileEntry, file, parentItem)
    {
        Nca = nca ?? throw new ArgumentNullException(nameof(nca));
        Id = FileEntry.Name.Split('.', 2)[0];
    }

    public sealed override List<SectionItemBase> ChildItems { get; } = new();

    public Nca Nca { get; }

    public override string Format => nameof(Nca);

    public string Id { get; }

    public TitleVersion SdkVersion => Nca.Header.SdkVersion;

    public NcaContentType ContentType => Nca.Header.ContentType;

    public DistributionType DistributionType => Nca.Header.DistributionType;

    public int ContentIndex => Nca.Header.ContentIndex;

    public byte KeyGeneration => Nca.Header.KeyGeneration;

    public NcaVersion FormatVersion => Nca.Header.FormatVersion;

    public bool IsNca0 => Nca.Header.IsNca0();

    public bool IsEncrypted => Nca.Header.IsEncrypted;

    public string FileName => FileEntry.Name;

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