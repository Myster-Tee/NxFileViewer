using System.Text.RegularExpressions;

namespace Emignatik.NxFileViewer.Services.FileRenaming;

public static partial class FileNameTextNormalizer
{
    // Tinfoil occasionally returns adjacent words without a separator, e.g. "ZeldaBreath".
    // Requiring at least two lowercase letters on both sides avoids common names such as iPhone/eShop.
    [GeneratedRegex(@"(?<=\p{Ll}{2})(?=\p{Lu}\p{Ll}{2})", RegexOptions.CultureInvariant)]
    private static partial Regex MissingWordSeparatorRegex();

    public static string RestoreMissingWordSeparators(string value) =>
        MissingWordSeparatorRegex().Replace(value, " ");
}
