using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Emignatik.NxFileViewer.Logging;
using Emignatik.NxFileViewer.NSP;
using Emignatik.NxFileViewer.Properties;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Views.NSP;
using log4net;
using log4net.Config;
using log4net.Core;

namespace Emignatik.NxFileViewer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILog _log;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;

            _log = LogManager.GetLogger(this.GetType());

            var logNotifier = new LogNotifier();
            BasicConfigurator.Configure(logNotifier);
            logNotifier.Log += OnLog;

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                KeySetProviderService.GetKeySet();
                _log.Info(Properties.Resources.InfoKeysSuccessfullyLoaded);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        public void SafeLoadFile(string filePath)
        {
            try
            {
                Settings.Default.LastOpenedFile = filePath;
                Settings.Default.Save();
            }
            catch
            {
            }

            try
            {
                var fileName = Path.GetFileName(filePath);
                _log.Info($"===> {fileName} <===");

                var nspInfoLoader = new NspInfoLoader(KeySetProviderService.GetKeySet());
                var nspInfo = nspInfoLoader.Load(filePath);

                //TODO: split "code behing logic" to separate view model
                ((MainWindowViewModel)this.DataContext).FileViewModel = new NspInfoViewModel(nspInfo)
                {
                    Source = filePath,
                };
            }
            catch (Exception ex)
            {
                _log.Error(string.Format(Properties.Resources.ErrFailedToLoadFile, filePath), ex);
            }
        }


        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            var dataObject = e.Data;
            if (dataObject == null) return;

            if (!dataObject.GetDataPresent(DataFormats.FileDrop)) return;

            var filePaths = dataObject.GetData(DataFormats.FileDrop) as string[];
            if (filePaths == null || filePaths.Length <= 0) return;

            var filePath = filePaths[0];
            switch (Path.GetExtension(filePath))
            {
                case ".nsp":
                    SafeLoadFile(filePath);
                    break;
            }
        }

        private void OnLog(object sender, LogEventHandlerArgs args)
        {
            if (!Dispatcher.CheckAccess()) // To prevent UI thread InvalidOperationException when log event comes from another thread
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    OnLog(sender, args);
                }));
                return;
            }

            var logEvent = args.LogEvent;

            // TODO: check if inner required
            //var exTemp = args.Exception;
            //while (exTemp != null)
            //{
            //    logEvent += " ==> " + exTemp.Message;
            //    exTemp = exTemp.InnerException;
            //}

            var tr = new TextRange(RichTextBoxLog.Document.ContentEnd, RichTextBoxLog.Document.ContentEnd)
            {
                Text = logEvent.RenderedMessage + Environment.NewLine
            };

            var color = Brushes.Black;
            if (logEvent.Level >= Level.Error)
                color = Brushes.Red;
            else if (logEvent.Level >= Level.Warn)
                color = Brushes.OrangeRed;
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