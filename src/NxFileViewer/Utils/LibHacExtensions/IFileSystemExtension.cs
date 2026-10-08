using LibHac.Common.Keys;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Ns;
using LibHac.Tools.Fs;
using LibHac.Tools.FsSystem.NcaUtils;
using LibHac.Tools.FsSystem;
using LibHac.Tools.Ncm;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using LibHac.Fs.Fsa;

namespace Emignatik.NxFileViewer.Utils.LibHacExtensions;

public static class IFileSystemExtension
{
    public static IEnumerable<DirectoryEntryEx> FindCnmtEntries(this IFileSystem fileSystem)
    {
        foreach (var fileEntry in fileSystem.EnumerateEntries().Where(e => e.Type == DirectoryEntryType.File))
        {
            var fileName = fileEntry.Name;
            if (fileName.EndsWith("cnmt.nca", StringComparison.OrdinalIgnoreCase))
                yield return fileEntry;
        }
    }

    public static Nca? LoadNca(this IFileSystem fileSystem, string ncaId, KeySet keySet)
    {
        var partitionFileEntry = fileSystem.EnumerateEntries()
            .Where(e => e.Type == DirectoryEntryType.File)
            .FirstOrDefault(entry => entry.Name.StartsWith(ncaId + ".", StringComparison.OrdinalIgnoreCase));

        if (partitionFileEntry == null)
            return null;

        var ncaFile = fileSystem.LoadFile(partitionFileEntry);

        return new Nca(keySet, new FileStorage(ncaFile));
    }

    public static IEnumerable<Cnmt> LoadCnmts(this IFileSystem fileSystem, KeySet keySet)
    {
        foreach (var cnmtFileEntry in fileSystem.FindCnmtEntries())
        {
            var ncaFile = fileSystem.LoadFile(cnmtFileEntry);

            var nca = new Nca(keySet, new FileStorage(ncaFile));

            if (nca.Header.ContentType != NcaContentType.Meta)
                continue;

            var cnmtEntries = nca.FindEntriesAmongSections("*.cnmt");

            foreach (var (sectionFileSystem, cnmtEntry) in cnmtEntries)
            {
                var cnmtFile = sectionFileSystem.LoadFile(cnmtEntry);
                var cnmt = new Cnmt(cnmtFile.AsStream());
                yield return cnmt;
            }
        }
    }

    /// <summary>
    /// Loads control.nacp file contained in the specified NCA.
    /// Transparently handles the compressed title block format introduced in newer Switch firmware,
    /// where the flag byte at offset 0x3215 signals that the first 0x3000 bytes hold a
    /// raw-DEFLATE-compressed blob rather than plain NacpLanguageEntry structs.
    /// </summary>
    public static ApplicationControlProperty? LoadNacp(this IFileSystem fileSystem, string ncaId, KeySet keySet)
    {
        var nca = fileSystem.LoadNca(ncaId, keySet);
        if (nca == null)
            return null;

        var foundEntry = nca.FindEntriesAmongSections("control.nacp").FirstOrDefault();
        if (foundEntry == null)
            return null;

        var (sectionFileSystem, nacpEntry) = foundEntry;

        var nacpFile = sectionFileSystem.LoadFile(nacpEntry);

        // ── Read raw bytes ────────────────────────────────────────────────────────────────────
        // We can't read directly into blitStruct.ByteSpan because we may need to decompress
        // the title block first.  Read into a temporary buffer instead.
        nacpFile.GetSize(out long nacpFileSize).ThrowIfFailure();

        byte[] rawBytes = new byte[nacpFileSize];
        nacpFile.Read(out _, 0, rawBytes, ReadOption.None).ThrowIfFailure();

        // ── Decompress title block if the new firmware format flag is set ─────────────────────
        // New format: flag byte at 0x3215 != 0 means the first 0x3000 bytes contain
        //   u16 LE  compressed_size
        //   u8[]    compressed_data[compressed_size]   (raw DEFLATE, wbits = -15)
        // Legacy format: first 0x3000 bytes are 16 × NacpLanguageEntry structs verbatim.
        // DecompressIfNeeded returns a buffer with the same layout as the legacy format so that
        // LibHac's ApplicationControlProperty struct can be populated without any further changes.
        byte[] nacpBytes;
        try
        {
            nacpBytes = NacpTitleBlockDecompressor.DecompressIfNeeded(rawBytes);
        }
        catch (InvalidDataException)
        {
            // Decompression failed — fall back to raw bytes so callers can still read metadata.
            nacpBytes = rawBytes;
        }

        // ── Populate LibHac struct ────────────────────────────────────────────────────────────
        var blitStruct = new BlitStruct<ApplicationControlProperty>(1);

        // Copy only as many bytes as the struct can hold (handles both exact-size and oversized
        // decompressed buffers safely).
        int copyLen = Math.Min(nacpBytes.Length, blitStruct.ByteSpan.Length);
        nacpBytes.AsSpan(0, copyLen).CopyTo(blitStruct.ByteSpan);

        return blitStruct.Value;
    }

    public static IFile LoadFile(this IFileSystem fileSystem, DirectoryEntryEx directoryEntryEx, OpenMode openMode = OpenMode.Read)
    {
        using var uniqueRefFile = new UniqueRef<IFile>();
        fileSystem.OpenFile(ref uniqueRefFile.Ref, directoryEntryEx.FullPath.ToU8Span(), openMode).ThrowIfFailure();
        return uniqueRefFile.Release();
    }
}