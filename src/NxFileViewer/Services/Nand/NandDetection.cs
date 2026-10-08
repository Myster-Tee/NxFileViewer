using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Emignatik.NxFileViewer.Services.Nand;

public sealed record NandDetectionResult(string Type, IReadOnlyList<string> Partitions, bool NameOnly, long Size, int Parts)
{
    public string Information => "NAND type : " + Type + Environment.NewLine +
        "Size : " + Size + " bytes" + Environment.NewLine + "Files : " + Parts + Environment.NewLine +
        (NameOnly ? Localization.LocalizationManager.Instance.Current.Keys.Nand_NameCandidate : Localization.LocalizationManager.Instance.Current.Keys.Nand_Detected) +
        Environment.NewLine + string.Join(Environment.NewLine, Partitions.Select(name => " - " + name));
}

public static class NandDetection
{
    public static bool IsCandidateName(string path)
    {
        var name = Path.GetFileName(path.Replace('\\', '/'));
        var extension = Path.GetExtension(name).ToLowerInvariant();
        return extension is ".bin" or ".img" ||
            extension is ".enc" or ".dec" && NandCliPlugin.PartitionNames.Contains(Path.GetFileNameWithoutExtension(name), StringComparer.OrdinalIgnoreCase) ||
            NandCliPlugin.PartitionNames.Contains(name, StringComparer.OrdinalIgnoreCase) ||
            Regex.IsMatch(name, @"(?:^\d{2}$|\.(?:0|00)$)");
    }

    public static bool IsContinuation(string path)
        => IsContinuationName(path, File.Exists);

    public static bool IsContinuationName(string path, Func<string, bool> exists)
    {
        var name = Path.GetFileName(path.Replace('\\', '/'));
        if (NandCliPlugin.PartitionNames.Contains(name, StringComparer.OrdinalIgnoreCase) ||
            NandCliPlugin.PartitionNames.Contains(Path.GetFileNameWithoutExtension(name), StringComparer.OrdinalIgnoreCase)) return false;
        var match = Regex.Match(name, @"^(?<prefix>.*?)(?<number>\d+)(?<suffix>\.bin)?$", RegexOptions.IgnoreCase);
        if (!match.Success || !int.TryParse(match.Groups["number"].Value, out var number) || number == 0) return false;
        var prefix = match.Groups["prefix"].Value;
        // Recognize split conventions only when a corresponding first part exists.
        var first = Path.Combine(Path.GetDirectoryName(path)!, prefix + new string('0', match.Groups["number"].Length) + match.Groups["suffix"].Value);
        return exists(first) && IsCandidateName(first);
    }

    public static NandDetectionResult? Detect(string path, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        using var stream = File.OpenRead(path);
        if (stream.Length < 512 || IsContinuation(path)) return null;
        return Detect(stream, path, token, includeSplitFiles: true);
    }

    public static NandDetectionResult? Detect(Stream stream, string path, CancellationToken token = default, bool includeSplitFiles = false)
    {
        if (!stream.CanSeek || stream.Length < 512) return null;
        byte[] Read(long offset, int length)
        {
            token.ThrowIfCancellationRequested();
            if (offset < 0 || offset > stream.Length - length) return Array.Empty<byte>();
            stream.Position = offset;
            var bytes = new byte[length];
            stream.ReadExactly(bytes);
            return bytes;
        }
        bool Magic(long offset, string text) => Read(offset, text.Length).SequenceEqual(Encoding.ASCII.GetBytes(text));
        foreach (var offset in new long[] { 0x200, 0x800200, 0x1800200 })
        {
            if (!Magic(offset, "EFI PART")) continue;
            var header = Read(offset, 92);
            if (header.Length == 0) continue;
            var count = BitConverter.ToUInt32(header, 80);
            var entrySize = BitConverter.ToUInt32(header, 84);
            var entryLba = BitConverter.ToUInt64(header, 72);
            if (count is 0 or > 128 || entrySize is < 128 or > 1024 || entryLba > long.MaxValue / 512) continue;
            var names = new List<string>();
            for (var index = 0; index < count; index++)
            {
                var entryOffset = offset - 512 + (long)entryLba * 512 + index * entrySize;
                var entry = Read(entryOffset, (int)entrySize);
                if (entry.Length == 0) break;
                var name = Encoding.Unicode.GetString(entry, 56, 72).TrimEnd('\0');
                if (NandCliPlugin.PartitionNames.Contains(name, StringComparer.Ordinal)) names.Add(name);
            }
            // Generic PC GPT images must not be classified as Switch NAND.
            if (!names.Contains("PRODINFO") || !names.Contains("SYSTEM")) continue;
            if (offset != 0x200) names.InsertRange(0, new[] { "BOOT0", "BOOT1" });
            var parts = includeSplitFiles ? SplitFiles(path) : new[] { path };
            return new(offset == 0x200 ? "RAWNAND" : "FULL NAND", names.Distinct().ToArray(), false,
                includeSplitFiles ? parts.Sum(file => new FileInfo(file).Length) : stream.Length, parts.Count);
        }
        string? type = null;
        if (Magic(0, "CAL0")) type = "PRODINFO";
        else if (Magic(0x680, "CERTIF")) type = "PRODINFOF";
        else if (Read(0x530, 12).SequenceEqual(Convert.FromHexString("010021000E00000009000000"))) type = "BOOT0";
        else if (new long[] { 0x13B4, 0x13F0, 0x1424, 0x12E8, 0x12D0, 0x12F0, 0x40AF8, 0x40ADC, 0x40ACC, 0x40AC0 }.Any(offset => Magic(offset, "PK11"))) type = "BOOT1";
        if (type != null) return new(type, new[] { type }, false, stream.Length, 1);
        if (!IsCandidateName(path)) return null;
        var fileName = Path.GetFileNameWithoutExtension(path);
        type = NandCliPlugin.PartitionNames.FirstOrDefault(name => name.Equals(fileName, StringComparison.OrdinalIgnoreCase) || name.Equals(Path.GetFileName(path), StringComparison.OrdinalIgnoreCase));
        return type != null && stream.Length % 512 == 0 ? new(type, new[] { type }, true, stream.Length, 1) : null;
    }

    public static IReadOnlyList<string> SplitFiles(string path)
    {
        var name = Path.GetFileName(path);
        var match = Regex.Match(name, @"^(?<prefix>.*?)(?<number>0+)(?<suffix>\.bin)?$", RegexOptions.IgnoreCase);
        var files = new List<string> { path };
        if (!match.Success) return files;
        for (var index = 1; index < 100; index++)
        {
            var next = Path.Combine(Path.GetDirectoryName(path)!, match.Groups["prefix"].Value + index.ToString("D" + match.Groups["number"].Length) + match.Groups["suffix"].Value);
            if (!File.Exists(next)) break;
            files.Add(next);
        }
        return files;
    }
}
