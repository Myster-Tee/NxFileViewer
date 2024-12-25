using System;
using System.Globalization;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Utils.MVVM.Converters;

namespace Emignatik.NxFileViewer.Views.Converters;

public class NcasIntegrityConverter : ValueConverterBase<string, NcasIntegrity>
{
    protected override string ConvertForView(NcasIntegrity value, object? parameter, CultureInfo culture)
    {
        switch (value)
        {
            case NcasIntegrity.Unchecked:
                return LocalizationManager.Instance.Current.Keys.NcasIntegrity_Unchecked;
            case NcasIntegrity.InProgress:
                return LocalizationManager.Instance.Current.Keys.NcasIntegrity_InProgress;
            case NcasIntegrity.Original:
                return LocalizationManager.Instance.Current.Keys.NcasIntegrity_Original;
            case NcasIntegrity.Incomplete:
                return LocalizationManager.Instance.Current.Keys.NcasIntegrity_Incomplete;
            case NcasIntegrity.Modified:
                return LocalizationManager.Instance.Current.Keys.NcasIntegrity_Modified;
            case NcasIntegrity.Corrupted:
                return LocalizationManager.Instance.Current.Keys.NcasIntegrity_Corrupted;
            case NcasIntegrity.Error:
                return LocalizationManager.Instance.Current.Keys.NcasIntegrity_Error;
            case NcasIntegrity.NoNca:
                return LocalizationManager.Instance.Current.Keys.NcasIntegrity_NoNca;
            default:
                return LocalizationManager.Instance.Current.Keys.NcasIntegrity_Unknown;
        }
    }

    protected override NcasIntegrity ConvertForViewModel(string? value, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}