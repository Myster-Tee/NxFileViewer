using System.Runtime.InteropServices;
using System.Text;

namespace LibHac.NSZ.Utils;

public static class NczBinaryReaderExtension
{

    public static string ReadAsciiString(this BinaryReader binaryReader, int length)
    {
        var readBytes = binaryReader.ReadBytes(length);
        return Encoding.ASCII.GetString(readBytes);
    }

    public static NczBlockCompressionHeader ReadNczCompressionBlockInfo(this BinaryReader reader)
    {
        try
        {
            var nczCompressionBlock = reader.ReadStruct<NczBlockCompressionHeaderRaw>();

            var compressedBlockSizes = new int[nczCompressionBlock.NumberOfBlocks];
            var bytes = MemoryMarshal.Cast<int, byte>(compressedBlockSizes);
            var read = reader.Read(bytes);
            if (read != bytes.Length)
                throw new EndOfStreamException();

            return new NczBlockCompressionHeader(nczCompressionBlock, compressedBlockSizes);
        }
        catch (EndOfStreamException)
        {
            throw new NczFormatException("NCZ compression block incomplete.");
        }
        catch (Exception ex)
        {
            throw new NczFormatException($"NCZ compression block read failed: {ex.Message}", ex);
        }
    }

    public static NczSectionRaw ReadNczSectionRaw(this BinaryReader reader)
    {
        try
        {
            var nczSection = reader.ReadStruct<NczSectionRaw>();
            return nczSection;
        }
        catch (EndOfStreamException)
        {
            throw new NczFormatException("NCZ section incomplete.");
        }
        catch (Exception ex)
        {
            throw new NczFormatException($"NCZ section read failed: {ex.Message}", ex);
        }
    }


    private static T ReadStruct<T>(this BinaryReader reader) where T : struct
    {
        var nbByteToRead = Marshal.SizeOf(typeof(T));
        var buffer = reader.ReadBytes(nbByteToRead);
        if (buffer.Length != nbByteToRead)
            throw new EndOfStreamException();

        var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        var t = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T))!;
        handle.Free();

        return t;
    }
}