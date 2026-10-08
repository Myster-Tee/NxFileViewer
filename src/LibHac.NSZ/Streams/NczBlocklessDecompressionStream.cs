using ZstdSharp;

namespace LibHac.NSZ.Streams;

/// <summary>
/// Lazily decompresses solid NCZ data once, retaining the decoded prefix on disk for random reads.
/// </summary>
public class NczBlocklessDecompressionStream : Stream
{
    private readonly Stream _nczStream;
    private readonly DecompressionStream _decoder;
    private readonly byte[] _buffer = new byte[1024 * 1024];
    private FileStream? _cache;
    private long _compressedPosition;
    private long _decodedLength;
    private long _position;
    private bool _disposed;

    public NczBlocklessDecompressionStream(NczHeader nczHeader, Stream nczStream)
    {
        ArgumentNullException.ThrowIfNull(nczHeader);
        if (nczHeader.BlockCompressionHeader != null)
            throw new ArgumentException("Use NczBlockDecompressionStream for block compression.", nameof(nczHeader));
        _nczStream = nczStream ?? throw new ArgumentNullException(nameof(nczStream));
        _compressedPosition = nczHeader.CompressionStartOffset;
        _nczStream.Position = _compressedPosition;
        _decoder = new DecompressionStream(_nczStream, leaveOpen: true);
        Length = nczHeader.NcaSize - nczHeader.OriginalBytes.Length;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        return Read(buffer.AsSpan(offset, count));
    }

    public override int Read(Span<byte> buffer)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var count = (int)Math.Min(buffer.Length, Length - _position);
        if (count == 0)
            return 0;

        // Create only on first read. DeleteOnClose also cleans up after failed reads.
        if (_cache == null) Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Temp", "NCZ"));
        _cache ??= new FileStream(Path.Combine(AppContext.BaseDirectory, "Temp", "NCZ", Path.GetRandomFileName()),
            FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 64 * 1024,
            FileOptions.DeleteOnClose | FileOptions.RandomAccess);
        var end = _position + count;
        while (_decodedLength < end)
        {
            // Other NCZ readers may share the underlying stream.
            _nczStream.Position = _compressedPosition;
            var read = _decoder.Read(_buffer.AsSpan(0, (int)Math.Min(_buffer.Length, end - _decodedLength)));
            _compressedPosition = _nczStream.Position;
            if (read == 0)
                throw new NczFormatException("Solid NCZ data ends before its declared decompressed size.");
            _cache.Position = _decodedLength;
            _cache.Write(_buffer, 0, read);
            _decodedLength += read;
        }

        _cache.Position = _position;
        _cache.ReadExactly(buffer[..count]);
        _position = end;
        return count;
    }

    public override long Position
    {
        get => _position;
        set
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (value < 0 || value > Length)
                throw new ArgumentOutOfRangeException(nameof(value));
            _position = value;
        }
    }

    public override long Length { get; }
    public override bool CanRead => !_disposed;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override void Flush() { }
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;
            try { _decoder.Dispose(); }
            finally { _cache?.Dispose(); }
        }
        base.Dispose(disposing);
    }
}
