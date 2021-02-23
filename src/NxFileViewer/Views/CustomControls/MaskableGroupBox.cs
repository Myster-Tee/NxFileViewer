using System.Windows;
using System.Windows.Controls;

namespace Emignatik.NxFileViewer.Views.CustomControls
{
    public class MaskableGroupBox : GroupBox
    {
        public static readonly DependencyProperty IsMaskedProperty = DependencyProperty.Register(
            "IsMasked", typeof(bool), typeof(MaskableGroupBox), new PropertyMetadata(false));

        public bool IsMasked
        {
            get => (bool)GetValue(IsMaskedProperty);
            set => SetValue(IsMaskedProperty, value);
        }
    }
}
