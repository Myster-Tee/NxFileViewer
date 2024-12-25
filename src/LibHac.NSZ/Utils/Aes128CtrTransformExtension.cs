using LibHac.Tools.FsSystem;

namespace LibHac.NSZ.Utils;

public static class Aes128CtrTransformExtension
{
    public static void UpdateCounter(this Aes128CtrTransform crypto, long offset)
    {
        UpdateCounter(crypto.Counter, offset);
    }

    /// <summary>
    /// Code taken from LibHac source code in <see cref="Aes128CtrStorage"/>
    /// TODO: to remove if LibHac expose this logic publicly update the counter with the offset
    /// </summary>
    /// <param name="counter"></param>
    /// <param name="offset"></param>
    private static void UpdateCounter(byte[] counter, long offset)
    {
        ulong off = (ulong)offset >> 4;
        for (uint j = 0; j < 0x7; j++)
        {
            counter[0x10 - j - 1] = (byte)(off & 0xFF);
            off >>= 8;
        }

        // Because the value stored in the counter is offset >> 4, the top 4 bits 
        // of byte 8 need to have their original value preserved
        counter[8] = (byte)(counter[8] & 0xF0 | (int)(off & 0x0F));
    }
}