using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Emignatik.NxFileViewer.Logging;
using Emignatik.NxFileViewer.Styling.Theme;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Views.UserControls;

/// <summary>
/// Logique d'interaction pour LoggingView.xaml
/// </summary>
public partial class LoggingView : UserControl
{

    public static readonly DependencyProperty LogSourceProperty = DependencyProperty.Register(
        "LogSource", typeof(ILogSource), typeof(LoggingView), new PropertyMetadata(default(ILogSource), OnChanged));

    private readonly IBrushesProvider _brushesProvider;

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
        _brushesProvider = App.ServiceProvider.GetRequiredService<IBrushesProvider>();
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

        Brush brush;
        if (logLevel >= LogLevel.Error)
            brush = _brushesProvider.FontBrushError;
        else if (logLevel >= LogLevel.Warning)
            brush = _brushesProvider.FontBrushWarning;
        else
            brush = _brushesProvider.FontBrushDefault;

        var paragraph = new Paragraph();
        var run = new Run(message)
        {
            Foreground = brush
        };
        paragraph.Inlines.Add(run);
        RichTextBoxLog.Document.Blocks.Add(paragraph);
    }

    private void MenuItemClearLogClick(object sender, RoutedEventArgs e)
    {
        RichTextBoxLog.Document.Blocks.Clear();
    }

}