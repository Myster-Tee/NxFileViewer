using System;
using System.Text;
using Emignatik.NxFileViewer.Crypto;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Crypto
{
    public class XtsAesCryptoTransformTest
    {
        [Fact]
        public void Encrypt_Decrypt()
        {
            var key1 = new byte[] { 0x01, 0xA0, 0x54, 0xFA, 0x64, 0x14, 0x27, 0x64, 0x76, 0x78, 0x12, 0xB8, 0x64, 0x70, 0xF5, 0xAB };
            var key2 = new byte[] { 0x02, 0x54, 0x88, 0x78, 0x11, 0x22, 0x3A, 0x52, 0x7B, 0xF8, 0xCC, 0x4E, 0xFE, 0x01, 0x00, 0x6C };

            var plaintextInputBuffer = BuildPlaintextBuffer("Hello, this is the message to encrypt/decrypt");

            var cipherOutputBuffer = new byte[plaintextInputBuffer.Length];

            var encryptor = new XtsAesCryptoTransform(key1, key2, TransformationMode.ENCRYPT);
            encryptor.TransformBlock(plaintextInputBuffer, 0, plaintextInputBuffer.Length, cipherOutputBuffer, 0, 0);

            var plaintextOutputBuffer = new byte[plaintextInputBuffer.Length];

            var decryptor = new XtsAesCryptoTransform(key1, key2, TransformationMode.DECRYPT);
            decryptor.TransformBlock(cipherOutputBuffer, 0, cipherOutputBuffer.Length, plaintextOutputBuffer, 0, 0);

            Assert.Equal(plaintextInputBuffer, plaintextOutputBuffer);
        }

        private static byte[] BuildPlaintextBuffer(string message)
        {
            var plaintextBuffer = Encoding.UTF8.GetBytes(message);
            var nbByteMissingBytes = 16 - plaintextBuffer.Length % 16;

            if (nbByteMissingBytes == 0)
            {
                return plaintextBuffer;
            }

            var buff = new byte[plaintextBuffer.Length + nbByteMissingBytes];
            Buffer.BlockCopy(plaintextBuffer, 0, buff, 0, plaintextBuffer.Length);
            return buff;
        }
    }
}