using System.Windows;
using System.Windows.Controls;

namespace Emignatik.NxFileViewer.Views.CustomControls
{
    public class PlayStopButton : Button
    {
        public static readonly DependencyProperty PlayStopButtonStateProperty = DependencyProperty.Register(
            "PlayStopButtonState", typeof(PlayStopButtonState), typeof(PlayStopButton), new PropertyMetadata(default(PlayStopButtonState)));

        public PlayStopButtonState PlayStopButtonState
        {
            get => (PlayStopButtonState)GetValue(PlayStopButtonStateProperty);
            set => SetValue(PlayStopButtonStateProperty, value);
        }
    }

    public enum PlayStopButtonState
    {
        Play,
        Stop,
        Cancelling
    }
}
