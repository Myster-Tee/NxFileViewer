using ZstdSharp;

namespace LibHac.NSZ.Utils;

public static class DecompressionStreamExtension
{

    /// <summary>
    /// ZstdSharp <see cref="DecompressionStream.Read(byte[],int,int)"/> method limits the number of read bytes,
    /// which means that the buffer is not always fully filled according to the buffer size.
    /// This methods continues to read the stream until the buffer is fully filled.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static int FillBuffer(this DecompressionStream stream, byte[] buffer, int offset, int count)
    {
        var offsetTemp = offset;
        var countTmp = count;

        do
        {
            var nbRead = stream.Read(buffer, offsetTemp, countTmp);
            if (nbRead <= 0)
                break;

            offsetTemp += nbRead;
            countTmp -= nbRead;
        } while (countTmp > 0);

        return offsetTemp - offset;
    }

}