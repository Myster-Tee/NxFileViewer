using System.Diagnostics;
using LibHac.NSZ.Utils;
using ZstdSharp;

namespace LibHac.NSZ.Streams;

/// <summary>
/// Stream to use for the raw decompression of NCZ compressed part, compressed without block compression.
/// This stream implements random access read but with very poor performances.
/// </summary>
public class NczBlocklessDecompressionStream : Stream
{
    private readonly Stream _nczStream;
    private readonly long _compressionStartOffset;
    private DecompressionStream _decompressionStream;

    /// <summary>
    /// The requested position (<see cref="Position"/>) from which to read in the decompressed stream.
    /// </summary>
    private long _requestedPosition;

    /// <summary>
    /// The real position of the decompressed bytes with <see cref="_decompressionStream"/>.
    /// </summary>
    private long _realPosition;

    private long _lastReadNczStreamPosition;

    public NczBlocklessDecompressionStream(NczHeader nczHeader, Stream nczStream)
    {
        if (nczHeader == null)
            throw new ArgumentNullException(nameof(nczHeader));

        if (nczHeader.BlockCompressionHeader != null)
            throw new ArgumentException($"Given {nameof(NczHeader)} specifies block compression and can't be decompressed using {GetType().Name}, use {nameof(NczBlockDecompressionStream)} instead.", nameof(nczHeader));

        _nczStream = nczStream ?? throw new ArgumentNullException(nameof(nczStream));
        _compressionStartOffset = nczHeader.CompressionStartOffset;

        _decompressionStream = new DecompressionStream(_nczStream);
        _lastReadNczStreamPosition = _compressionStartOffset;
        _realPosition = 0;
        _requestedPosition = 0;

        Length = nczHeader.NcaSize - nczHeader.OriginalBytes.Length;
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (!DecompressToExpectedPosition())
            return 0;

        var nbRead = _decompressionStream.FillBuffer(buffer, offset, count);

        _realPosition += nbRead;
        _requestedPosition = _realPosition;
        _lastReadNczStreamPosition = _nczStream.Position;

        return nbRead;
    }

    private bool DecompressToExpectedPosition()
    {
        // Test if expected position has been changed to somewhere before what we already decompressed
        if (_requestedPosition < _realPosition)
        {
            // We need to restart decompression from the beginning
            _decompressionStream.Dispose();
            _decompressionStream = new DecompressionStream(_nczStream);
            _nczStream.Position = _compressionStartOffset;
            _realPosition = 0;
        }
        else
        {
            // Ensure NCZ stream position, in case it was changed from somewhere else
            _nczStream.Position = _lastReadNczStreamPosition;
        }

        var buff = new byte[1000000];
        do
        {
            var remainingBytesToRead = (int)Math.Min(_requestedPosition - _realPosition, buff.Length);
            if (remainingBytesToRead <= 0)
                break;

            // We waste CPU energy decompressing lost bytes, how sad...
            var nbBytesRead = _decompressionStream.Read(buff.AsSpan(0, remainingBytesToRead));
            if (nbBytesRead <= 0)
                break;

            _realPosition += nbBytesRead;
        }
        while (true);

        var expectedPositionReached = _realPosition == _requestedPosition;
        Debug.Assert(expectedPositionReached, $"For any consistent NCZ, {nameof(_requestedPosition)} should always be reachable.");
        return expectedPositionReached;
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

    public override long Length { get; }

    /// <summary>
    /// Get or set the position of the decompressed stream.
    /// Position 0 corresponds to <see cref="NczHeader.CompressionStartOffset"/> in NCZ stream.
    /// </summary>
    public override long Position
    {
        get => _requestedPosition;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException($"{nameof(Position)} can't be less than 0.");
            if (value >= Length)
                throw new ArgumentOutOfRangeException($"{nameof(Position)} can't be greater or equal to stream length.");

            _requestedPosition = value;
        }
    }
}