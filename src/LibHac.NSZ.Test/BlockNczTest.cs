using System.Text;
using LibHac.NSZ.Streams;
using ZstdSharp;

namespace LibHac.NSZ.Test;

public class BlockNczTest
{
    private const int BlockSize = 1 << 14;

    [Theory]
    [InlineData(BlockSize)]
    [InlineData(731)]
    public void MixedBlocksSupportBoundedReadsAndRandomAccess(int lastSize)
    {
        var first = Enumerable.Repeat((byte)7, BlockSize).ToArray();
        var raw = new byte[BlockSize];
        new Random(191).NextBytes(raw);
        var last = Enumerable.Repeat((byte)9, lastSize).ToArray();
        var expected = first.Concat(raw).Concat(last).ToArray();
        using var compressor = new Compressor();
        var compressedFirst = compressor.Wrap(first).ToArray();
        var compressedLast = compressor.Wrap(last).ToArray();
        using var source = CreateNcz(expected.Length, compressedFirst, raw, compressedLast);
        var header = NczHeader.Read(source);
        source.ShortReads = true;
        var payloadStart = header.CompressionStartOffset;
        using var stream = new NczBlockDecompressionStream(header, source);
        var buffer = new byte[BlockSize];
        var reads = source.BytesRead;
        Assert.Equal(0, stream.Read(buffer, 0, 0));
        Assert.Equal(reads, source.BytesRead);
        stream.ReadExactly(buffer);
        Assert.Equal(first, buffer);
        Assert.Equal(payloadStart + compressedFirst.Length, source.Position);
        Assert.True(source.CanRead);
        stream.Position = 0;
        reads = source.BytesRead;
        stream.ReadExactly(buffer);
        Assert.Equal(reads, source.BytesRead);
        stream.Position = BlockSize - 19;
        var crossing = new byte[BlockSize + 57];
        var count = stream.Read(crossing);
        Assert.Equal(expected.AsSpan(BlockSize - 19, count).ToArray(), crossing[..count]);
        stream.Position = 2 * BlockSize;
        var tail = new byte[lastSize];
        stream.ReadExactly(tail);
        Assert.Equal(last, tail);
        Assert.Equal(0, stream.Read(buffer));
        stream.MaxCacheSize = 0;
        stream.Position = BlockSize;
        stream.ReadExactly(buffer);
        Assert.Equal(raw, buffer); // Raw reads must tolerate short source reads.
        stream.Position = 0;
        stream.ReadExactly(buffer);
        Assert.Equal(first, buffer);
    }

    [Fact]
    public void TruncatedRawBlockFailsInsteadOfReturningIncompleteData()
    {
        using var source = CreateNcz(BlockSize, new byte[BlockSize]);
        var header = NczHeader.Read(source);
        source.SetLength(source.Length - 1);
        source.ShortReads = true;
        using var stream = new NczBlockDecompressionStream(header, source);
        Assert.Throws<NczFormatException>(() => stream.Read(new byte[BlockSize]));
    }

    [Fact]
    public void InvalidBlockSizesAndCountsFailEarly()
    {
        var raw = new NczBlockCompressionHeaderRaw { BlockSizeExponent = 14, DecompressedSize = BlockSize };
        Assert.Throws<NczFormatException>(() => BlocksBuilder.Build(new(raw, new[] { BlockSize + 1 }), 0).ToArray());
        Assert.Throws<NczFormatException>(() => BlocksBuilder.Build(new(raw, new[] { 100, 100 }), 0).ToArray());
    }

    private static ShortReadStream CreateNcz(int size, params byte[][] blocks)
    {
        using var memory = new MemoryStream();
        using var writer = new BinaryWriter(memory, Encoding.ASCII, true);
        writer.Write(new byte[NczHeader.INCOMPRESSIBLE_HEADER_SIZE]);
        writer.Write(Encoding.ASCII.GetBytes("NCZSECTN"));
        writer.Write(1L);
        writer.Write((long)NczHeader.INCOMPRESSIBLE_HEADER_SIZE);
        writer.Write((long)size);
        writer.Write(1L);
        writer.Write(0L);
        writer.Write(new byte[32]);
        writer.Write(Encoding.ASCII.GetBytes("NCZBLOCK"));
        writer.Write((byte)2);
        writer.Write((byte)1);
        writer.Write((byte)0);
        writer.Write((byte)14);
        writer.Write(blocks.Length);
        writer.Write((long)size);
        foreach (var block in blocks) writer.Write(block.Length);
        foreach (var block in blocks) writer.Write(block);
        return new ShortReadStream(memory.ToArray());
    }

    private sealed class ShortReadStream(byte[] bytes) : MemoryStream(bytes)
    {
        public bool ShortReads { get; set; }
        public long BytesRead { get; private set; }
        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = base.Read(buffer, offset, ShortReads ? Math.Min(count, 127) : count);
            BytesRead += read;
            return read;
        }
        public override int Read(Span<byte> buffer)
        {
            var read = base.Read(ShortReads ? buffer[..Math.Min(buffer.Length, 127)] : buffer);
            BytesRead += read;
            return read;
        }
    }
}
