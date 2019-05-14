using System;
using System.Globalization;
using System.Windows.Data;
using Emignatik.NxFileViewer.NxFormats.NACP.Structs;
using Emignatik.NxFileViewer.Properties;

namespace Emignatik.NxFileViewer.Views.Converters
{
    public class NacpLanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is NacpLanguage language))
                return Resources.Lng_Unknown;

            switch (language)
            {
                case NacpLanguage.AmericanEnglish:
                    return Resources.Lng_AmericanEnglish;
                case NacpLanguage.BritishEnglish:
                    return Resources.Lng_BritishEnglish;
                case NacpLanguage.Japanese:
                    return Resources.Lng_Japanese;
                case NacpLanguage.French:
                    return Resources.Lng_French;
                case NacpLanguage.German:
                    return Resources.Lng_German;
                case NacpLanguage.LatinAmericanSpanish:
                    return Resources.Lng_LatinAmericanSpanish;
                case NacpLanguage.Spanish:
                    return Resources.Lng_Spanish;
                case NacpLanguage.Italian:
                    return Resources.Lng_Italian;
                case NacpLanguage.Dutch:
                    return Resources.Lng_Dutch;
                case NacpLanguage.CanadianFrench:
                    return Resources.Lng_CanadianFrench;
                case NacpLanguage.Portuguese:
                    return Resources.Lng_Portuguese;
                case NacpLanguage.Russian:
                    return Resources.Lng_Russian;
                case NacpLanguage.Korean:
                    return Resources.Lng_Korean;
                case NacpLanguage.TraditionalChinese:
                    return Resources.Lng_TraditionalChinese;
                case NacpLanguage.SimplifiedChinese:
                    return Resources.Lng_SimplifiedChinese;
                case NacpLanguage.Unknown:
                    return Resources.Lng_Unknown;
                default:
                    return Resources.Lng_Unknown;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var localizedLanguage = value as string;

            if (localizedLanguage == Resources.Lng_AmericanEnglish)
                return NacpLanguage.AmericanEnglish;
            if (localizedLanguage == Resources.Lng_BritishEnglish)
                return NacpLanguage.BritishEnglish;
            if (localizedLanguage == Resources.Lng_Japanese)
                return NacpLanguage.Japanese;
            if (localizedLanguage == Resources.Lng_French)
                return NacpLanguage.French;
            if (localizedLanguage == Resources.Lng_German)
                return NacpLanguage.German;
            if (localizedLanguage == Resources.Lng_LatinAmericanSpanish)
                return NacpLanguage.LatinAmericanSpanish;
            if (localizedLanguage == Resources.Lng_Spanish)
                return NacpLanguage.Spanish;
            if (localizedLanguage == Resources.Lng_Italian)
                return NacpLanguage.Italian;
            if (localizedLanguage == Resources.Lng_Dutch)
                return NacpLanguage.Dutch;
            if (localizedLanguage == Resources.Lng_CanadianFrench)
                return NacpLanguage.CanadianFrench;
            if (localizedLanguage == Resources.Lng_Portuguese)
                return NacpLanguage.Portuguese;
            if (localizedLanguage == Resources.Lng_Russian)
                return NacpLanguage.Russian;
            if (localizedLanguage == Resources.Lng_Korean)
                return NacpLanguage.Korean;
            if (localizedLanguage == Resources.Lng_TraditionalChinese)
                return NacpLanguage.TraditionalChinese;
            if (localizedLanguage == Resources.Lng_SimplifiedChinese)
                return NacpLanguage.SimplifiedChinese;
            if (localizedLanguage == Resources.Lng_Unknown)
                return NacpLanguage.Unknown;
            
            return NacpLanguage.Unknown;
        }
    }
}
