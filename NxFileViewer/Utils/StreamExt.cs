using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Emignatik.NxFileViewer.Utils
{
    public static class StreamExt
    {
        private static readonly Encoding _defaultEncoding = Encoding.UTF8;

        public static long FindNextBytePos(this Stream stream, byte searchedByte)
        {
            while (true)
            {
                var readByte = stream.ReadByte();
                if (readByte < 0) throw new Exception($"End of stream reached before finding byte \"{searchedByte:X2}\".");
                if (readByte == searchedByte) return stream.Position;
            }
        }

        public static byte[] ReadBytes(this Stream stream, int nbBytes, bool throwOnMissing = true)
        {
            var bytes = new byte[nbBytes];
            var read = stream.Read(bytes, 0, nbBytes);
            if (read != nbBytes && throwOnMissing) throw new Exception($"End of stream reached before the expected \"0x{nbBytes:X2}\" byte(s).");

            return bytes;
        }

        public static string ReadNullTerminatedString(this Stream stream)
        {
            var strPosStart = stream.Position;
            var strPosEnd = stream.FindNextBytePos(0) - 1;

            var buffer = new byte[strPosEnd - strPosStart];

            stream.Position = strPosStart;
            stream.Read(buffer, 0, buffer.Length);
            stream.Position++; //To make stream position pointing after the null terminating byte

            return _defaultEncoding.GetString(buffer);
        }

        public static string ReadNullTerminatedString(this Stream stream, int nbByte, Encoding encoding, bool throwOnMissing = true)
        {
            var bytes = stream.ReadBytes(nbByte, throwOnMissing);
            return bytes.AsNullTerminatedString(encoding);
        }

        public static string ReadNullTerminatedString(this Stream stream, int nbByte, bool throwOnMissing = true)
        {
            return stream.ReadNullTerminatedString(nbByte, _defaultEncoding, throwOnMissing);
        }

        public static byte[] ReadBytes(this Stream stream, int nbBytes, long position, bool throwOnEndOfStream = true)
        {
            stream.Position = position;
            return stream.ReadBytes(nbBytes, throwOnEndOfStream);
        }

        public static T ReadStruct<T>(this Stream stream, long position) where T : struct
        {
            stream.Position = position;
            return stream.ReadStruct<T>();
        }

        public static T ReadStruct<T>(this Stream stream) where T : struct
        {
            var structType = typeof(T);
            var buffLength = Marshal.SizeOf(structType);

            var buff = new byte[buffLength];
            var read = stream.Read(buff, 0, buffLength);
            if (read != buffLength)
                throw new Exception($"Can't fill structure \"{nameof(T)}\", stream too short.");

            return buff.ToStruct<T>();
        }
    }
}