namespace LibHac.NSZ.Utils;

public class BufferHelper
{
    private readonly byte[] _buffer;
    private int _position = 0;
    private int _nbBytesWritten = 0;
    private readonly int _maxWritableBytes;

    /// <summary>
    /// Get the index were next write will start
    /// </summary>
    public int Position => _position;

    /// <summary>
    /// True when the buffer is fully written
    /// </summary>
    public bool CanWrite => _nbBytesWritten < _maxWritableBytes;

    /// <summary>
    /// Get the total number of bytes written in the buffer
    /// </summary>
    public int NbBytesWritten => _nbBytesWritten;

    /// <summary>
    /// Get the maximum number of byte which can be really written in the buffer
    /// </summary>
    public int MaxWritableBytes => _maxWritableBytes;

    public BufferHelper(byte[] buffer, int? initialPosition = null, int? maxWritableBytes = null)
    {
        _buffer = buffer;

        if (initialPosition != null)
            _position = Math.Min(Math.Max(0, initialPosition.Value), buffer.Length);
        else
            _position = 0;

        var maxWritableBytesInBuffer = buffer.Length - _position;
        if (maxWritableBytes != null)
            _maxWritableBytes = Math.Min(Math.Max(0, maxWritableBytes.Value), maxWritableBytesInBuffer);
        else
            _maxWritableBytes = maxWritableBytesInBuffer;
    }

    /// <summary>
    /// Write into the buffer at the current position
    /// </summary>
    /// <param name="bytesToWrite"></param>
    /// <returns>Returns the number of bytes written</returns>
    public int Write(Span<byte> bytesToWrite)
    {
        if (!CanWrite)
            return 0;

        // Create a sub destination buffer at to index where to start writing
        var destination = _buffer.AsSpan()[_position..];

        // Compute the maximum number of bytes allowed to be written in the destination buffer
        var maxRemainingBytes = _maxWritableBytes - _nbBytesWritten;

        // Compute the number of bytes that can be written
        var nbBytesToWrite = Math.Min(maxRemainingBytes, bytesToWrite.Length);

        // Copy the bytes in the sub buffer
        bytesToWrite.Slice(0, nbBytesToWrite).CopyTo(destination);

        _position += nbBytesToWrite;
        _nbBytesWritten += nbBytesToWrite;

        return nbBytesToWrite;
    }
}