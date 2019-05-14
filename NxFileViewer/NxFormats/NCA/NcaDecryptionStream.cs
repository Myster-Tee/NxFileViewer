using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Emignatik.NxFileViewer.Crypto;
using Emignatik.NxFileViewer.KeysParsing;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.NxFormats.NCA
{
    public class NcaDecryptionStream : Stream
    {
        public const int HEADER_SIZE = 0xC00;
        private const int SECTOR_SIZE = 0x200;

        private readonly Stream _baseStream;
        private bool _hotfixSectionHeaderSectorIndex;
        private readonly IXtsAesCryptoTransform _headerDecryptor;
        private readonly byte[] _lastDecryptedHeaderSector = new byte[SECTOR_SIZE];
        private long _lastDecryptedHeaderSectorIndex = -1;

        public NcaDecryptionStream(Stream baseStream, KeySet keySet)
        {
            _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
            if (keySet == null)
                throw new ArgumentNullException(nameof(keySet));

            var headerDecryptionKey = GetNcaHeaderDecryptionKey(keySet);

            _headerDecryptor = XtsAes.CreateDecryptor(headerDecryptionKey);

            CheckNcaVersion();
        }

        private void CheckNcaVersion()
        {
            lock (_baseStream)
            {
                var position = _baseStream.Position;
                try
                {
                    var decryptedBytes = DecryptHeaderSector(1);

                    var magic = Encoding.ASCII.GetString(decryptedBytes, 0, 4);
                    var isNca2 = magic == "NCA2";
                    var isNca3 = !isNca2 && magic == "NCA3";

                    if (!isNca2 && !isNca3) throw new Exception($"File is not a valid NCA (Magic was \"{magic}\")!");

                    // NOTE: Extract from [https://switchbrew.org/wiki/NCA_Format#Encryption]:
                    // For pre-1.0.0 "NCA2" NCAs, the first 0x400 byte are encrypted the same way as in NCA3.
                    // However, each section header is individually encrypted as though it were sector 0, instead of the appropriate sector as in NCA3.
                    _hotfixSectionHeaderSectorIndex = isNca2;
                }
                finally
                {
                    _baseStream.Position = position;
                }
            }
        }

        private static byte[] GetNcaHeaderDecryptionKey(KeySet keySet)
        {
            const string HEADER_KEY = "header_key";
            if (!keySet.TryGetKey(HEADER_KEY, out var bytes))
                throw new NcaKeyException($"Set of keys doesn't contain the expected key \"{HEADER_KEY}\".");

            using (var sha256 = SHA256.Create())
            {
                var keyHash = KeyParser.ByteArrayToHexString(sha256.TransformFinalBlock(bytes, 0, bytes.Length));

                if (keyHash != "AEAAB1CA08ADF9BEF12991F369E3C567D6881E4E4A6A47A51F6E4877062D542D")
                    throw new NcaKeyException($"Key \"{HEADER_KEY}\" is not valid.");
            }

            return bytes;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _baseStream.Dispose();
            _headerDecryptor.Dispose();
        }

        public override void Flush()
        {
            throw new NotSupportedException($"\"{nameof(Flush)}\" not supported.");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException($"\"{nameof(Seek)}\" not supported.");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException($"\"{nameof(SetLength)}\" not supported.");
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current
        /// stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="outBuffer">
        /// An array of bytes. When this method returns, the buffer contains the specified
        /// byte array with the values between offset and (offset + count - 1) replaced by
        /// the bytes read from the current source.
        /// </param>
        /// <param name="outOffset">
        /// The zero-based byte offset in buffer at which to begin storing the data read
        /// from the current stream.
        /// </param>
        /// <param name="outCount">
        /// The maximum number of bytes to be read from the current stream.
        /// </param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number
        /// of bytes requested if that many bytes are not currently available, or zero (0)
        /// if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// The sum of offset and count is larger than the buffer length.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// buffer is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// offset or count is negative.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// The stream does not support reading.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override int Read(byte[] outBuffer, int outOffset, int outCount)
        {
            if (outOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(outOffset));

            if (outCount < 0)
                throw new ArgumentOutOfRangeException(nameof(outCount));

            var initialPosition = _baseStream.Position;

            var sectorStartIndex = PositionToSectorIndex(initialPosition);

            var sectorStartPosition = sectorStartIndex * SECTOR_SIZE;
            _baseStream.Position = sectorStartPosition;

            var nbRemainingBytesToRead = outCount;
            var sectorIndexTemp = sectorStartIndex;
            var outOffsetTemp = outOffset;

            // Computes the first offset where to start copying from the decrypted sector buffer
            var copyOffset = initialPosition - sectorStartPosition;

            while (nbRemainingBytesToRead > 0)
            {
                if (sectorIndexTemp > 5)
                    throw new NotSupportedException($"\"{nameof(NcaDecryptionStream)}\" supports only NCA header in the moment, feel free to implement more!");

                // Decrypts header sector corresponding to the current position of the underlying stream
                var decryptedBuff = DecryptHeaderSector(sectorIndexTemp);

                // Computes the number of bytes to copy
                var nbBytesToCopy = (int)Math.Min(nbRemainingBytesToRead, decryptedBuff.Length - copyOffset);

                // Copies decrypted byte to the output buffer
                Array.Copy(decryptedBuff, copyOffset, outBuffer, outOffsetTemp, nbBytesToCopy);
                outOffsetTemp += nbBytesToCopy;

                nbRemainingBytesToRead -= nbBytesToCopy;
                sectorIndexTemp++;
                copyOffset = 0;
            }

            // Fix the underlying stream position according to the number of bytes read
            _baseStream.Position = initialPosition + outCount;

            return outCount;
        }

        private byte[] DecryptHeaderSector(long sectorIndex)
        {
            if (_lastDecryptedHeaderSectorIndex == sectorIndex)
                return _lastDecryptedHeaderSector;

            var sectorStartPos = sectorIndex * SECTOR_SIZE;

            var encryptedBuff = _baseStream.ReadBytes(SECTOR_SIZE, sectorStartPos);

            var fixedSectorIndex = _hotfixSectionHeaderSectorIndex && sectorIndex > 1 ? 0 : sectorIndex;
            _headerDecryptor.TransformBlock(encryptedBuff, 0, encryptedBuff.Length, _lastDecryptedHeaderSector, 0, (ulong)fixedSectorIndex);

            _lastDecryptedHeaderSectorIndex = sectorIndex;

            return _lastDecryptedHeaderSector;
        }

        private static long PositionToSectorIndex(long position)
        {
            return position / SECTOR_SIZE;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException($"\"{nameof(Write)}\" not supported.");
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => _baseStream.Length;

        public override long Position
        {
            get => _baseStream.Position;
            set => _baseStream.Position = value;
        }

        public static void DecryptHeaderToFile(string encryptedNcaSourceFilePath, string decryptedHeaderFilePath, KeySet keySet)
        {
            using (var ncaDecryptionStream = new NcaDecryptionStream(File.OpenRead(encryptedNcaSourceFilePath), keySet))
            {
                var decryptedHeaderBuff = new byte[HEADER_SIZE];
                ncaDecryptionStream.Read(decryptedHeaderBuff, 0, decryptedHeaderBuff.Length);
                File.WriteAllBytes(decryptedHeaderFilePath, decryptedHeaderBuff);
            }
        }
    }
}