using System;
using System.Globalization;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Utils.MVVM.Converters;

namespace Emignatik.NxFileViewer.Views.Converters;

public class NacpLanguageConverter : ValueConverterBase<string, NacpLanguage>
{
    protected override string ConvertForView(NacpLanguage value, object parameter, CultureInfo culture)
    {
        return value switch
        {
            NacpLanguage.AmericanEnglish => LocalizationManager.Instance.Current.Keys.Lng_AmericanEnglish,
            NacpLanguage.BritishEnglish => LocalizationManager.Instance.Current.Keys.Lng_BritishEnglish,
            NacpLanguage.Japanese => LocalizationManager.Instance.Current.Keys.Lng_Japanese,
            NacpLanguage.French => LocalizationManager.Instance.Current.Keys.Lng_French,
            NacpLanguage.German => LocalizationManager.Instance.Current.Keys.Lng_German,
            NacpLanguage.LatinAmericanSpanish => LocalizationManager.Instance.Current.Keys.Lng_LatinAmericanSpanish,
            NacpLanguage.Spanish => LocalizationManager.Instance.Current.Keys.Lng_Spanish,
            NacpLanguage.Italian => LocalizationManager.Instance.Current.Keys.Lng_Italian,
            NacpLanguage.Dutch => LocalizationManager.Instance.Current.Keys.Lng_Dutch,
            NacpLanguage.CanadianFrench => LocalizationManager.Instance.Current.Keys.Lng_CanadianFrench,
            NacpLanguage.Portuguese => LocalizationManager.Instance.Current.Keys.Lng_Portuguese,
            NacpLanguage.Russian => LocalizationManager.Instance.Current.Keys.Lng_Russian,
            NacpLanguage.Korean => LocalizationManager.Instance.Current.Keys.Lng_Korean,
            NacpLanguage.TraditionalChinese => LocalizationManager.Instance.Current.Keys.Lng_TraditionalChinese,
            NacpLanguage.SimplifiedChinese => LocalizationManager.Instance.Current.Keys.Lng_SimplifiedChinese,
            NacpLanguage.Unknown => LocalizationManager.Instance.Current.Keys.Lng_Unknown,
            _ => LocalizationManager.Instance.Current.Keys.Lng_Unknown
        };
    }

    protected override NacpLanguage ConvertForViewModel(string? value, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}