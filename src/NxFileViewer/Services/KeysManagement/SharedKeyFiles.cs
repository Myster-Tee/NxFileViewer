using System;
using System.IO;

namespace Emignatik.NxFileViewer.Services.KeysManagement;

public static class SharedKeyFiles
{
    public static string DirectoryPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".switch");

    public static bool Copy(string source, string destination, bool overwrite)
    {
        source = Path.GetFullPath(source);
        destination = Path.GetFullPath(destination);
        if (source.Equals(destination, StringComparison.OrdinalIgnoreCase)) return false;
        if (!File.Exists(source)) throw new FileNotFoundException("Keys file not found.", source);
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        var temporary = destination + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.Copy(source, temporary);
            File.Move(temporary, destination, overwrite);
            return true;
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }
}
