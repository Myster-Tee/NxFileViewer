using System.Diagnostics.CodeAnalysis;
using LibHac.NSZ.Utils;

namespace LibHac.NSZ.Streams;

/// <summary>
/// Combines an NCA header with its associated decompression stream
/// </summary>
public class NczToNcaStream : Stream
{
    private long _actualNcaPosition = 0;
    private bool _disposed = false;


    private readonly Stream _decompressionStream;
    private readonly long _ncaSize;
    private readonly byte[] _headerBytes;

    public NczToNcaStream(byte[] headerBytes, Stream decompressionStream, long ncaSize)
    {
        _headerBytes = headerBytes ?? throw new ArgumentNullException(nameof(headerBytes));
        _decompressionStream = decompressionStream ?? throw new ArgumentNullException(nameof(decompressionStream));
        _ncaSize = ncaSize;
    }

    /// <summary>
    /// The the full NCA length
    /// </summary>
    public override long Length
    {
        get
        {
            EnsureNotDisposed();
            return _ncaSize;
        }
    }

    /// <summary>
    /// The NCA offset
    /// </summary>
    public override long Position
    {
        get => _actualNcaPosition;
        set
        {
            EnsureNotDisposed();

            if (!IsPositionAllowed(value, out var message))
                throw new ArgumentOutOfRangeException(nameof(Position), message);

            _actualNcaPosition = value;
        }
    }

    public override bool CanRead
    {
        get
        {
            if (_disposed)
                return false;
            return true;
        }
    }

    public override bool CanSeek
    {
        get
        {
            if (_disposed)
                return false;
            return true;
        }
    }

    public override bool CanWrite => false;


    public override void Flush()
    {
        EnsureNotDisposed();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        EnsureNotDisposed();

        var bh = new BufferHelper(buffer, offset, count);


        if (_actualNcaPosition < _headerBytes.Length)
        {
            // Current position is in the header
            _actualNcaPosition += bh.Write(_headerBytes.AsSpan((int)_actualNcaPosition));
        }

        if (bh.CanWrite)
        {

            // Buffer not yet filled and position fall in compressed part
            _decompressionStream.Position = _actualNcaPosition - _headerBytes.Length;

            // Decompress bytes
            var decompressedBytes = new byte[bh.MaxWritableBytes - bh.NbBytesWritten];
            var nbBytesRead = _decompressionStream.Read(decompressedBytes);

            // Writes the buffer
            _actualNcaPosition += bh.Write(decompressedBytes.AsSpan(0, nbBytesRead));
        }

        return bh.NbBytesWritten;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        EnsureNotDisposed();

        var newPosition = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => unchecked(_actualNcaPosition + offset),
            SeekOrigin.End => unchecked(Length + offset),
            _ => throw new ArgumentException($"Unsupported {nameof(origin)}={origin}.", nameof(origin))
        };

        if (!IsPositionAllowed(newPosition, out var message))
            throw new IOException($"Seek operation leads to an invalid position: {message}");

        return _actualNcaPosition;
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }


    private bool IsPositionAllowed(long position, [NotNullWhen(false)] out string? message)
    {
        if (position < 0)
        {
            message = "Position can't be less than 0.";
            return false;
        }

        if (position >= Length)
        {
            message = $"Position can't be greater or equal to stream length.";
            return false;
        }

        message = null;
        return true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _decompressionStream.Dispose();
            _disposed = true;
        }
    }

    private void EnsureNotDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException("Stream disposed.");
    }

}
