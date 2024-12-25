using LibHac.Tools.FsSystem.NcaUtils;
using System.Runtime.InteropServices;

namespace LibHac.NSZ;

/// <summary>
/// The raw structure of an NCZ section header
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct NczSectionRaw
{
    public long Offset;

    public long Size;

    public long CryptoType;

    public long Padding;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] CryptoKey;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] CryptoCounter;
}

/// <summary>
/// A section specify the way to decrypt a portion of the decompressed stream, starting at <see cref="Offset"/>
/// </summary>
public class NczSection
{
    private readonly NczSectionRaw _sectionRaw;

    public NczSection(NczSectionRaw sectionRaw)
    {
        _sectionRaw = sectionRaw;
    }

    /// <summary>
    /// The offset relative to the beginning of the NCA (including NCA header)
    /// </summary>
    public long Offset => _sectionRaw.Offset;

    /// <summary>
    /// The size covered by this section
    /// </summary>
    public long Size => _sectionRaw.Size;

    /// <summary>
    /// The encryption type to use to recover the original NCA file
    /// </summary>
    public NcaEncryptionType CryptoType => (NcaEncryptionType)_sectionRaw.CryptoType;

    /// <summary>
    /// The padding
    /// </summary>
    public long Padding => _sectionRaw.Padding;

    /// <summary>
    /// The encryption key
    /// </summary>
    public byte[] CryptoKey => _sectionRaw.CryptoKey;

    /// <summary>
    /// The encryption counter when encryption is <see cref="NcaEncryptionType.AesCtr"/> or <see cref="NcaEncryptionType.AesCtrEx"/> 
    /// </summary>
    public byte[] CryptoCounter => _sectionRaw.CryptoCounter;

    /// <summary>
    /// Returns true if specified NCA position fall in this section
    /// </summary>
    /// <param name="ncaOffset"></param>
    /// <returns></returns>
    public bool ContainsOffset(long ncaOffset)
    {
        return ncaOffset >= Offset && ncaOffset < (Offset + Size);
    }

    public override string ToString()
    {
        return $"Section [{Offset}-{Offset + Size - 1}] (Crypto={CryptoType})";
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct NczBlockCompressionHeaderRaw
{

    public byte Version;

    public byte Type;

    public byte Unused;

    public byte BlockSizeExponent;

    public int NumberOfBlocks;

    public long DecompressedSize;

}

/// <summary>
/// Header containing information about NCZ block compression 
/// </summary>
public class NczBlockCompressionHeader
{
    private readonly NczBlockCompressionHeaderRaw _nczBlockCompressionHeaderRaw;

    public NczBlockCompressionHeader(NczBlockCompressionHeaderRaw nczBlockCompressionHeaderRaw, int[] compressedBlockSizes)
    {
        _nczBlockCompressionHeaderRaw = nczBlockCompressionHeaderRaw;
        CompressedBlockSizes = compressedBlockSizes;
    }

    /// <summary>
    /// The NCZ version
    /// </summary>
    public byte Version => _nczBlockCompressionHeaderRaw.Version;

    /// <summary>
    /// 
    /// </summary>
    public byte Type => _nczBlockCompressionHeaderRaw.Type;

    /// <summary>
    /// The decompressed block size in exponent of 2
    /// </summary>
    public byte BlockSizeExponent => _nczBlockCompressionHeaderRaw.BlockSizeExponent;

    /// <summary>
    /// The total decompressed size (does not include NCA header)
    /// </summary>
    public long DecompressedSize => _nczBlockCompressionHeaderRaw.DecompressedSize;

    /// <summary>
    /// The array of compressed blocks sizes.
    /// </summary>
    public int[] CompressedBlockSizes { get; }
}