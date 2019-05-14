using System;

namespace Emignatik.NxFileViewer.Crypto
{
    public interface IXtsAesCryptoTransform: IDisposable
    {

        /// <summary>
        /// Gets the transformation mode
        /// </summary>
        TransformationMode Mode { get; }

        /// <summary>
        /// Gets the encryption/decryption block size
        /// </summary>
        int BlockSize { get; }

        /// <summary>
        /// </summary>
        /// <param name="inputBuffer">Input buffer</param>
        /// <param name="inputOffset">Offset in <see cref="inputBuffer"/> from which to start transformation</param>
        /// <param name="inputCount">Number of byte to transform starting at offset</param>
        /// <param name="outputBuffer">Output buffer where to write the transformation result</param>
        /// <param name="outputOffset"></param>
        /// <param name="sectorIndex"></param>
        /// <returns>The number of transformed bytes</returns>
        int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset, ulong sectorIndex);

    }
}