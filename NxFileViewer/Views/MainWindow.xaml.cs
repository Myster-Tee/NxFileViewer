using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Emignatik.NxFileViewer.Logging;
using Emignatik.NxFileViewer.NSP;
using Emignatik.NxFileViewer.Properties;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Views.NSP;
using Microsoft.Win32;

namespace Emignatik.NxFileViewer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;

            Logger.Log += OnLog;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                KeySetProviderService.GetKeySet();
                Logger.LogInfo(Properties.Resources.InfoKeysSuccessfullyLoaded);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.Message);
            }
        }

        private void PromptOpenFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                FileName = Settings.Default.LastOpenedFile,
                Filter = Properties.Resources.OpenFile_Filter
            };

            if (openFileDialog.ShowDialog(Application.Current.MainWindow) != true) return;

            var filePath = openFileDialog.FileName;

            SafeLoadFile(filePath);
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
                Logger.LogInfo($"===> {Path.GetFileName(filePath)} <===");

                var nspInfoLoader = new NspInfoLoader(new TempDirMgr(Settings.Default.WorkDir), KeySetProviderService.GetKeySet());
                var nspInfo = nspInfoLoader.Load(filePath);

                //TODO: split "code behing logic" to separate view model
                ((MainWindowViewModel)this.DataContext).FileViewModel = new NspInfoViewModel(nspInfo)
                {
                    Source = filePath,
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format(Properties.Resources.ErrFailedToLoadFile, filePath), ex);
            }
        }

        private void OnOpenFileCommand(object sender, ExecutedRoutedEventArgs e)
        {
            PromptOpenFile();
        }

        private void OnOpenLastFileCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var lastOpenedFile = Settings.Default.LastOpenedFile;
            if (lastOpenedFile != null && File.Exists(lastOpenedFile))
            {
                SafeLoadFile(lastOpenedFile);
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

            var fullMessage = args.Message;

            var exTemp = args.Exception;
            while (exTemp != null)
            {
                fullMessage += " ==> " + exTemp.Message;
                exTemp = exTemp.InnerException;
            }

            var tr = new TextRange(RichTextBoxLog.Document.ContentEnd, RichTextBoxLog.Document.ContentEnd)
            {
                Text = fullMessage + Environment.NewLine
            };

            var color = Brushes.Black;
            switch (args.LogLevel)
            {
                case LogLevel.INFO:
                    color = Brushes.Blue;
                    break;
                case LogLevel.WARNING:
                    color = Brushes.OrangeRed;
                    break;
                case LogLevel.ERROR:
                    color = Brushes.Red;
                    break;
            }

            tr.ApplyPropertyValue(TextElement.ForegroundProperty, color);
        }

        private void MenuItemClearLogClick(object sender, RoutedEventArgs e)
        {
            RichTextBoxLog.Document.Blocks.Clear();
        }
    }
}