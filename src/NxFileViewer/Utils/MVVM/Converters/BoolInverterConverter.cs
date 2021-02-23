using System.Globalization;

namespace Emignatik.NxFileViewer.Utils.MVVM.Converters
{
    public class BoolInverterConverter : ValueConverterBase<bool, bool>
    {
        protected override bool ConvertForView(bool value, object parameter, CultureInfo culture)
        {
            return !value;
        }

        protected override bool ConvertForViewModel(bool value, object parameter, CultureInfo culture)
        {
            return !value;
        }
    }
}
