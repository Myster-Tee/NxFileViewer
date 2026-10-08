using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Localization;

namespace Emignatik.NxFileViewer.Services.Updates;

public static class UpdateAvailabilitySummary
{
    public static string Format(string? viewerVersion, IEnumerable<ComponentUpdateResult> results)
    {
        var lines = results.Where(r => r.UpdateAvailable).Select(r => r.Name + ": " + r.Status);
        if (viewerVersion != null)
            lines = new[] { "NxFileViewer: " + string.Format(LocalizationManager.Instance.Current.Keys.Update_Available, viewerVersion) }.Concat(lines);
        return string.Join(Environment.NewLine, lines);
    }
}
