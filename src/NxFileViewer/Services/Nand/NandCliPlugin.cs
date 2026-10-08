using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Services.KeysManagement;

namespace Emignatik.NxFileViewer.Services.Nand;

public sealed class NandCliPlugin(IAppSettings settings, INandProcessRunner? processRunner = null,
    NandPluginManager? manager = null, IKeySetProviderService? keyProvider = null)
{
    private readonly INandProcessRunner _processRunner = processRunner ?? new NandProcessRunner();
    public static IReadOnlyList<string> PartitionNames { get; } = new[]
    {
        "BOOT0", "BOOT1", "PRODINFO", "PRODINFOF", "SAFE", "SYSTEM", "USER",
        "BCPKG2-1-Normal-Main", "BCPKG2-2-Normal-Sub", "BCPKG2-3-SafeMode-Main",
        "BCPKG2-4-SafeMode-Sub", "BCPKG2-5-Repair-Main", "BCPKG2-6-Repair-Sub"
    };

    public static IReadOnlyList<string> FindPartitions(string information) => PartitionNames
        .Where(name => Regex.IsMatch(information, @"(?m)^\s*(?:-\s*|NAND type\s*:\s*)" + Regex.Escape(name) + @"(?![\w-])"))
        .ToArray();

    public string ReadInformation(string source, IProgressReporter progress, CancellationToken token)
    {
        ValidateSource(source);
        manager?.Prepare(progress, token);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(token);
        timeout.CancelAfter(TimeSpan.FromSeconds(60));
        return Run(source, null, null, progress, timeout.Token);
    }

    public void Export(string source, string destination, string partition, IProgressReporter progress, CancellationToken token)
    {
        if (!PartitionNames.Contains(partition)) throw new ArgumentException(nameof(partition));
        source = ValidateSource(source);
        destination = Path.GetFullPath(destination);
        if (destination.StartsWith(@"\\", StringComparison.Ordinal) || destination.IndexOf(':', 2) >= 0 || File.Exists(destination) || Directory.Exists(destination))
            throw new IOException(LocalizationManager.Instance.Current.Keys.Nand_NewTarget);
        var temporaryDirectory = Path.Combine(Path.GetDirectoryName(destination)!, ".nxfv-nand-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(temporaryDirectory);
        var temporaryFile = Path.Combine(temporaryDirectory, partition + ".bin");
        try
        {
            Run(source, temporaryFile, partition, progress, token);
            token.ThrowIfCancellationRequested();
            if (!File.Exists(temporaryFile) || new FileInfo(temporaryFile).Length == 0)
                throw new IOException(LocalizationManager.Instance.Current.Keys.Nand_ExportFailed);
            File.Move(temporaryFile, destination, overwrite: false);
        }
        finally
        {
            try { Directory.Delete(temporaryDirectory, recursive: true); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }

    private static string ValidateSource(string source)
    {
        source = Path.GetFullPath(source);
        if (source.StartsWith(@"\\", StringComparison.Ordinal) || !File.Exists(source))
            throw new IOException(LocalizationManager.Instance.Current.Keys.Nand_SourceMissing);
        return source;
    }

    private string Run(string source, string? output, string? partition, IProgressReporter progress, CancellationToken token)
    {
        source = ValidateSource(source);
        manager?.Prepare(progress, token);
        var executable = manager?.ExecutablePath ?? settings.NandExecutablePath;
        if (string.IsNullOrWhiteSpace(executable) || !File.Exists(executable))
            throw new FileNotFoundException(LocalizationManager.Instance.Current.Keys.Nand_NotInstalled);
        var start = new ProcessStartInfo(Path.GetFullPath(executable))
        {
            UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardOutput = true, RedirectStandardError = true, RedirectStandardInput = true,
            WorkingDirectory = Path.GetDirectoryName(Path.GetFullPath(executable))!
        };
        start.ArgumentList.Add("-i");
        start.ArgumentList.Add(source);
        if (output == null) start.ArgumentList.Add("--info");
        else
        {
            start.ArgumentList.Add("-o"); start.ArgumentList.Add(output);
            start.ArgumentList.Add("-part=" + partition);
        }
        if (output == null)
        {
            var keyPath = settings.NandBisKeysPath;
            if (!string.IsNullOrWhiteSpace(keyPath))
            {
                if (!File.Exists(keyPath)) throw new FileNotFoundException(LocalizationManager.Instance.Current.Keys.Nand_KeysMissing);
            }
            else
            {
                keyPath = keyProvider?.ActualProdKeysFilePath;
                // A prod.keys without console BIS entries must still allow basic NAND information.
                if (!File.Exists(keyPath) || !File.ReadLines(keyPath!).Any(line =>
                        Regex.IsMatch(line, @"^\s*bis_key_0[0-3]\s*=\s*[0-9a-fA-F]{64}\s*(?:[#;].*)?$", RegexOptions.IgnoreCase)))
                    keyPath = null;
            }
            if (!string.IsNullOrWhiteSpace(keyPath))
            {
                start.ArgumentList.Add("-keyset"); start.ArgumentList.Add(Path.GetFullPath(keyPath));
            }
        }
        token.ThrowIfCancellationRequested();
        progress.SetMode(true);
        progress.SetText(output == null ? LocalizationManager.Instance.Current.Keys.Nand_Info : LocalizationManager.Instance.Current.Keys.Nand_Export);
        return _processRunner.Run(start, progress, token);
    }
}

public interface INandProcessRunner
{
    string Run(ProcessStartInfo start, IProgressReporter progress, CancellationToken token);
}

public sealed class NandProcessRunner(bool allowUsageExit = false) : INandProcessRunner
{
    public string Run(ProcessStartInfo start, IProgressReporter progress, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        // Local file operations need no elevation. Keep these overrides in the child process only.
        start.Environment["__COMPAT_LAYER"] = "RunAsInvoker";
        start.Environment["LC_ALL"] = "C";
        start.Environment["LANG"] = "C";
        using var process = Process.Start(start) ?? throw new IOException(LocalizationManager.Instance.Current.Keys.Nand_ExportFailed);
        // Never accept an interactive restore/overwrite prompt from upstream.
        process.StandardInput.Close();
        using var registration = token.Register(() =>
        {
            try { process.Kill(entireProcessTree: true); }
            catch (InvalidOperationException) { }
            catch (System.ComponentModel.Win32Exception) { }
        });
        var stdout = Capture(process.StandardOutput, start.ArgumentList.Contains("-o") ? progress : null);
        var stderr = Capture(process.StandardError);
        process.WaitForExit();
        Task.WaitAll(stdout, stderr);
        token.ThrowIfCancellationRequested();
        if (process.ExitCode != 0 && !(allowUsageExit && process.ExitCode == -1001))
            throw new IOException($"NxNandManager: exit code {process.ExitCode}.");
        return stdout.Result;
    }

    private static async Task<string> Capture(StreamReader reader, IProgressReporter? progress = null)
    {
        var result = new StringBuilder();
        var line = new StringBuilder();
        var buffer = new char[4096];
        int count;
        while ((count = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            if (result.Length < 1024 * 1024) result.Append(buffer, 0, Math.Min(count, 1024 * 1024 - result.Length));
            if (progress == null) continue;
            for (var index = 0; index < count; index++)
            {
                if (buffer[index] is '\r' or '\n')
                {
                    var match = Regex.Match(line.ToString(), @"\((\d{1,3})%\)");
                    if (match.Success && int.TryParse(match.Groups[1].Value, out var percentage) && percentage <= 100)
                    {
                        progress.SetMode(false);
                        progress.SetPercentage(percentage / 100.0);
                    }
                    line.Clear();
                }
                else if (line.Length < 4096) line.Append(buffer[index]);
            }
        }
        return result.ToString();
    }
}
