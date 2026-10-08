using System.Diagnostics;
using System.IO;

namespace Emignatik.NxFileViewer.Services;

public static class PluginGuiLauncher
{
    public static ProcessStartInfo CreateStartInfo(string executable, bool nand)
    {
        var info = new ProcessStartInfo(Path.GetFullPath(executable))
        {
            WorkingDirectory = Path.GetDirectoryName(Path.GetFullPath(executable))!,
            UseShellExecute = true
        };
        if (nand) info.ArgumentList.Add("--gui");
        return info;
    }

    public static void Open(string executable, bool nand)
    {
        using var process = Process.Start(CreateStartInfo(executable, nand));
    }
}
