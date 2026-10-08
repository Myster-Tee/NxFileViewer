using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Emignatik.NxFileViewer.Services.KeysManagement;

public enum TicketKeySaveResult { Added, AlreadyPresent, Conflict }

public static class TicketKeyStore
{
    private static readonly object Sync = new();
    public static TicketKeySaveResult AddMissing(string path, string rightsId, string key)
    {
        if (!Regex.IsMatch(rightsId, "\\A[0-9a-fA-F]{32}\\z") || !Regex.IsMatch(key, "\\A[0-9a-fA-F]{32}\\z"))
            throw new ArgumentException("Invalid ticket key format.");
        lock (Sync)
        {
            var text = File.Exists(path) ? File.ReadAllText(path) : "";
            var found = false;
            foreach (var line in text.Split('\n'))
            {
                var data = line.Split('#')[0].Split('=');
                if (data.Length != 2 || !data[0].Trim().Equals(rightsId, StringComparison.OrdinalIgnoreCase)) continue;
                if (!data[1].Trim().Equals(key, StringComparison.OrdinalIgnoreCase)) return TicketKeySaveResult.Conflict;
                found = true;
            }
            if (found) return TicketKeySaveResult.AlreadyPresent;
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
            var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                var newline = text.Contains("\r\n") ? "\r\n" : Environment.NewLine;
                File.WriteAllText(temporary, text + (text.Length > 0 && !text.EndsWith('\n') ? newline : "")
                    + rightsId.ToLowerInvariant() + " = " + key.ToLowerInvariant() + newline, new UTF8Encoding(false));
                File.Move(temporary, path, true);
                return TicketKeySaveResult.Added;
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
        }
    }
}
