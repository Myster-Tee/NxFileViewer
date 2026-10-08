using System.Text;
using LibHac.NSZ.Streams;
using ZstdSharp;

namespace LibHac.NSZ.Test;

public class SolidNczTest
{
    [Fact]
    public void CachedReadCanCrossDecoderFrontierAfterSourceWasMoved()
    {
        var data = new byte[3 * 1024 * 1024];
        new Random(52).NextBytes(data);
        using var source = CreateNcz(data, data.Length);
        using var stream = new NczBlocklessDecompressionStream(NczHeader.Read(source), source);
        var buffer = new byte[8192];
        stream.ReadExactly(buffer);
        stream.Position = 4096;
        source.Position = 0;
        stream.ReadExactly(buffer);
        Assert.Equal(data.AsSpan(4096, buffer.Length).ToArray(), buffer);
        stream.ReadExactly(buffer);
        Assert.Equal(data.AsSpan(12288, buffer.Length).ToArray(), buffer);
    }

    [Fact]
    public void NcaWrapperSeeksAcrossHeaderAndAcceptsEndOfStream()
    {
        using var payload = new MemoryStream(new byte[] { 3, 4, 5 });
        using var stream = new NczToNcaStream(new byte[] { 1, 2 }, payload, 5);
        var buffer = new byte[4];
        Assert.Equal(1, stream.Seek(1, SeekOrigin.Begin));
        Assert.Equal(4, stream.Read(buffer));
        Assert.Equal(new byte[] { 2, 3, 4, 5 }, buffer);
        Assert.Equal(0, stream.Read(buffer));
        Assert.Equal(3, stream.Seek(-2, SeekOrigin.End));
        Assert.Equal(4, stream.ReadByte());
        Assert.Equal(2, stream.Seek(-2, SeekOrigin.Current));
        Assert.Equal(3, stream.ReadByte());
    }

    [Fact]
    public void ForwardAndRandomReadsDecodePrefixOnlyOnce()
    {
        var data = new byte[4 * 1024 * 1024];
        new Random(52).NextBytes(data);
        using var source = CreateNcz(data, data.Length);
        var header = NczHeader.Read(source);
        using (var stream = new NczBlocklessDecompressionStream(header, source))
        {
            var buffer = new byte[8192];
            Assert.Equal(0, stream.Read(buffer, 0, 0));
            Assert.Equal(8192, stream.Read(buffer));
            Assert.Equal(data[..8192], buffer);
            stream.Position = data.Length - buffer.Length;
            Assert.Equal(buffer.Length, stream.Read(buffer));
            Assert.Equal(data[^8192..], buffer);
            var reads = source.BytesRead;
            foreach (var position in new[] { 0, 2000000, 1000000, data.Length - 4096 })
            {
                stream.Position = position;
                var count = Math.Min(buffer.Length, data.Length - position);
                Assert.Equal(count, stream.Read(buffer));
                Assert.Equal(data.AsSpan(position, count).ToArray(), buffer[..count]);
            }
            Assert.Equal(reads, source.BytesRead);
            stream.Position = stream.Length;
            Assert.Equal(0, stream.Read(buffer));
            stream.Position = 100;
            source.Position = 0; // Simulate another reader using the NCZ source.
            Assert.Equal(buffer.Length, stream.Read(buffer));
            Assert.Equal(data.AsSpan(100, buffer.Length).ToArray(), buffer);
        }
        Assert.True(source.CanRead);
    }

    [Fact]
    public void ShortPayloadFailsAndDisposedReaderRejectsReads()
    {
        using var source = CreateNcz(new byte[32], 100);
        var stream = new NczBlocklessDecompressionStream(NczHeader.Read(source), source);
        Assert.Throws<NczFormatException>(() => stream.Read(new byte[100]));
        stream.Dispose();
        Assert.Throws<ObjectDisposedException>(() => stream.Read(new byte[1]));
        Assert.True(source.CanRead);
    }

    private static CountingStream CreateNcz(byte[] data, int declaredSize)
    {
        using var compressor = new Compressor();
        using var memory = new MemoryStream();
        using var writer = new BinaryWriter(memory, Encoding.ASCII, true);
        writer.Write(new byte[NczHeader.INCOMPRESSIBLE_HEADER_SIZE]);
        writer.Write(Encoding.ASCII.GetBytes("NCZSECTN"));
        writer.Write(1L);
        writer.Write((long)NczHeader.INCOMPRESSIBLE_HEADER_SIZE);
        writer.Write((long)declaredSize);
        writer.Write(1L); // Unencrypted section.
        writer.Write(0L);
        writer.Write(new byte[32]);
        writer.Write(compressor.Wrap(data).ToArray());
        return new CountingStream(memory.ToArray());
    }

    private sealed class CountingStream(byte[] bytes) : MemoryStream(bytes)
    {
        public long BytesRead { get; private set; }
        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = base.Read(buffer, offset, count);
            BytesRead += read;
            return read;
        }
        public override int Read(Span<byte> buffer)
        {
            var read = base.Read(buffer);
            BytesRead += read;
            return read;
        }
    }
}
