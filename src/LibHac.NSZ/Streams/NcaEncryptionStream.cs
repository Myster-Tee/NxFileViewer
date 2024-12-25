using LibHac.NSZ.Utils;

namespace LibHac.NSZ.Streams;


/// <summary>
/// Stream in charge of restoring the original NCA encryption from the decompression stream.
/// </summary>
public class NcaEncryptionStream : Stream
{
    private readonly Stream _nczDecompressionStream;
    private readonly NczSectionsEncryptionHelper _nczSectionsEncryptionHelper;


    /// <summary>
    /// </summary>
    /// <param name="nczHeader"></param>
    /// <param name="nczDecompressionStream">The stream in charge of decompressing the NCZ compressed part (which excludes untouched 0x4000 NCZ header)</param>
    /// <exception cref="ArgumentNullException"></exception>
    public NcaEncryptionStream(NczHeader nczHeader, Stream nczDecompressionStream)
    {
        nczHeader = nczHeader ?? throw new ArgumentNullException(nameof(nczHeader));
        _nczDecompressionStream = nczDecompressionStream ?? throw new ArgumentNullException(nameof(nczDecompressionStream));
        _nczSectionsEncryptionHelper = new NczSectionsEncryptionHelper(nczHeader.Sections);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _nczDecompressionStream?.Dispose();
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        // Here, it is mandatory to compute the NCA offset before reading the decompression stream
        // in order to ensure the correct offset to use to get the corresponding section encryption settings
        var ncaOffset = NczHeader.INCOMPRESSIBLE_HEADER_SIZE + _nczDecompressionStream.Position;

        // Decompress in the sub buffer
        var nbDecompressedBytes = _nczDecompressionStream.Read(buffer, offset, count);

        // Recover the sub buffer to the original NCA buffer
        var nbBytesRecovered = _nczSectionsEncryptionHelper.Recover(ncaOffset, buffer.AsSpan(0, nbDecompressedBytes));

        if (nbDecompressedBytes != nbBytesRecovered)
            throw new NczFormatException("The number of decompressed bytes doesn't match the number of recovered bytes.");

        return nbBytesRecovered;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => _nczDecompressionStream.Length;

    public override long Position
    {
        get => _nczDecompressionStream.Position;
        set => _nczDecompressionStream.Position = value;
    }
}