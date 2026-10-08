using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Emignatik.NxFileViewer.Services.KeysManagement;

public static partial class KeyFileValidator
{
    // CRC32 fingerprints used by NSZ/Ownfoil. These are checksums, not cryptographic keys.
    private static readonly IReadOnlyDictionary<string, uint> MasterKeyChecksums = new Dictionary<string, uint>
    {
        ["master_key_00"] = 3540309694,
        ["master_key_01"] = 3477638116,
        ["master_key_02"] = 2087460235,
        ["master_key_03"] = 4095912905,
        ["master_key_04"] = 3833085536,
        ["master_key_05"] = 2078263136,
        ["master_key_06"] = 2812171174,
        ["master_key_07"] = 1146095808,
        ["master_key_08"] = 1605958034,
        ["master_key_09"] = 3456782962,
        ["master_key_0a"] = 2012895168,
        ["master_key_0b"] = 3813624150,
        ["master_key_0c"] = 3881579466,
        ["master_key_0d"] = 723654444,
        ["master_key_0e"] = 2690905064,
        ["master_key_0f"] = 4082108335,
        ["master_key_10"] = 788455323,
        ["master_key_11"] = 1214507020,
        ["master_key_12"] = 1051942134,
        ["master_key_13"] = 2476807835,
        ["master_key_14"] = 2448653557,
        ["master_key_15"] = 4071812001,
        ["master_key_16"] = 418653682,
    };

    public static KeyFileValidationResult ValidateProdKeys(string? filePath)
    {
        if (!IsExistingFile(filePath))
            return MissingFileResult();

        var entries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var invalidLines = new List<int>();
        ParseKeyValueFile(filePath!, entries, invalidLines);

        var missing = MasterKeyChecksums.Keys.Where(key => !entries.ContainsKey(key)).ToArray();
        var invalid = MasterKeyChecksums
            .Where(expected => entries.TryGetValue(expected.Key, out var value) && !MatchesChecksum(value, expected.Value))
            .Select(expected => expected.Key)
            .ToArray();
        var highestValidRevision = MasterKeyChecksums
            .Where(expected => entries.TryGetValue(expected.Key, out var value) && MatchesChecksum(value, expected.Value))
            .Select(expected => Convert.ToInt32(expected.Key[^2..], 16))
            .DefaultIfEmpty(-1)
            .Max();
        var unsupported = entries.Keys
            .Where(key => MasterKeyNameRegex().IsMatch(key) &&
                          (!MasterKeyChecksums.ContainsKey(key) ||
                           MasterKeyFirmwareMap.GetSupportedFirmware(Convert.ToInt32(key[^2..], 16)) == null))
            .OrderBy(key => key, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new KeyFileValidationResult(true, entries.Count, missing, invalid, invalidLines,
            highestValidRevision < 0 ? null : highestValidRevision, unsupported);
    }

    public static KeyFileValidationResult ValidateTitleKeys(string? filePath)
    {
        if (!IsExistingFile(filePath))
            return MissingFileResult();

        var validEntries = 0;
        var invalidLines = new List<int>();
        var lineNumber = 0;
        foreach (var rawLine in File.ReadLines(filePath!))
        {
            lineNumber++;
            var line = StripComment(rawLine).Trim();
            if (line.Length == 0)
                continue;

            var match = KeyValueRegex().Match(line);
            if (!match.Success || match.Groups[1].Value.Length != 32 || match.Groups[2].Value.Length != 32)
                invalidLines.Add(lineNumber);
            else
                validEntries++;
        }

        return new KeyFileValidationResult(true, validEntries, [], [], invalidLines);
    }

    private static void ParseKeyValueFile(string filePath, IDictionary<string, string> entries, ICollection<int> invalidLines)
    {
        var lineNumber = 0;
        foreach (var rawLine in File.ReadLines(filePath))
        {
            lineNumber++;
            var line = StripComment(rawLine).Trim();
            if (line.Length == 0)
                continue;

            var match = KeyValueRegex().Match(line);
            if (!match.Success || match.Groups[2].Value.Length % 2 != 0)
            {
                invalidLines.Add(lineNumber);
                continue;
            }

            entries[match.Groups[1].Value] = match.Groups[2].Value;
        }
    }

    private static string StripComment(string line)
    {
        var commentIndex = line.IndexOf('#');
        return commentIndex < 0 ? line : line[..commentIndex];
    }

    private static bool MatchesChecksum(string hexValue, uint expectedChecksum)
    {
        if (hexValue.Length != 32)
            return false;

        try
        {
            return ComputeCrc32(Convert.FromHexString(hexValue)) == expectedChecksum;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static uint ComputeCrc32(ReadOnlySpan<byte> bytes)
    {
        var crc = uint.MaxValue;
        foreach (var value in bytes)
        {
            crc ^= value;
            for (var bit = 0; bit < 8; bit++)
                crc = (crc >> 1) ^ ((crc & 1) == 0 ? 0 : 0xedb88320u);
        }
        return ~crc;
    }

    private static bool IsExistingFile(string? filePath)
    {
        try
        {
            return !string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath);
        }
        catch
        {
            return false;
        }
    }

    private static KeyFileValidationResult MissingFileResult() => new(false, 0, [], [], []);

    [GeneratedRegex(@"^\s*([a-zA-Z0-9_]+)\s*=\s*([a-fA-F0-9]+)\s*$", RegexOptions.CultureInvariant)]
    private static partial Regex KeyValueRegex();

    [GeneratedRegex(@"^master_key_[0-9a-f]{2}$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex MasterKeyNameRegex();
}
