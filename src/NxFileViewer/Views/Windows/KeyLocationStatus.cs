using System;
using System.Collections.Generic;
using System.IO;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services.KeysManagement;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.Views.Windows;

public sealed record KeyLocationStatus(string Location, string FilePath, KeyFileValidationResult Validation, bool IsActive, string? Error)
{
    public string Summary => Error ?? SettingsWindowViewModel.BuildValidationSummary(Validation);

    public static IReadOnlyList<KeyLocationStatus> Inspect(string programPath, string sharedPath, string? customPath, string? activePath, bool titleKeys)
    {
        var keys = LocalizationManager.Instance.Current.Keys;
        var results = new List<KeyLocationStatus>();
        Add(keys.Keys_ProgramFolder, programPath);
        Add(keys.Keys_SharedFolder, sharedPath);
        if (!string.IsNullOrWhiteSpace(customPath)) Add(keys.SettingsView_Title_KeysCustomFilePath, customPath);
        return results;

        void Add(string label, string path)
        {
            var missing = new KeyFileValidationResult(false, 0, [], [], []);
            try
            {
                var fullPath = path.ToFullPath();
                var validation = titleKeys ? KeyFileValidator.ValidateTitleKeys(fullPath) : KeyFileValidator.ValidateProdKeys(fullPath);
                var active = activePath != null && string.Equals(fullPath, Path.GetFullPath(activePath), StringComparison.OrdinalIgnoreCase);
                results.Add(new(label, fullPath, validation, active, null));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
            {
                results.Add(new(label, path, missing, false, ex.Message));
            }
        }
    }
}
