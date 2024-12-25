using System.Diagnostics.CodeAnalysis;
using LibHac.Common.Keys;
using LibHac.Crypto;
using LibHac.NSZ.Streams;
using LibHac.NSZ.Utils;
using LibHac.Tools.FsSystem;
using LibHac.Tools.FsSystem.NcaUtils;

namespace LibHac.NSZ;

public class Ncz : Nca
{
    private readonly KeySet _keySet;
    private readonly Stream _nczStream;

    public Ncz(KeySet keySet, Stream nczStream, NczReadMode readMode) : this(keySet, nczStream, readMode, Initialize(nczStream, readMode, keySet, out var nczToNcaStream), nczToNcaStream)
    {

    }

    private Ncz(KeySet keySet, Stream nczStream, NczReadMode readMode, NczHeader nczHeader, NczToNcaStream originalNczToNcaStream) : base(keySet, originalNczToNcaStream.AsStorage())
    {
        _keySet = keySet;
        _nczStream = nczStream;
        NczHeader = nczHeader;
        NcaStream = originalNczToNcaStream;
        ReadMode = readMode;
    }

    private static NczHeader Initialize(Stream nczStream, NczReadMode readMode, KeySet keySet, out NczToNcaStream nczToNcaStream)
    {
        if (nczStream == null)
            throw new ArgumentNullException(nameof(nczStream));

        // Read NCZ header
        var nczHeader = NczHeader.Read(nczStream);

        switch (readMode)
        {
            case NczReadMode.Original:
                nczToNcaStream = CreateOriginalNczToNcaStream(nczHeader, nczStream);
                break;
            case NczReadMode.Fast:
                nczToNcaStream = CreateFastNczToNcaStream(nczHeader, nczStream, keySet);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(readMode), readMode, null);
        }

        return nczHeader;
    }

    private static NczToNcaStream CreateOriginalNczToNcaStream(NczHeader nczHeader, Stream nczStream)
    {

        // Initialize decompression stream according to the compression mode (block or blockless)
        Stream decompressionStream = nczHeader.IsUsingBlockCompression
            ? new NczBlockDecompressionStream(nczHeader, nczStream)
            : new NczBlocklessDecompressionStream(nczHeader, nczStream);

        // Wrap decompression stream in encryption stream
        var ncaEncryptionStream = new NcaEncryptionStream(nczHeader, decompressionStream);

        // Build the original NCA stream
        return new NczToNcaStream(nczHeader.OriginalBytes, ncaEncryptionStream, nczHeader.NcaSize);
    }

    private static NczToNcaStream CreateFastNczToNcaStream(NczHeader nczHeader, Stream nczStream, KeySet keySet)
    {
        using var originalNczToNcaStream = CreateOriginalNczToNcaStream(nczHeader, nczStream);
        var plaintextPatchedNczHeader = ReadAndPatchHeaderAsNoEncryption(originalNczToNcaStream, keySet);

        Stream decompressionStream = nczHeader.IsUsingBlockCompression ?
            new NczBlockDecompressionStream(nczHeader, nczStream) :
            new NczBlocklessDecompressionStream(nczHeader, nczStream);

        return new NczToNcaStream(plaintextPatchedNczHeader, decompressionStream, nczHeader.NcaSize);
    }

    /// <summary>
    /// Get the parsed NCZ header
    /// </summary>
    public NczHeader NczHeader { get; }

    /// <summary>
    /// Get the NCA stream
    /// </summary>
    public NczToNcaStream NcaStream { get; }

    /// <summary>
    /// Get the NCZ read method
    /// </summary>
    public NczReadMode ReadMode { get; }

    /// <summary>
    /// Allows to switch the read mode of the NCZ
    /// </summary>
    /// <param name="readMode"></param>
    /// <returns></returns>
    public Ncz SwitchReadMode(NczReadMode readMode)
    {
        return readMode == ReadMode ? this : new Ncz(_keySet, _nczStream, readMode);
    }

    /// <summary>
    /// Decrypt the given NCZ header and patch all the contained FS Header sections, so they appear unencrypted.
    /// </summary>
    /// <param name="originalNczToNcaStream"></param>
    /// <param name="keySet"></param>
    /// <returns></returns>
    private static byte[] ReadAndPatchHeaderAsNoEncryption(NczToNcaStream originalNczToNcaStream, KeySet keySet)
    {
        var nca = new Nca(keySet, originalNczToNcaStream.AsStorage());
        using var decryptedStorage = nca.OpenDecryptedNca();
        var plaintextNczHeader = new byte[NczHeader.INCOMPRESSIBLE_HEADER_SIZE];
        decryptedStorage.Read(0, plaintextNczHeader).ThrowIfFailure();

        Memory<byte> decryptedHeaderMem = plaintextNczHeader;

        for (var sectionIndex = 0; sectionIndex <= 3; sectionIndex++)
        {
            const int FS_HEADER_SIZE = 0x200;

            if (!nca.SectionExists(sectionIndex))
                continue;

            // Patch header to mark the Section without encryption 
            plaintextNczHeader[0x400 + sectionIndex * FS_HEADER_SIZE + 0x4] = (byte)FsSystem.NcaFsHeader.EncryptionType.None;

            // Recompute Hash of NcaFsHeader
            Span<byte> fsHeaderHash0 = new byte[Sha256.DigestSize];
            Sha256.GenerateSha256Hash(decryptedHeaderMem.Slice(0x400 + sectionIndex * FS_HEADER_SIZE, FS_HEADER_SIZE).Span, fsHeaderHash0);

            // Update hash in decrypted header (this breaks original NCA signature from Nintendo)
            Buffer.BlockCopy(fsHeaderHash0.ToArray(), 0, plaintextNczHeader, 0x280 + sectionIndex * Sha256.DigestSize, Sha256.DigestSize);
        }

        return plaintextNczHeader;
    }

}

public class NczHeader
{
    /// <summary>
    /// The untouched header size (same as original NCA)
    /// </summary>
    public const int INCOMPRESSIBLE_HEADER_SIZE = 0x4000;

    public NczSection[] Sections { get; private set; } = null!;

    /// <summary>
    /// Get a boolean indicating if NCZ is using block compression
    /// </summary>
    [MemberNotNullWhen(true, nameof(BlockCompressionHeader))]
    public bool IsUsingBlockCompression => BlockCompressionHeader != null;

    public NczBlockCompressionHeader? BlockCompressionHeader { get; private set; }

    public long CompressionStartOffset { get; private set; }

    public byte[] OriginalBytes { get; } = new byte[INCOMPRESSIBLE_HEADER_SIZE];

    public long NcaSize { get; private set; }

    private NczHeader()
    {
    }

    public static NczHeader Read(Stream nczStream)
    {
        nczStream = nczStream ?? throw new ArgumentNullException(nameof(nczStream));

        var nczHeader = new NczHeader();
        nczStream.Position = 0;

        var nczBinaryReader = new BinaryReader(nczStream);

        if (nczBinaryReader.Read(nczHeader.OriginalBytes) != nczHeader.OriginalBytes.Length)
            throw new NczFormatException("NCZ header size invalid.");

        var magic = nczBinaryReader.ReadAsciiString(8);
        if (magic != "NCZSECTN")
            throw new NczFormatException("NCZ section magic not found.");

        var sectionCount = nczBinaryReader.ReadInt64();

        nczHeader.Sections = new NczSection[sectionCount];

        var ncaSize = (long)INCOMPRESSIBLE_HEADER_SIZE;
        for (var i = 0; i < sectionCount; i++)
        {
            var section = nczBinaryReader.ReadNczSectionRaw();
            nczHeader.Sections[i] = new NczSection(section);
            ncaSize += section.Size;
        }
        nczHeader.NcaSize = ncaSize;

        var blockMagic = nczBinaryReader.ReadAsciiString(8);
        const string NCZ_BLOCK_MAGIC = "NCZBLOCK";
        if (blockMagic == NCZ_BLOCK_MAGIC)
        {
            nczHeader.BlockCompressionHeader = nczBinaryReader.ReadNczCompressionBlockInfo();
            nczHeader.CompressionStartOffset = nczStream.Position;
        }
        else
        {
            nczHeader.CompressionStartOffset = nczBinaryReader.BaseStream.Position - NCZ_BLOCK_MAGIC.Length;
        }

        return nczHeader;
    }
}


public enum NczReadMode
{
    /// <summary>
    /// Combines the original NCA header with the re-encryption of the decompressed part.
    /// Prefer this mode if you need to check original NCA integrity. 
    /// </summary>
    Original,

    /// <summary>
    /// Combines the decrypted header with the decompressed part.
    /// Prefer this stream if you want to browse the FS content faster, but beware: NCA integrity checks (signature and hash) will very probably fail.
    /// </summary>
    Fast
}