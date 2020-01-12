using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Emignatik.NxFileViewer.Logging;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Views
{
    /// <summary>
    /// Logique d'interaction pour LoggingView.xaml
    /// </summary>
    public partial class LoggingView : UserControl
    {

        public static readonly DependencyProperty LogSourceProperty = DependencyProperty.Register(
            "LogSource", typeof(ILogSource), typeof(LoggingView), new PropertyMetadata(default(ILogSource), OnChanged));

        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var loggingView = (LoggingView) d;

            if (e.OldValue is ILogSource oldLogSource)
                oldLogSource.Log -= loggingView.OnLog;

            if (e.NewValue is ILogSource newLogSource)
                newLogSource.Log += loggingView.OnLog;
        }

        public ILogSource LogSource
        {
            get => (ILogSource) GetValue(LogSourceProperty);
            set => SetValue(LogSourceProperty, value);
        }

        public LoggingView()
        {
            InitializeComponent();
        }

        private void OnLog(LogLevel logLevel, string message)
        {
            var dispatcher = Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess()) // To prevent UI thread InvalidOperationException when log event comes from another thread
            {
                dispatcher.BeginInvoke(new Action(() =>
                {
                    OnLog(logLevel, message);
                }));
                return;
            }

            var tr = new TextRange(RichTextBoxLog.Document.ContentEnd, RichTextBoxLog.Document.ContentEnd)
            {
                Text = message + Environment.NewLine
            };

            SolidColorBrush color;
            if (logLevel >= LogLevel.Error)
                color = Brushes.Red;
            else if (logLevel >= LogLevel.Warning)
                color = Brushes.Orange;
            else
                color = Brushes.Blue;

            tr.ApplyPropertyValue(TextElement.ForegroundProperty, color);
        }

        private void MenuItemClearLogClick(object sender, RoutedEventArgs e)
        {
            RichTextBoxLog.Document.Blocks.Clear();
        }

    }
}
