using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.KeysManagement;

public static partial class KeyReaderMessageLogger
{
    public static void Log(ILogger logger, string message, string unusedKeyMessage)
    {
        var match = UnusedKeyMessageRegex().Match(message.Trim());
        if (match.Success)
            logger.LogInformation(string.Format(unusedKeyMessage, match.Groups[1].Value));
        else
            logger.LogWarning(message);
    }

    [GeneratedRegex("^Failed to match key ([a-zA-Z0-9_]+)$", RegexOptions.CultureInvariant)]
    private static partial Regex UnusedKeyMessageRegex();
}
