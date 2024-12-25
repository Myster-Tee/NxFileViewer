using LibHac.Tools.FsSystem;
using LibHac.Tools.FsSystem.NcaUtils;

namespace LibHac.NSZ.Utils;

/// <summary>
/// Helper for restoring original NCA using encryption defined in each <see cref="NczSection"/> header
/// </summary>
public class NczSectionsEncryptionHelper
{
    private readonly IReadOnlyList<NczSection> _nczSections;

    /// <summary>
    /// </summary>
    /// <param name="nczSections">The list of section headers</param>
    /// <exception cref="ArgumentNullException"></exception>
    public NczSectionsEncryptionHelper(IReadOnlyList<NczSection> nczSections)
    {
        _nczSections = nczSections ?? throw new ArgumentNullException(nameof(nczSections));
    }

    /// <summary>
    /// Recovers the raw NCZ decompressed buffer to the original NCA buffer
    /// </summary>
    /// <param name="ncaOffset">The NCA byte offset (including NCA header) for which to recover original bytes</param>
    /// <param name="buffer">Input/output buffer containing raw decompressed data to be recovered</param>
    public int Recover(long ncaOffset, Span<byte> buffer)
    {
        var nbBytesWritten = 0;
        var writeOffsetTemp = 0;
        var ncaOffsetTmp = ncaOffset;

        do
        {
            // Search the section corresponding to the current NCA offset
            var section = _nczSections.FirstOrDefault(s => s.ContainsOffset(ncaOffsetTmp));
            if (section == null)
                break;

            using var sectionEncryptionHelper = new SectionEncryptionHelper(section);

            // Initialize a sub buffer to what remains to recover in the buffer
            var subBuffer = buffer[writeOffsetTemp..];

            var nbRecoveredBytes = sectionEncryptionHelper.Recover(ncaOffsetTmp, subBuffer);
            if(nbRecoveredBytes <= 0)
                break;

            nbBytesWritten += nbRecoveredBytes;
            ncaOffsetTmp += nbRecoveredBytes;
            writeOffsetTemp += nbRecoveredBytes;

        } while (writeOffsetTemp < buffer.Length);

        return nbBytesWritten;
    }

}

/// <summary>
/// Helper to recover an original NCA section according to section encryption defined in <see cref="NczSection"/>.
/// </summary>
public class SectionEncryptionHelper : IDisposable
{
    private readonly Aes128CtrTransform? _crypto;
    private readonly NczSection _section;

    public SectionEncryptionHelper(NczSection section)
    {
        _section = section;

        switch (section.CryptoType)
        {
            case NcaEncryptionType.Auto:
            case NcaEncryptionType.None:
                _crypto = null;
                break;
            case NcaEncryptionType.AesCtr:
            case NcaEncryptionType.AesCtrEx:
                _crypto = new Aes128CtrTransform(section.CryptoKey, section.CryptoCounter);
                break;
            case NcaEncryptionType.AesXts:
            default:
                throw new NotSupportedException($"Encryption type \"{section.CryptoType}\" not supported.");
        }
    }

    /// <summary>
    /// Recovers the original NCA buffer from the specified NCZ decompressed buffer,
    /// and return the number of bytes recovered (written) in the buffer.
    /// The number of bytes recovered can be less than the buffer size when the remaining section space (between specified NCA offset and section end)
    /// is less than buffer size.
    /// </summary>
    /// <param name="ncaOffset"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="NczFormatException">When the number rev</exception>
    public int Recover(long ncaOffset, Span<byte> buffer)
    {
        if (!_section.ContainsOffset(ncaOffset))
            throw new ArgumentOutOfRangeException(nameof(ncaOffset), "Value is outside of section.");

        // Compute how many bytes can be encrypted from given NCA offset to section end
        var nbAvailableSectionBytes = _section.Offset + _section.Size - ncaOffset;

        // Compute the maximum number of recoverable bytes
        var maxRecoverableBytes = (int)Math.Min(nbAvailableSectionBytes, buffer.Length);

        // If no crypto, we don't touch the buffer
        if (_crypto != null && maxRecoverableBytes > 0)
        {
            var subBuffer = buffer[..maxRecoverableBytes];

            // Adjust crypto settings at NCA offset
            _crypto.UpdateCounter(ncaOffset);

            // Recover the original bytes
            var nbBytesTransformed = _crypto.TransformBlock(subBuffer);
            if (nbBytesTransformed != maxRecoverableBytes)
                throw new NczFormatException("The number of recovered bytes doesn't match the number of bytes to recover.");
        }

        // Prepare the buffer to what can be recovered
        return maxRecoverableBytes;
    }

    public void Dispose()
    {
        // TODO: dispose _crypto when LibHac will implement Aes128CtrTransform as IDisposable
    }
}