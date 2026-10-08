using System;
using System.IO;
using System.Linq;

namespace Emignatik.NxFileViewer.Services;

public static class EmptyDirectoryCleanup
{
    public static int Clean(string programDirectory)
    {
        var root = new DirectoryInfo(Path.GetFullPath(programDirectory));
        if (!root.Exists || (root.Attributes & FileAttributes.ReparsePoint) != 0) return 0;
        return Visit(root, true);
    }

    private static int Visit(DirectoryInfo directory, bool isRoot)
    {
        var removed = 0;
        try
        {
            // Do not follow links or inspect development/private-data directories.
            if ((directory.Attributes & FileAttributes.ReparsePoint) != 0 ||
                directory.Name is ".git" or ".codex" or ".agents" or ".aws" ||
                directory.Name.Equals("test", StringComparison.OrdinalIgnoreCase)) return 0;
            foreach (var child in directory.EnumerateDirectories().ToArray()) removed += Visit(child, false);
            if (!isRoot && !directory.EnumerateFileSystemInfos().Any())
            {
                // Nonrecursive deletion preserves files created concurrently.
                directory.Delete(false);
                removed++;
            }
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
        return removed;
    }
}
