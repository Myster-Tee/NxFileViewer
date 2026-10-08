using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Emignatik.NxFileViewer.Services.Integrity;

public static class FirmwareReferenceStore
{
    public static void Save(IReadOnlyDictionary<string, string> manifests, string applicationDirectory, CancellationToken token)
    {
        _ = new FirmwareIntegrityVerifier(manifests.Values);
        foreach (var name in manifests.Keys)
            if (Path.GetFileName(name) != name || !name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Invalid firmware manifest filename.");
        var root = Path.GetFullPath(Path.Combine(applicationDirectory, "fw"));
        Directory.CreateDirectory(root);
        var target = Path.Combine(root, "hashes");
        if ((File.GetAttributes(root) & FileAttributes.ReparsePoint) != 0 ||
            (Directory.Exists(target) && (File.GetAttributes(target) & FileAttributes.ReparsePoint) != 0))
            throw new IOException("Firmware references must be in the program directory.");
        var stage = Path.Combine(root, ".hashes-stage-" + Guid.NewGuid().ToString("N"));
        var backup = Path.Combine(root, "hashes-backup-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(stage);
        var backedUp = false;
        try
        {
            foreach (var manifest in manifests)
            {
                token.ThrowIfCancellationRequested();
                File.WriteAllText(Path.Combine(stage, manifest.Key), manifest.Value);
            }
            token.ThrowIfCancellationRequested();
            if (Directory.Exists(target)) { Directory.Move(target, backup); backedUp = true; }
            try { Directory.Move(stage, target); }
            catch
            {
                if (backedUp && !Directory.Exists(target)) Directory.Move(backup, target);
                throw;
            }
        }
        finally
        {
            // This unique directory was created here, and stays under the fixed fw root.
            if (Path.GetFullPath(stage).StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) && Directory.Exists(stage))
                Directory.Delete(stage, recursive: true);
        }
    }
}
