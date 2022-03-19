using System;
using System.Globalization;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Utils.MVVM.Converters;

namespace Emignatik.NxFileViewer.Views.Converters;

public class NcasValidityConverter : ValueConverterBase<string, NcasValidity>
{
    protected override string ConvertForView(NcasValidity value, object parameter, CultureInfo culture)
    {
        switch (value)
        {
            case NcasValidity.NoNca:
                return LocalizationManager.Instance.Current.Keys.NcasValidity_NoNca;
            case NcasValidity.Unchecked:
                return LocalizationManager.Instance.Current.Keys.NcasValidity_Unchecked;
            case NcasValidity.InProgress:
                return LocalizationManager.Instance.Current.Keys.NcasValidity_InProgress;
            case NcasValidity.Invalid:
                return LocalizationManager.Instance.Current.Keys.NcasValidity_Invalid;
            case NcasValidity.Valid:
                return LocalizationManager.Instance.Current.Keys.NcasValidity_Valid;
            case NcasValidity.Error:
                return LocalizationManager.Instance.Current.Keys.NcasValidity_Error;
            default:
                return LocalizationManager.Instance.Current.Keys.NcasValidity_Unknown;
        }
    }

    protected override NcasValidity ConvertForViewModel(string? value, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}