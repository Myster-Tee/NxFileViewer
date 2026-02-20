using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Emignatik.NxFileViewer.Models.Overview;

namespace Emignatik.NxFileViewer.Utils.LibHacExtensions;

/// <summary>
/// Handles decompression of the NACP title block introduced in a newer Switch firmware.
///
/// Legacy NACP format:
///   [0x0000 – 0x2FFF]  16 × NacpLanguageEntry (each 0x300 bytes: 0x200 name + 0x100 author)
///   [0x3000 – 0x3FFF]  Metadata fields
///
/// New compressed format (flag byte at 0x3215 != 0):
///   [0x0000 – 0x2FFF]  Compressed title block:
///                          u16 LE  compressed_size
///                          u8[]    compressed_data[compressed_size]  (raw DEFLATE, wbits=-15)
///   [0x3000 – 0x3FFF]  Metadata fields  (unchanged, uncompressed)
///
/// The compressed data decompresses to up to 32 NacpLanguageEntry blocks (0x300 bytes each).
/// Indices 0–15 map to <see cref="NacpLanguage"/> values 0–15 (legacy languages).
/// Indices 16–30 map to the extended <see cref="NacpLanguage"/> values added in newer firmware.
/// </summary>
internal static class NacpTitleBlockDecompressor
{
    private const int TitleBlockSize  = 0x3000;
    private const int MetadataOffset  = 0x3000;
    private const int FlagOffset      = 0x3215;
    private const int EntrySize       = 0x300;
    private const int NameSize        = 0x200;
    private const int PublisherSize   = 0x100;
    private const int LegacyLangCount = 16;

    /// <summary>
    /// Returns true when the raw NACP bytes use the new compressed title block format.
    /// </summary>
    public static bool IsCompressed(ReadOnlySpan<byte> nacpBytes)
        => nacpBytes.Length > FlagOffset && nacpBytes[FlagOffset] != 0;

    /// <summary>
    /// If the buffer uses the new compressed format, decompresses the title block and returns
    /// a new byte array whose first 0x3000 bytes are the first 16 language entries (compatible
    /// with LibHac's <c>ApplicationControlProperty</c> struct), followed by the original
    /// metadata section verbatim.  If already legacy format, returns a copy unchanged.
    /// </summary>
    public static byte[] DecompressIfNeeded(ReadOnlySpan<byte> nacpBytes)
    {
        if (!IsCompressed(nacpBytes))
            return nacpBytes.ToArray();

        var decompressed = Decompress(nacpBytes);

        int metaLen = Math.Max(0, nacpBytes.Length - MetadataOffset);
        byte[] result = new byte[TitleBlockSize + metaLen];

        decompressed.AsSpan(0, TitleBlockSize).CopyTo(result);

        if (metaLen > 0)
            nacpBytes.Slice(MetadataOffset, metaLen).CopyTo(result.AsSpan(TitleBlockSize));

        return result;
    }

    /// <summary>
    /// Returns title entries for extended languages (index 16+) present in the compressed
    /// title block, keyed by their <see cref="NacpLanguage"/> value.
    /// Returns an empty dictionary for legacy uncompressed NACPs.
    /// Only entries with a non-empty name are included.
    /// </summary>
    public static IReadOnlyDictionary<NacpLanguage, NacpTitleEntry> GetExtendedTitleEntries(ReadOnlySpan<byte> rawNacpBytes)
    {
        var result = new Dictionary<NacpLanguage, NacpTitleEntry>();

        if (!IsCompressed(rawNacpBytes))
            return result;

        byte[] decompressed;
        try
        {
            decompressed = Decompress(rawNacpBytes);
        }
        catch (InvalidDataException)
        {
            return result;
        }

        int totalEntries = decompressed.Length / EntrySize;

        foreach (NacpLanguage lang in Enum.GetValues<NacpLanguage>())
        {
            int index = (int)lang;
            if (index < LegacyLangCount || index >= totalEntries)
                continue;

            int offset       = index * EntrySize;
            string name      = Encoding.UTF8.GetString(decompressed, offset, NameSize).TrimEnd('\0');
            string publisher = Encoding.UTF8.GetString(decompressed, offset + NameSize, PublisherSize).TrimEnd('\0');

            if (!string.IsNullOrEmpty(name))
                result[lang] = new NacpTitleEntry(name, publisher);
        }

        return result;
    }

    // ── Private helpers ───────────────────────────────────────────────────────────────────────

    private static byte[] Decompress(ReadOnlySpan<byte> nacpBytes)
    {
        if (nacpBytes.Length < 2)
            throw new InvalidDataException("NACP buffer too small to contain compressed title block header.");

        ushort compressedSize = (ushort)(nacpBytes[0] | (nacpBytes[1] << 8));
        int dataStart = 2;
        int dataEnd   = dataStart + compressedSize;

        if (dataEnd > MetadataOffset)
            throw new InvalidDataException(
                $"NACP compressed_size (0x{compressedSize:X}) causes compressed data " +
                $"(ends at 0x{dataEnd:X}) to overflow the title block region (0x{MetadataOffset:X}).");

        try
        {
            using var input   = new MemoryStream(nacpBytes.Slice(dataStart, compressedSize).ToArray());
            using var deflate = new DeflateStream(input, CompressionMode.Decompress);
            using var output  = new MemoryStream(TitleBlockSize);
            deflate.CopyTo(output);
            byte[] decompressed = output.ToArray();

            if (decompressed.Length < TitleBlockSize)
                throw new InvalidDataException(
                    $"Decompressed NACP title block is 0x{decompressed.Length:X} bytes; " +
                    $"expected at least 0x{TitleBlockSize:X}.");

            return decompressed;
        }
        catch (Exception ex) when (ex is not InvalidDataException)
        {
            throw new InvalidDataException("Failed to decompress NACP title block.", ex);
        }
    }
}