using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Emignatik.NxFileViewer.Views.CustomControls
{
    public class GridSplitterEx : GridSplitter
    {
        public static readonly DependencyProperty TriggerAnimationProperty = DependencyProperty.Register(
            "TriggerAnimation", typeof(bool), typeof(GridSplitterEx), new PropertyMetadata(default(bool), OnTriggerAnimationPropertyChanged));

        private static void OnTriggerAnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gridSplitterEx = ((GridSplitterEx)d);
            if ((bool)e.NewValue)
            {
                gridSplitterEx.AnimationTimer.Stop();
                gridSplitterEx.AnimationTimer.Start();
            }
        }

        public GridSplitterEx()
        {
            AnimationTimer = new Timer(3000);
            AnimationTimer.Elapsed += OnAnimationDurationTimerElapsed;
        }

        private void OnAnimationDurationTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(OnAnimationDurationTimerElapsed, DispatcherPriority.Normal,sender, e);
                return;
            }
            this.AnimationTimer.Stop();
            TriggerAnimation = false;

        }

        private Timer AnimationTimer { get; }

        public bool TriggerAnimation
        {
            get => (bool)GetValue(TriggerAnimationProperty);
            set => SetValue(TriggerAnimationProperty, value);
        }
    }
}
