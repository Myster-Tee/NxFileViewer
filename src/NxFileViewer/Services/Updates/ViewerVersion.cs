using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Emignatik.NxFileViewer.Services.Updates;

/// <summary>Semantic precedence, including numeric preview identifiers and ignored build metadata.</summary>
public sealed class ViewerVersion : IComparable<ViewerVersion>
{
    public Version Numeric { get; }
    private readonly string[] _preview;
    private ViewerVersion(Version numeric, string[] preview) { Numeric = numeric; _preview = preview; }

    public static ViewerVersion FromNumeric(Version version) =>
        new(new Version(version.Major, version.Minor, Math.Max(0, version.Build)), Array.Empty<string>());

    public static bool TryParse(string? text, out ViewerVersion version)
    {
        version = null!;
        if (string.IsNullOrEmpty(text)) return false;
        var match = Regex.Match(text, @"^[vV]?(\d+\.\d+\.\d+)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*)?\z");
        if (!match.Success || !Version.TryParse(match.Groups[1].Value, out var numeric)) return false;
        var preview = match.Groups[2].Success ? match.Groups[2].Value.Split('.') : Array.Empty<string>();
        if (preview.Any(p => p.All(char.IsAsciiDigit) && p.Length > 1 && p[0] == '0')) return false;
        version = new ViewerVersion(numeric, preview);
        return true;
    }

    public int CompareTo(ViewerVersion? other)
    {
        if (other == null) return 1;
        var numeric = Numeric.CompareTo(other.Numeric);
        if (numeric != 0) return numeric;
        if (_preview.Length == 0 || other._preview.Length == 0)
            return (_preview.Length == 0 ? 1 : 0).CompareTo(other._preview.Length == 0 ? 1 : 0);
        for (var i = 0; i < Math.Min(_preview.Length, other._preview.Length); i++)
        {
            var left = _preview[i];
            var right = other._preview[i];
            var leftNumeric = left.All(char.IsAsciiDigit);
            var rightNumeric = right.All(char.IsAsciiDigit);
            var result = leftNumeric && rightNumeric
                ? (left.Length != right.Length ? left.Length.CompareTo(right.Length) : string.CompareOrdinal(left, right))
                : leftNumeric != rightNumeric ? (leftNumeric ? -1 : 1) : string.CompareOrdinal(left, right);
            if (result != 0) return result;
        }
        return _preview.Length.CompareTo(other._preview.Length);
    }
}
