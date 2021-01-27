using System;
using System.Globalization;
using System.Windows.Data;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.Overview;

namespace Emignatik.NxFileViewer.Views.Converters
{
    public class NacpLanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            

            if (!(value is NacpLanguage language))
                return LocalizationManager.Instance.Current.Keys.Lng_Unknown;

            switch (language)
            {
                case NacpLanguage.AmericanEnglish:
                    return LocalizationManager.Instance.Current.Keys.Lng_AmericanEnglish;
                case NacpLanguage.BritishEnglish:
                    return LocalizationManager.Instance.Current.Keys.Lng_BritishEnglish;
                case NacpLanguage.Japanese:
                    return LocalizationManager.Instance.Current.Keys.Lng_Japanese;
                case NacpLanguage.French:
                    return LocalizationManager.Instance.Current.Keys.Lng_French;
                case NacpLanguage.German:
                    return LocalizationManager.Instance.Current.Keys.Lng_German;
                case NacpLanguage.LatinAmericanSpanish:
                    return LocalizationManager.Instance.Current.Keys.Lng_LatinAmericanSpanish;
                case NacpLanguage.Spanish:
                    return LocalizationManager.Instance.Current.Keys.Lng_Spanish;
                case NacpLanguage.Italian:
                    return LocalizationManager.Instance.Current.Keys.Lng_Italian;
                case NacpLanguage.Dutch:
                    return LocalizationManager.Instance.Current.Keys.Lng_Dutch;
                case NacpLanguage.CanadianFrench:
                    return LocalizationManager.Instance.Current.Keys.Lng_CanadianFrench;
                case NacpLanguage.Portuguese:
                    return LocalizationManager.Instance.Current.Keys.Lng_Portuguese;
                case NacpLanguage.Russian:
                    return LocalizationManager.Instance.Current.Keys.Lng_Russian;
                case NacpLanguage.Korean:
                    return LocalizationManager.Instance.Current.Keys.Lng_Korean;
                case NacpLanguage.TraditionalChinese:
                    return LocalizationManager.Instance.Current.Keys.Lng_TraditionalChinese;
                case NacpLanguage.SimplifiedChinese:
                    return LocalizationManager.Instance.Current.Keys.Lng_SimplifiedChinese;
                case NacpLanguage.Unknown:
                    return LocalizationManager.Instance.Current.Keys.Lng_Unknown;
                default:
                    return LocalizationManager.Instance.Current.Keys.Lng_Unknown;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
