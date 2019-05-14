using System;
using System.Linq;
using System.Security.Cryptography;

namespace Emignatik.NxFileViewer.Crypto
{
    public class XtsAesCryptoTransform : IXtsAesCryptoTransform
    {
        private const int BLOCK_SIZE = 16;
        public const byte GF_128_FDBK = 0x87;

        private readonly byte[] _key1;
        private readonly byte[] _key2;

        public XtsAesCryptoTransform(byte[] key1, byte[] key2, TransformationMode transformationMode)
        {
            _key1 = key1 ?? throw new ArgumentNullException(nameof(key1));
            _key2 = key2 ?? throw new ArgumentNullException(nameof(key2));

            //TODO: check keys length

            Mode = transformationMode;
        }

        public int BlockSize => BLOCK_SIZE;

        public TransformationMode Mode { get; }

        public byte[] BuildPlaintextTweak2(ulong sector)
        {
            var tweak = new byte[BLOCK_SIZE];

            for (var i = BLOCK_SIZE - 1; i >= 0; i--)
            {
                tweak[i] = (byte) (sector & 0xFF);
                sector = sector >> 8;
            }

            return tweak;
        }

        public byte[] BuildPlaintextTweak(ulong sectorNumber)
        {
            var tweak = new byte[BLOCK_SIZE];
            for (var i = 0; i < BLOCK_SIZE; i++)
            {
                tweak[i] = (byte) (sectorNumber & 0xFF);
                sectorNumber = sectorNumber >> 8; // ulong is 64bits (8 bytes), the biggest C# integer. Therefore last 8 bytes will be padded to 0
            }

            return tweak;
        }

        private byte[] EncryptTweak(byte[] plaintextTweak)
        {
            using (var aes = Aes.Create())
            {
                if (aes == null) throw new Exception("AES missing!");
                aes.Key = _key2;
                aes.IV = new byte[BLOCK_SIZE];
                aes.Padding = PaddingMode.None;
                using (var encryptor = aes.CreateEncryptor())
                {
                    return encryptor.TransformFinalBlock(plaintextTweak, 0, plaintextTweak.Length);
                }
            }
        }

        private byte[] BuildTweak(ulong dataUnitNumber)
        {
            var t1 = BuildPlaintextTweak(1);
            var t2 = BuildPlaintextTweak2(1);

            var plaintextTweak = BuildPlaintextTweak2(dataUnitNumber).ToArray();
            return EncryptTweak(plaintextTweak);
        }


        public void Dispose()
        {
            //TODO implémenter qqch
        }


        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset, ulong sectorIndex)
        {
            var tweak = BuildTweak(sectorIndex);

            if (inputCount % BLOCK_SIZE != 0) // Not a multiple of BLOCK_SIZE
            {
                throw new Exception($"{nameof(inputCount)} should be a multiple of {BLOCK_SIZE}");
            }

            var outputBlockStartIndex = outputOffset;
            for (var inputBlockStartIndex = inputOffset; inputBlockStartIndex < inputOffset + inputCount; inputBlockStartIndex += BLOCK_SIZE)
            {
                // Create cc: merge the Tweak and the current input block
                var cc = new byte[BLOCK_SIZE];
                for (var blockByteIndex = 0; blockByteIndex < BLOCK_SIZE; blockByteIndex++)
                {
                    cc[blockByteIndex] = (byte) (inputBuffer[inputBlockStartIndex + blockByteIndex] ^ tweak[blockByteIndex]);
                }

                // Create pp: decrypt/encrypt the current block with Key1
                byte[] pp;
                using (var aes = Aes.Create())
                {
                    if (aes == null) throw new Exception("AES missing!");
                    aes.Key = _key1;
                    aes.IV = new byte[BLOCK_SIZE];
                    aes.Padding = PaddingMode.None;

                    switch (Mode)
                    {
                        case TransformationMode.DECRYPT:
                            using (var decryptor = aes.CreateDecryptor())
                            {
                                pp = decryptor.TransformFinalBlock(cc, 0, cc.Length);
                            }
                            break;
                        case TransformationMode.ENCRYPT:
                            using (var encryptor = aes.CreateEncryptor())
                            {
                                pp = encryptor.TransformFinalBlock(cc, 0, cc.Length);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                // Create p: Merge the tweak in the current decrypted/encrypted block
                for (var blockByteIndex = 0; blockByteIndex < BLOCK_SIZE; blockByteIndex++)
                {
                    outputBuffer[outputBlockStartIndex + blockByteIndex] = (byte) (pp[blockByteIndex] ^ tweak[blockByteIndex]);
                }

                // Create the next block tweak
                byte carry = 0x00;
                for (var i = 0; i < BLOCK_SIZE; i++)
                {
                    var prevTweakByte = tweak[i];
                    tweak[i] = (byte) ((byte) ((byte) (prevTweakByte << 1) + carry) & 0xFF);
                    carry = (byte) (prevTweakByte >> 7 & 1);
                }

                if (carry != 0)
                {
                    tweak[0] ^= GF_128_FDBK;
                }

                outputBlockStartIndex += BLOCK_SIZE;
            }

            return 0; //TODO: à finir
        }
    }
}