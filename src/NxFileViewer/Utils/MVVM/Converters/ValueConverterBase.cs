using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Emignatik.NxFileViewer.Utils.MVVM.Converters
{
    public abstract class ValueConverterBase<TView, TViewModel> : IValueConverter
    {
        object? IValueConverter.Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not TViewModel && value != null)
            {
                Debug.Fail($"{this.GetType().Name} invalid usage: «{typeof(TViewModel).Name}» expected!");
                return default(TViewModel);
            }

            var tViewModel = (TViewModel)value!;
            return ConvertForView(tViewModel, parameter, culture);
        }

        object? IValueConverter.ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TView) && value != null)
            {
                Debug.Fail($"{this.GetType().Name} invalid usage: «{typeof(TView).Name}» expected!");
                return default(TView);
            }

            var tView = (TView)value!;
            return ConvertForViewModel(tView, parameter, culture);
        }

        /// <summary>
        /// Converts the ViewModel value to the View value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        protected abstract TView? ConvertForView(TViewModel? value, object parameter, CultureInfo culture);

        /// <summary>
        /// Converts the View value to the ViewModel value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        protected abstract TViewModel? ConvertForViewModel(TView? value, object parameter, CultureInfo culture);

    }
}