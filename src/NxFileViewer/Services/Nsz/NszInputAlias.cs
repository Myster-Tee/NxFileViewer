using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.Nsz;

// Frozen Python distributions may ignore Python encoding environment variables.
// Keep every file name passed to NSZ ASCII, then restore the requested output name.
public sealed class NszInputAlias : IDisposable
{
    private readonly string _directory;
    private readonly string _outputDirectory;
    private readonly string _source;
    private readonly string _aliasName;
    public string SourceArgument { get; }
    public string KeysArgument { get; }

    public NszInputAlias(string source, string outputDirectory, string keys, CancellationToken token, ILogger? logger = null)
    {
        _source = source;
        _outputDirectory = Path.GetFullPath(outputDirectory);
        _aliasName = "input" + Path.GetExtension(source).ToLowerInvariant();
        var folder = ".nsz-input-" + Guid.NewGuid().ToString("N");
        _directory = Path.Combine(_outputDirectory, folder);
        SourceArgument = Path.Combine(folder, _aliasName);
        KeysArgument = Path.Combine(folder, "prod.keys");
        Directory.CreateDirectory(_directory);
        try
        {
            token.ThrowIfCancellationRequested();
            var alias = Path.Combine(_directory, _aliasName);
            if (!OperatingSystem.IsWindows() || !CreateHardLink(alias, Path.GetFullPath(source), IntPtr.Zero))
            {
                logger?.LogInformation("NSZ: preparing temporary input copy because a hard link is unavailable.");
                using var input = File.OpenRead(source);
                using var output = new FileStream(alias, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                var buffer = new byte[1024 * 1024];
                int count;
                while ((count = input.Read(buffer)) > 0)
                {
                    token.ThrowIfCancellationRequested();
                    output.Write(buffer, 0, count);
                }
            }
            token.ThrowIfCancellationRequested();
            File.Copy(keys, Path.Combine(_directory, "prod.keys"));
        }
        catch { Dispose(); throw; }
    }

    public void RestoreOutputName(NszOperation operation)
    {
        var extension = operation == NszOperation.Compress
            ? Path.GetExtension(_source).Equals(".xci", StringComparison.OrdinalIgnoreCase) ? ".xcz" : ".nsz"
            : Path.GetExtension(_source).Equals(".xcz", StringComparison.OrdinalIgnoreCase) ? ".xci" : ".nsp";
        var produced = Path.Combine(_outputDirectory, Path.ChangeExtension(_aliasName, extension));
        if (!File.Exists(produced) || new FileInfo(produced).Length == 0)
            throw new InvalidDataException(Localization.LocalizationManager.Instance.Current.Keys.Nsz_OutputMissing);
        var requested = Path.Combine(_outputDirectory, Path.ChangeExtension(Path.GetFileName(_source), extension));
        if (!string.Equals(produced, requested, StringComparison.Ordinal)) File.Move(produced, requested, false);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory)) Directory.Delete(_directory, recursive: true);
    }

    [DllImport("kernel32.dll", EntryPoint = "CreateHardLinkW", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CreateHardLink(string fileName, string existingFileName, IntPtr securityAttributes);
}
