using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LibHac.Common;
using LibHac.Tools.Fs;
using LibHac.Tools.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

/// <summary>
/// <see cref="Nca"/> wrapper
/// </summary>
public class NcaItem : PartitionFileEntryItemBase
{
    private Validity _headerSignatureValidity;
    private bool? _hashValid;

    public static int MaxSections { get; } = 4;

    public NcaItem(Nca nca, DirectoryEntryEx ncaFileEntry, PartitionFileSystemItemBase parentItem) : base(ncaFileEntry, parentItem)
    {
        Nca = nca ?? throw new ArgumentNullException(nameof(nca));
        Id = FileEntry.Name.Split('.', 2)[0];
    }

    public new SectionItem[] ChildItems => base.ChildItems.OfType<SectionItem>().ToArray();

    public Nca Nca { get; }

    public override string Format => nameof(Nca);

    /// <summary>
    /// Id of the NCA (name without extension), also corresponds to the NCA hash in hex
    /// </summary>
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

    /// <summary>
    /// Get the Validity of the NCA's header signature
    /// </summary>
    public Validity HeaderSignatureValidity
    {
        get => _headerSignatureValidity;
        internal set
        {
            _headerSignatureValidity = value;
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Get the Validity of the NCA's hash
    /// Null when not checked
    /// </summary>
    public bool? HashValid
    {
        get => _hashValid;
        set
        {
            _hashValid = value;
            NotifyPropertyChanged();
        }
    }

    public virtual Nca GetOriginalNca()
    {
        return Nca;
    }

    /// <summary>
    /// Try to get the expected hash from the NCA Id
    /// </summary>
    /// <param name="hash"></param>
    /// <returns></returns>
    public bool TryGetExpectedHashFromId([NotNullWhen(true)] out byte[]? hash)
    {
        try
        {

            var hex = Id;
            if (hex.Length % 2 != 0)
            {
                hash = null;
                return false;
            }

            hash = new byte[hex.Length / 2];
            var byteIndex = 0;
            for (var i = 0; i < hex.Length; i += 2)
            {
                var byteHex = hex.Substring(i, 2);
                hash[byteIndex++] = Convert.ToByte(byteHex, 16);
            }
        }
        catch
        {
            hash = null;
            return false;
        }
        return true;
    }
}