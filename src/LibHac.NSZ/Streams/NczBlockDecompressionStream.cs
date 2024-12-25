using System.Diagnostics.CodeAnalysis;
using LibHac.NSZ.Utils;
using ZstdSharp;

namespace LibHac.NSZ.Streams;

/// <summary>
/// Stream providing an abstraction layer for the decompression of NCZ compressed blocks.
/// The stream <see cref="Position"/> is relative to the <see cref="NczHeader.CompressionStartOffset"/>.
/// </summary>
public class NczBlockDecompressionStream : Stream
{

    private readonly BlockInfo[] _blocks;
    private readonly NczBlockCompressionHeader _nczBlockCompressionHeader;
    private readonly Stream _nczStream;
    private long _position;
    private readonly BlocksCacheManager _blocksCacheManager = new();

    public NczBlockDecompressionStream(NczHeader nczHeader, Stream nczStream)
    {
        if (nczHeader == null)
            throw new ArgumentNullException(nameof(nczHeader));

        _nczBlockCompressionHeader = nczHeader.BlockCompressionHeader ?? throw new ArgumentException($"Given {nameof(NczHeader)} specifies blockless compression and can't be decompressed using {GetType().Name}, use {nameof(NczBlocklessDecompressionStream)} instead.", nameof(nczHeader));
        _nczStream = nczStream ?? throw new ArgumentNullException(nameof(nczStream));

        _blocks = BlocksBuilder.Build(_nczBlockCompressionHeader, nczHeader.CompressionStartOffset).ToArray();
    }

    public override void Flush()
    {
    }

    /// <summary>
    /// Read the decompressed stream
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public override int Read(byte[] buffer, int offset, int count)
    {
        var bh = new BufferHelper(buffer, offset, count);
        do
        {
            if (!TryReadBlockFromPosition(_position, out var blockBuffer))
                break;

            if (blockBuffer == Span<byte>.Empty)
                break;

            _position += bh.Write(blockBuffer);
        }
        while (bh.CanWrite);

        return bh.NbBytesWritten;
    }

    /// <summary>
    /// Read the block at the given position, decompress it if compressed
    /// and return the buffer from the given position to the end of the block
    /// </summary>
    /// <param name="position"></param>
    /// <param name="blockBuffer"></param>
    /// <returns></returns>
    private bool TryReadBlockFromPosition(long position, out Span<byte> blockBuffer)
    {
        if (!TryReadAllBlock(position, out var fullBlockBuffer, out var blockInfo))
        {
            blockBuffer = Span<byte>.Empty;
            return false;
        }

        blockBuffer = fullBlockBuffer.AsSpan((int)(position - blockInfo.DecompressedOffsetStart));
        return true;
    }

    /// <summary>
    /// Read the full block at the given position and decompress it if compressed
    /// </summary>
    /// <param name="position"></param>
    /// <param name="blockBuffer">The bl</param>
    /// <param name="blockInfo"></param>
    /// <returns></returns>
    /// <exception cref="NczFormatException"></exception>
    private bool TryReadAllBlock(long position, [NotNullWhen(true)] out byte[]? blockBuffer, [NotNullWhen(true)] out BlockInfo? blockInfo)
    {
        if (_blocksCacheManager.TryGetBlock(position, out blockInfo, out blockBuffer))
            return true;

        // Search for the block corresponding to the current position
        blockInfo = _blocks.FirstOrDefault(cb => cb.ContainsDecompressedOffset(position));
        if (blockInfo == null)
        {
            blockBuffer = null;
            return false;
        }

        // Set the stream position to beginning of the block
        _nczStream.Position = blockInfo.CompressedOffsetStart;

        blockBuffer = new byte[blockInfo.DecompressedBlockSize];

        int nbRead;
        if (blockInfo.IsCompressed)
        {
            using var decompressionStream = new DecompressionStream(_nczStream);
            nbRead = decompressionStream.FillBuffer(blockBuffer, 0, blockBuffer.Length);
        }
        else
        {
            nbRead = _nczStream.Read(blockBuffer);
        }

        if (nbRead != blockInfo.DecompressedBlockSize)
            throw new NczFormatException($"Decompressed block size {nbRead} doesn't match block size {blockInfo.DecompressedBlockSize}.");

        _blocksCacheManager.AddBlock(blockInfo, blockBuffer);

        return true;
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

    /// <summary>
    /// The decompressed stream length
    /// </summary>
    public override long Length => _nczBlockCompressionHeader.DecompressedSize;

    /// <summary>
    /// Get or set the decompressed stream position (relative to <see cref="NczHeader.CompressionStartOffset"/>)
    /// </summary>
    public override long Position
    {
        get => _position;
        set => _position = value;
    }

    /// <summary>
    /// Get or set the maximum number of blocks to cache (<see cref="BlocksCacheManager.MaxSize"/>)
    /// </summary>
    public int MaxCacheSize
    {
        get => _blocksCacheManager.MaxSize;
        set => _blocksCacheManager.MaxSize = value;
    }
}


public static class BlocksBuilder
{

    /// <summary>
    /// Build a list of <see cref="BlockInfo"/> from the <see cref="NczBlockCompressionHeader"/>.
    /// </summary>
    /// <param name="nczBlockCompressionHeader"></param>
    /// <param name="compressionStartOffset"></param>
    /// <returns></returns>
    /// <exception cref="NczFormatException"></exception>
    public static IEnumerable<BlockInfo> Build(NczBlockCompressionHeader nczBlockCompressionHeader, long compressionStartOffset)
    {
        var blockSizeExponent = nczBlockCompressionHeader.BlockSizeExponent;
        if (blockSizeExponent < 14 || blockSizeExponent > 32)
            throw new NczFormatException($"Block size exponent must be between 14 and 32, found {blockSizeExponent}.");
        var decompressedBlockSize = 1L << blockSizeExponent;

        if (decompressedBlockSize < 1)
            throw new NczFormatException($"Decompressed block size can't be less than 1, found {decompressedBlockSize}.");

        var compressedOffset = compressionStartOffset;
        var decompressedOffset = 0L;

        // The last decompressed index
        var lastDecompressedOffset = nczBlockCompressionHeader.DecompressedSize - 1;

        for (var index = 0; index < nczBlockCompressionHeader.CompressedBlockSizes.Length; index++)
        {
            var compressedBlockSize = nczBlockCompressionHeader.CompressedBlockSizes[index];
            if (compressedBlockSize <= 0)
                throw new NczFormatException($"Compressed block size can't be less or equal to 0, found {compressedBlockSize}.");

            var decompressedOffsetEnd = decompressedOffset + decompressedBlockSize - 1;

            if (index == nczBlockCompressionHeader.CompressedBlockSizes.Length - 1)
            {
                // Fix the last block end offset with the decompressed size
                decompressedOffsetEnd = lastDecompressedOffset;
                decompressedBlockSize = lastDecompressedOffset - decompressedOffset + 1;
            }
            else
            {
                if (decompressedOffsetEnd > lastDecompressedOffset)
                    throw new NczFormatException($"Decompressed block end offset {decompressedOffsetEnd} fall outside of decompressed data size {nczBlockCompressionHeader.DecompressedSize}.");
            }

            var blockInfo = new BlockInfo
            {
                BlockIndex = index,
                DecompressedOffsetStart = decompressedOffset,
                DecompressedOffsetEnd = decompressedOffsetEnd,
                DecompressedBlockSize = decompressedBlockSize,
                CompressedOffsetStart = compressedOffset,
                IsCompressed = compressedBlockSize < decompressedBlockSize,
            };

            yield return blockInfo;

            decompressedOffset += decompressedBlockSize;
            compressedOffset += compressedBlockSize;
        }
    }

}

/// <summary>
/// Helper class containing information of a compressed block, starting at <see cref="CompressedOffsetStart"/>
/// </summary>
public class BlockInfo
{
    /// <summary>
    /// Index of the block is the block list
    /// </summary>
    public int BlockIndex { get; init; }

    /// <summary>
    /// The corresponding decompression start offset, relative to the beginning of the decompressed data
    /// </summary>
    public long DecompressedOffsetStart { get; init; }

    /// <summary>
    /// The corresponding decompression end offset, relative to the beginning of the decompressed data
    /// </summary>
    public long DecompressedOffsetEnd { get; init; }

    /// <summary>
    /// The decompressed block size
    /// </summary>
    public long DecompressedBlockSize { get; init; }

    /// <summary>
    /// The offset at which starts this block, relative to the beginning of the compressed data (<see cref="NczHeader.CompressionStartOffset"/>)
    /// </summary>
    public long CompressedOffsetStart { get; init; }

    /// <summary>
    /// True if block is compressed, false otherwise
    /// </summary>
    public bool IsCompressed { get; init; }

    /// <summary>
    /// Determines if the given decompressed offset is in this compressed block
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    public bool ContainsDecompressedOffset(long offset)
    {
        return offset >= DecompressedOffsetStart && offset <= DecompressedOffsetEnd;
    }

    public override string ToString()
    {
        return $"BlockStart={CompressedOffsetStart}, DecompressionRange=[{DecompressedOffsetStart}:{DecompressedOffsetEnd}]";
    }
}

/// <summary>
/// Cache manager for decompressed blocks
/// </summary>
public class BlocksCacheManager
{
    private int _maxSize = 3;
    private readonly Queue<(BlockInfo, byte[])> _cachedBlocks = new();

    /// <summary>
    /// Get the maximum number of blocks to cache
    /// </summary>
    public int MaxSize
    {
        get => _maxSize;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(MaxSize), value, "Value must be greater or equal to 0.");
            if (value < _maxSize)
                DequeueToSize(value);
            _maxSize = value;
        }
    }

    /// <summary>
    /// Try to get the cached block at the given position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="blockInfo"></param>
    /// <param name="blockBuffer"></param>
    /// <returns></returns>
    public bool TryGetBlock(long position, [NotNullWhen(true)] out BlockInfo? blockInfo, [NotNullWhen(true)] out byte[]? blockBuffer)
    {
        foreach (var (blockInfoTmp, blockBufferTmp) in _cachedBlocks)
        {
            if (!blockInfoTmp.ContainsDecompressedOffset(position))
                continue;

            blockInfo = blockInfoTmp;
            blockBuffer = blockBufferTmp;
            return true;
        }
        blockInfo = null;
        blockBuffer = null;
        return false;
    }

    /// <summary>
    /// Add the block to the cache if not already present
    /// </summary>
    /// <param name="blockInfo"></param>
    /// <param name="blockBuffer"></param>
    public bool AddBlock(BlockInfo blockInfo, byte[] blockBuffer)
    {
        if (blockInfo == null)
            throw new ArgumentNullException(nameof(blockInfo));
        if (blockBuffer == null)
            throw new ArgumentNullException(nameof(blockBuffer));

        if (_cachedBlocks.Any(a => ReferenceEquals(a.Item1, blockInfo)))
            // Block already cached
            return false;

        DequeueToSize(_maxSize - 1);

        _cachedBlocks.Enqueue((blockInfo, blockBuffer));
        return true;
    }

    private void DequeueToSize(int size)
    {
        while (_cachedBlocks.Count > size)
            _cachedBlocks.Dequeue();
    }


}