using LibHac.NSZ.Utils;

namespace LibHac.NSZ.Test.Utils;

public class BufferHelperTest
{

    [Fact]
    public void WriteNominal()
    {
        var buffer = new byte[5];
        var bh = new BufferHelper(buffer, null, null);
        Assert.True(bh.CanWrite);
        Assert.Equal(0, bh.Position);
        Assert.Equal(5, bh.MaxWritableBytes);
        Assert.Equal(new byte[buffer.Length], buffer);


        Assert.Equal(2, bh.Write(new byte[] { 5, 6 }));
        Assert.True(bh.CanWrite);
        Assert.Equal(new byte[] { 5, 6, 0, 0, 0 }, buffer);
        Assert.Equal(2, bh.Position);

        Assert.Equal(2, bh.Write(new byte[] { 2, 26 }));
        Assert.True(bh.CanWrite);
        Assert.Equal(new byte[] { 5, 6, 2, 26, 0 }, buffer);
        Assert.Equal(4, bh.Position);

        Assert.Equal(1, bh.Write(new byte[] { 8, 55, 59 }));
        Assert.False(bh.CanWrite);
        Assert.Equal(new byte[] { 5, 6, 2, 26, 8 }, buffer);
        Assert.Equal(5, bh.Position);

        Assert.Equal(0, bh.Write(new byte[] { 30, 17 }));
        Assert.False(bh.CanWrite);
        Assert.Equal(new byte[] { 5, 6, 2, 26, 8 }, buffer);
        Assert.Equal(5, bh.Position);
        Assert.Equal(5, bh.NbBytesWritten);
    }

    [Fact]
    public void WriteOneTimeFill()
    {
        var buffer = new byte[5];
        var bh = new BufferHelper(buffer);
        Assert.True(bh.CanWrite);
        Assert.Equal(new byte[buffer.Length], buffer);

        Assert.Equal(5, bh.Write(new byte[] { 5, 6, 20, 30, 54, 254, 51, 137 }));
        Assert.False(bh.CanWrite);
        Assert.Equal(new byte[] { 5, 6, 20, 30, 54 }, buffer);

        Assert.False(bh.CanWrite);

        Assert.Equal(5, bh.Position);
        Assert.Equal(5, bh.NbBytesWritten);
    }

    [Fact]
    public void EmptyBuffer()
    {
        var buffer = Array.Empty<byte>();
        var bh = new BufferHelper(buffer);

        Assert.False(bh.CanWrite);
        Assert.Equal(Array.Empty<byte>(), buffer);


        Assert.Equal(0, bh.Write(new byte[] { 5, 6, 20, 30, 54, 254, 51, 137 }));
        Assert.False(bh.CanWrite);
        Assert.Equal(0, bh.Position);
        Assert.Equal(0, bh.NbBytesWritten);

        Assert.Equal(0, bh.Write(new byte[] { 5, 6, 20, 30, 54, 254, 51, 137 }));
        Assert.False(bh.CanWrite);
        Assert.Equal(0, bh.Position);
        Assert.Equal(0, bh.NbBytesWritten);
    }

    [Fact]
    public void WriteMax()
    {
        var buffer = new byte[5];
        var bh = new BufferHelper(buffer, null, 3);
        Assert.True(bh.CanWrite);
        Assert.Equal(new byte[buffer.Length], buffer);

        Assert.Equal(2, bh.Write(new byte[] { 5, 10 }));
        Assert.True(bh.CanWrite);
        Assert.Equal(new byte[] { 5, 10, 0, 0, 0 }, buffer);
        Assert.Equal(2, bh.Position);

        Assert.Equal(1, bh.Write(new byte[] { 22 }));
        Assert.False(bh.CanWrite);
        Assert.Equal(new byte[] { 5, 10, 22, 0, 0 }, buffer);
        Assert.Equal(3, bh.Position);

        Assert.Equal(0, bh.Write(new byte[] { 51, 77, 94 }));
        Assert.False(bh.CanWrite);
        Assert.Equal(new byte[] { 5, 10, 22, 0, 0 }, buffer);
        Assert.Equal(3, bh.Position);
        Assert.Equal(3, bh.NbBytesWritten);
    }

    [Fact]
    public void WriteOffset()
    {
        var buffer = new byte[5];
        var bh = new BufferHelper(buffer, 2, null);
        Assert.True(bh.CanWrite);
        Assert.Equal(2, bh.Position);
        Assert.Equal(3, bh.MaxWritableBytes);
        Assert.Equal(new byte[buffer.Length], buffer);


        Assert.Equal(2, bh.Write(new byte[] { 5, 10 }));
        Assert.True(bh.CanWrite);
        Assert.Equal(new byte[] { 0, 0, 5, 10, 0 }, buffer);
        Assert.Equal(4, bh.Position);
        Assert.Equal(2, bh.NbBytesWritten);


        Assert.Equal(1, bh.Write(new byte[] { 85, 2 }));
        Assert.False(bh.CanWrite);
        Assert.Equal(new byte[] { 0, 0, 5, 10, 85 }, buffer);
        Assert.Equal(5, bh.Position);
        Assert.Equal(3, bh.NbBytesWritten);
    }

    [Fact]
    public void WriteOffsetAndMax()
    {
        var buffer = new byte[8];

        var bh = new BufferHelper(buffer, initialPosition: 2, maxWritableBytes: 3);
        Assert.True(bh.CanWrite);
        Assert.Equal(2, bh.Position);
        Assert.Equal(3, bh.MaxWritableBytes);
        Assert.Equal(new byte[buffer.Length], buffer);


        Assert.Equal(2, bh.Write(new byte[] { 5, 10 }));
        Assert.True(bh.CanWrite);
        Assert.Equal(new byte[] { 0, 0, 5, 10, 0, 0, 0, 0 }, buffer);
        Assert.Equal(4, bh.Position);
        Assert.Equal(2, bh.NbBytesWritten);


        Assert.Equal(1, bh.Write(new byte[] { 85, 2, 77 }));
        Assert.False(bh.CanWrite);
        Assert.Equal(new byte[] { 0, 0, 5, 10, 85, 0, 0, 0 }, buffer);
        Assert.Equal(5, bh.Position);
        Assert.Equal(3, bh.NbBytesWritten);
    }


    [Fact]
    public void WriteOffsetAndMaxGreater()
    {
        var buffer = new byte[8];

        var bh = new BufferHelper(buffer, buffer.Length, 30000);
        Assert.False(bh.CanWrite);
        Assert.Equal(buffer.Length, bh.Position);
        Assert.Equal(0, bh.MaxWritableBytes);
        Assert.Equal(new byte[buffer.Length], buffer);
        Assert.Equal(0, bh.NbBytesWritten);


        Assert.Equal(0, bh.Write(new byte[] { 5, 10 }));
        Assert.False(bh.CanWrite);
        Assert.Equal(new byte[buffer.Length], buffer);
        Assert.Equal(buffer.Length, bh.Position);
        Assert.Equal(0, bh.NbBytesWritten);
    }

    [Fact]
    public void WriteOffsetAndMaxLess()
    {
        var buffer = new byte[8];

        var bh = new BufferHelper(buffer, -5, -40);
        Assert.False(bh.CanWrite);
        Assert.Equal(0, bh.Position);
        Assert.Equal(0, bh.MaxWritableBytes);
        Assert.Equal(new byte[buffer.Length], buffer);
        Assert.Equal(0, bh.NbBytesWritten);

        Assert.Equal(0, bh.Write(new byte[] { 5, 10 }));
        Assert.False(bh.CanWrite);
        Assert.Equal(new byte[buffer.Length], buffer);
        Assert.Equal(0, bh.Position);
        Assert.Equal(0, bh.NbBytesWritten);
    }

    [Fact]
    public void WriteEmpty()
    {
        var buffer = new byte[5];
        var bh = new BufferHelper(buffer, null, null);
        Assert.True(bh.CanWrite);
        Assert.Equal(new byte[buffer.Length], buffer);

        Assert.Equal(0, bh.Write(Array.Empty<byte>()));
        Assert.True(bh.CanWrite);
        Assert.Equal(new byte[buffer.Length], buffer);
        Assert.Equal(0, bh.Position);
        Assert.Equal(0, bh.NbBytesWritten);
        Assert.Equal(5, bh.MaxWritableBytes);
    }
}