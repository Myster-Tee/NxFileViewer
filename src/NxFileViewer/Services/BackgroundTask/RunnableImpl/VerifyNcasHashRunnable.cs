using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils;
using LibHac;
using LibHac.Fs.Fsa;
using LibHac.Tools.FsSystem;
using LibHac.Tools.Ncm;
using Microsoft.Extensions.Logging;
using ZstdSharp;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;

public class VerifyNcasHashRunnable : IVerifyNcasHashRunnable
{
    private readonly IAppSettings _appSettings;
    private const string NCA_HASH_CATEGORY = "NcaHash";

    private FileOverview? _fileOverview;
    private readonly ILogger _logger;

    public VerifyNcasHashRunnable(ILoggerFactory loggerFactory, IAppSettings appSettings)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
    }

    public bool SupportsCancellation => true;

    public bool SupportProgress => true;

    public void Setup(FileOverview fileOverview)
    {
        _fileOverview = fileOverview ?? throw new ArgumentNullException(nameof(fileOverview));
    }

    public void Run(IProgressReporter progressReporter, CancellationToken cancellationToken)
    {
        if (_fileOverview == null)
            throw new InvalidOperationException($"{nameof(Setup)} should be called first.");

        _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaHash_VerificationStart_Log);
        try
        {
            VerifyHashes(progressReporter, _fileOverview, cancellationToken);
        }
        finally
        {
            _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaHash_VerificationEnd_Log);
        }
    }


    private void VerifyHashes(IProgressReporter progressReporter, FileOverview fileOverview, CancellationToken cancellationToken)
    {
        fileOverview.NcasHashExceptions = null;

        // Build the list of all CnmtContentEntry with their corresponding CnmtItem
        // The CnmtContentEntry contains a reference to an NCA with the expected Sha256 hash
        var itemsToProcess = new List<Tuple<CnmtContentEntry, CnmtItem>>();
        foreach (var cnmtItem in fileOverview.CnmtContainers.Select(container => container.CnmtItem))
        {
            itemsToProcess.AddRange(cnmtItem.Cnmt.ContentEntries.Select(cnmtContentEntry =>
            {
                cnmtItem.Errors.RemoveAll(NCA_HASH_CATEGORY);
                return new Tuple<CnmtContentEntry, CnmtItem>(cnmtContentEntry, cnmtItem);
            }));
        }

        if (itemsToProcess.Count <= 0)
        {
            fileOverview.NcasHashValidity = NcasValidity.NoNca;
            return;
        }

        var occurredExceptions = new List<Exception>();
        var allValid = true;
        var operationCanceled = false;
        try
        {

            fileOverview.NcasHashValidity = NcasValidity.InProgress;

            var processedItem = 0;
            foreach (var (cnmtContentEntry, cnmtItem) in itemsToProcess)
            {
                cancellationToken.ThrowIfCancellationRequested();

                progressReporter.SetPercentage(0.0);

                var progressText = LocalizationManager.Instance.Current.Keys.NcaHash_ProgressText.SafeFormat(++processedItem, itemsToProcess.Count);
                progressReporter.SetText(progressText);

                var expectedNcaHash = cnmtContentEntry.Hash;
                var expectedNcaId = cnmtContentEntry.NcaId.ToStrId();

                // Search for the referenced NCA
                var ncaItem = fileOverview.NcaItems.FirstOrDefault(item => item.FileName.StartsWith(expectedNcaId + ".", StringComparison.OrdinalIgnoreCase));

                if (ncaItem == null)
                {
                    cnmtItem.Errors.Add(NCA_HASH_CATEGORY, LocalizationManager.Instance.Current.Keys.NcaHash_CnmtItem_Error_NcaMissing.SafeFormat(expectedNcaId));
                    continue;
                }

                try
                {
                    ncaItem.Errors.RemoveAll(NCA_HASH_CATEGORY);

                    //=============================================//
                    //===============> Verify Hash <===============//
                    VerifyFileHash(progressReporter, ncaItem.File, _appSettings.ProgressBufferSize, expectedNcaHash, cancellationToken, ncaItem.Name.ToLower().EndsWith(".ncz") ? true : false, out var hashValid);
                    //===============> Verify Hash <===============//
                    //=============================================//

                    if (!hashValid)
                    {
                        allValid = false;
                        ncaItem.Errors.Add(NCA_HASH_CATEGORY, LocalizationManager.Instance.Current.Keys.NcaHash_NcaItem_Invalid);
                        _logger.LogError(LocalizationManager.Instance.Current.Keys.NcaHash_Invalid_Log.SafeFormat(ncaItem.DisplayName));
                    }
                    else
                        _logger.LogInformation(LocalizationManager.Instance.Current.Keys.NcaHash_Valid_Log.SafeFormat(ncaItem.DisplayName));

                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    allValid = false;
                    occurredExceptions.Add(ex);
                    ncaItem.Errors.Add(NCA_HASH_CATEGORY, LocalizationManager.Instance.Current.Keys.NcaHash_NcaItem_Exception.SafeFormat(ex.Message));
                    _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.NcaHash_Exception_Log.SafeFormat(ncaItem.DisplayName, ex.Message));
                }
            }

        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(LocalizationManager.Instance.Current.Keys.Log_NcaHashCanceled);
            operationCanceled = true;
        }
        catch (Exception ex)
        {
            allValid = false;
            occurredExceptions.Add(ex);
            _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.NcasHash_Error_Log.SafeFormat(ex.Message));
        }

        if (operationCanceled)
        {
            fileOverview.NcasHashValidity = NcasValidity.Unchecked;
        }
        else if (occurredExceptions.Count > 0)
        {
            fileOverview.NcasHashExceptions = occurredExceptions;
            fileOverview.NcasHashValidity = NcasValidity.Error;
        }
        else
        {
            fileOverview.NcasHashValidity = allValid ? NcasValidity.Valid : NcasValidity.Invalid;
        }

    }
    private static byte readInt8(Stream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        return reader.ReadByte();
    }
    private static int ReadInt32(Stream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        return reader.ReadInt32();
    }
    private static long ReadInt64(Stream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        return reader.ReadInt64();
    }


    class Section
    {
        private Stream f;
        public long offset;
        public long size;
        public long cryptoType;
        private long padding;
        public byte[] cryptoKey = new byte[16];
        public byte[] cryptoCounter = new byte[16];
        public Section(Stream file)
        {
            f = file;
            offset = ReadInt64(f);
            size = ReadInt64(f);
            cryptoType = ReadInt64(f);
            padding = ReadInt64(f);
            file.Read(cryptoKey);
            file.Read(cryptoCounter);
        }
    }

    public class AESCTR
    {
        private readonly byte[] _key;
        private readonly byte[] _nonce;
        private byte[] _currentCtr;
        private IBufferedCipher _cipher;

        public AESCTR(byte[] key, byte[] nonce, int offset = 0)
        {
            _key = key;
            _nonce = nonce;
            Seek(offset);
        }

        public byte[] Encrypt(byte[] data)
        {
            _cipher.Init(true, new ParametersWithIV(new KeyParameter(_key), _currentCtr));
            return _cipher.DoFinal(data);
        }

        public byte[] Decrypt(byte[] data)
        {
            return Encrypt(data); // In CTR mode, encrypt and decrypt are the same operation.
        }

        public void Seek(long offset)
        {
            byte[] iv = new byte[16];
            Array.Copy(_nonce, 0, iv, 0, _nonce.Length); // Copy the nonce into the first 8 bytes.
            byte[] offsetBytes = BitConverter.GetBytes(offset >> 4);
            if (BitConverter.IsLittleEndian) // Convert to big-endian if necessary.
                Array.Reverse(offsetBytes);
            Array.Copy(offsetBytes, 0, iv, 8, offsetBytes.Length); // Copy the offset-derived bytes into the next 8 bytes.

            _currentCtr = iv;

            _cipher = new BufferedBlockCipher(new SicBlockCipher(new AesEngine()));
            _cipher.Init(true, new ParametersWithIV(new KeyParameter(_key), iv));
        }
    }

    private static bool VerifyNCZFileHash(IProgressReporter progressReporter, IFile file, long fileSize, IReadOnlyCollection<byte> expectedNcaHash, CancellationToken cancellationToken)
    {
        long ncaHeaderSize = 0x4000;
        var nczStream = file.AsStream();
        nczStream.Seek(0, SeekOrigin.Begin);
        byte[] header = new byte[ncaHeaderSize];
        nczStream.Read(header);
        byte[] magic = new byte[8];
        nczStream.Read(magic);

        if (!magic.SequenceEqual(System.Text.Encoding.ASCII.GetBytes("NCZSECTN")))
        {
            throw new Exception("No NCZSECTN found! Is this really a .ncz file?");
        }
        long sectionCount = ReadInt64(nczStream);
        var sections = new List<Section>();
        for (int i = 0; i < sectionCount; i++)
        {
            sections.Add(new Section(nczStream));
        }

        long nca_size = ncaHeaderSize;
        foreach (Section section in sections)
        {
            nca_size += section.size;
        }

        long pos = nczStream.Position;

        byte[] blockMagic = new byte[8];
        nczStream.Read(blockMagic);
        nczStream.Seek(pos, SeekOrigin.Begin);

        bool useBlockCompression = blockMagic.SequenceEqual(System.Text.Encoding.ASCII.GetBytes("NCZBLOCK"));
        if (useBlockCompression)
        {
            throw new Exception("Not implemented");
        }
        pos = nczStream.Position;
        var ncaStream = new DecompressionStream(nczStream);

        IncrementalHash sha256 = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        sha256.AppendData(header);

        foreach (Section s in sections)
        {
            long i = s.offset;
            var crypto = new AESCTR(s.cryptoKey, s.cryptoCounter);
            var end = s.offset + s.size;
            while (i < end)
            {
                cancellationToken.ThrowIfCancellationRequested();
                crypto.Seek(i);
                long chunkSz = end - i > 0x10000 ? 0x10000 : end - i;
                if (useBlockCompression)
                {
                    throw new Exception("Not implemented");
                }
                else
                {
                    byte[] chunk = new byte[chunkSz];
                    if (ncaStream.Read(chunk) == 0)
                    {
                        break;
                    }
                    if (s.cryptoType == 3 || s.cryptoType == 4) {
                        chunk = crypto.Encrypt(chunk);
                    }
                    sha256.AppendData(chunk);
                    progressReporter.SetPercentage(fileSize == 0 ? 0.0 : (double)(nczStream.Position / (double) fileSize));
                    i += chunkSz;
                }
            }
        }

        var x = sha256.GetHashAndReset();
        return x.SequenceEqual(expectedNcaHash);
    }
    private static void VerifyFileHash(IProgressReporter progressReporter, IFile file, int bufferSize, IReadOnlyCollection<byte> expectedNcaHash, CancellationToken cancellationToken, bool compressed, out bool hashValid)
    {
        if (file.GetSize(out var fileSize) != Result.Success)
            fileSize = 0;

        var sha256 = SHA256.Create();

        var ncaStream = file.AsStream();
        if (compressed)
        {
            hashValid = VerifyNCZFileHash(progressReporter, file, fileSize, expectedNcaHash, cancellationToken);
            return;
        }
        var buffer = new byte[bufferSize];

        decimal totalRead = 0;
        int read;
        while ((read = ncaStream.Read(buffer)) > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            sha256.TransformBlock(buffer, 0, read, null, 0);
            totalRead += read;
            progressReporter.SetPercentage(fileSize == 0 ? 0.0 : (double)(totalRead / fileSize));
        }

        sha256.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        var currentNcaHash = sha256.Hash!;

        hashValid = IsHashEqual(currentNcaHash, expectedNcaHash);
    }

    private static bool IsHashEqual(IReadOnlyList<byte> currentNcaHash, IReadOnlyCollection<byte> expectedNcaHash)
    {
        if (currentNcaHash.Count != expectedNcaHash.Count)
            return false;

        return !expectedNcaHash.Where((expectedByte, byteIndex) => currentNcaHash[byteIndex] != expectedByte).Any();
    }

}

public interface IVerifyNcasHashRunnable : IRunnable
{
    void Setup(FileOverview fileOverview);
}