using System;
using System.Linq;
using System.Reflection;

namespace Emignatik.NxFileViewer.Services.Updates;

public static class ViewerDistribution
{
    // Embedded at build time, independent of install folder and machine-wide .NET.
    public static bool IsSelfContained => bool.TryParse(typeof(ViewerDistribution).Assembly
        .GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(a => a.Key == "ViewerSelfContained")?.Value, out var value) && value;
}
