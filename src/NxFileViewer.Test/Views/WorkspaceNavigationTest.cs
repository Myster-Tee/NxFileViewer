using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Emignatik.NxFileViewer.Test.Views;

[CollectionDefinition("Workspace UI", DisableParallelization = true)]
public sealed class WorkspaceUiCollection { }

[Collection("Workspace UI")]
public sealed class WorkspaceNavigationTest
{
    [Fact]
    public void NavigationKeepsPagesAndDraftsAndSettingsButtonsDoNotCloseMainWindow()
    {
        Exception? error = null;
        var thread = new Thread(() =>
        {
            try
            {
                var app = new App { ShutdownMode = ShutdownMode.OnExplicitShutdown };
                app.InitializeComponent();
                var main = new MainWindow();
                app.MainWindow = main;
                var closed = false;
                main.Closed += (_, _) => closed = true;
                var tabs = (TabControl)main.FindName("WorkspaceTabs");
                App.ServiceProvider.GetRequiredService<IShowBatchIntegrityWindowCommand>().Execute(null);
                var batchTab = (TabItem)main.FindName("BatchTab");
                var batchPage = Assert.IsType<BatchIntegrityWindow>(batchTab.Content);
                var batch = Assert.IsType<BatchIntegrityWindowViewModel>(batchPage.DataContext);
                batch.InputDirectory = "retained batch directory";
                App.ServiceProvider.GetRequiredService<IShowSettingsWindowCommand>().Execute(null);
                var settingsPage = Assert.IsType<SettingsWindow>(((TabItem)main.FindName("SettingsTab")).Content);
                var settings = Assert.IsType<SettingsWindowViewModel>(settingsPage.DataContext);
                Assert.Equal(4, ((TabControl)settingsPage.FindName("SettingsSections")).Items.Count);
                main.NavigateNamingSettings();
                var sections = (TabControl)settingsPage.FindName("SettingsSections");
                Assert.Same(settingsPage.FindName("NamingSettingsTab"), sections.SelectedItem);
                Assert.Equal(1, sections.SelectedIndex);
                var originalPattern = settings.EditedSettings.RenamingOptions.ApplicationPattern;
                settings.EditedSettings.RenamingOptions.ApplicationPattern = "GAME/{Title}.{Ext}";
                main.Navigate(WorkspaceSection.Rename);
                main.NavigateNamingSettings();
                Assert.Equal("GAME/{Title}.{Ext}", settings.EditedSettings.RenamingOptions.ApplicationPattern);
                settings.CancelSettingsCommand.Execute(null);
                Assert.Equal(originalPattern, settings.EditedSettings.RenamingOptions.ApplicationPattern);
                main.Navigate(WorkspaceSection.Settings);
                Assert.IsType<UpdateCenterView>(((ContentControl)settingsPage.FindName("SettingsUpdatesContent")).Content);
                Assert.IsType<PluginSettingsView>(((ContentControl)settingsPage.FindName("SettingsPluginsContent")).Content);
                var originalLevel = settings.EditedSettings.NszCompressionLevel;
                settings.EditedSettings.NszCompressionLevel = 7;
                main.Navigate(WorkspaceSection.Batch);
                Assert.Same(batchPage, batchTab.Content);
                Assert.Equal("retained batch directory", batch.InputDirectory);
                main.Navigate(WorkspaceSection.Settings);
                Assert.Same(settingsPage, ((TabItem)main.FindName("SettingsTab")).Content);
                Assert.Equal(7, settings.EditedSettings.NszCompressionLevel);
                settings.CancelSettingsCommand.Execute(null);
                Assert.False(closed);
                Assert.Equal((int)WorkspaceSection.Home, tabs.SelectedIndex);
                Assert.Equal(originalLevel, settings.EditedSettings.NszCompressionLevel);
                main.Navigate(WorkspaceSection.Settings);
                settings.ApplySettingsCommand.Execute(null);
                Assert.False(closed);
                var actual = App.ServiceProvider.GetRequiredService<IAppSettings>();
                var originalTitleUrl = actual.TitlePageUrl;
                settings.EditedSettings.TitlePageUrl = "https://example.com/general-draft";
                main.Navigate(WorkspaceSection.Plugins);
                var pluginPage = Assert.IsType<PluginSettingsView>(((TabItem)main.FindName("PluginsTab")).Content);
                var plugins = Assert.IsType<SettingsWindowViewModel>(pluginPage.DataContext);
                Assert.NotSame(settings, plugins);
                Assert.True(plugins.PluginSettingsOnly);
                plugins.EditedSettings.NszCompressionLevel = 9;
                var originalNandExecutable = actual.NandExecutablePath;
                var originalBisKeys = actual.NandBisKeysPath;
                plugins.EditedSettings.NandExecutablePath = "C:\\Plugins\\NxNandManager.exe";
                plugins.EditedSettings.NandBisKeysPath = "C:\\Keys\\bis.keys";
                plugins.ApplySettingsCommand.Execute(null);
                Assert.Equal("C:\\Plugins\\NxNandManager.exe", actual.NandExecutablePath);
                Assert.Equal("C:\\Keys\\bis.keys", actual.NandBisKeysPath);
                Assert.Equal(9, actual.NszCompressionLevel);
                Assert.Equal(originalTitleUrl, actual.TitlePageUrl);
                Assert.Equal("https://example.com/general-draft", settings.EditedSettings.TitlePageUrl);
                main.Navigate(WorkspaceSection.Settings);
                settings.ApplySettingsCommand.Execute(null);
                Assert.Equal("C:\\Plugins\\NxNandManager.exe", actual.NandExecutablePath);
                main.Navigate(WorkspaceSection.Nand);
                var nandPage = Assert.IsType<NandView>(((TabItem)main.FindName("NandTab")).Content);
                Assert.IsType<NandViewModel>(nandPage.DataContext);
                main.Navigate(WorkspaceSection.Home);
                main.Navigate(WorkspaceSection.Nand);
                Assert.Same(nandPage, ((TabItem)main.FindName("NandTab")).Content);
                var nandRoot = new Emignatik.NxFileViewer.Models.TreeItems.Impl.NandFileItem("synthetic.bin");
                using var nandFile = new Emignatik.NxFileViewer.Models.NxFile("synthetic.bin", nandRoot,
                    new Emignatik.NxFileViewer.Models.Overview.FileOverview(nandRoot))
                {
                    NandPhysicalPath = "synthetic.bin",
                    NandResult = new Emignatik.NxFileViewer.Services.Nand.NandDetectionResult("RAWNAND", new[] { "SYSTEM" }, false, 512, 1)
                };
                var openedNand = new Emignatik.NxFileViewer.Views.UserControls.OpenedFileViewModel(nandFile, App.ServiceProvider);
                Assert.True(openedNand.IsNand);
                Assert.Equal(3, openedNand.InitialDetailsTabIndex);
                Assert.False(openedNand.Nand!.IsStandalone);
                Assert.Equal("SYSTEM", openedNand.Nand.SelectedPartition);
                Assert.False(batch.CanActOnFile(new Emignatik.NxFileViewer.Services.Integrity.BatchIntegrityResult(
                    "synthetic.bin", "NAND", "NAND", "RAWNAND", "None", Emignatik.NxFileViewer.Models.Overview.NcasIntegrity.Unchecked, null) { IsNand = true }));
                var openedNandPage = new Emignatik.NxFileViewer.Views.UserControls.OpenedFileView { DataContext = openedNand };
                var nandDetails = (TabControl)openedNandPage.Content;
                Assert.IsType<NandView>(((TabItem)nandDetails.Items[3]).Content);
                Assert.Equal(9, actual.NszCompressionLevel);
                Assert.Equal("https://example.com/general-draft", actual.TitlePageUrl);
                main.Navigate(WorkspaceSection.Plugins);
                Assert.Same(pluginPage, ((TabItem)main.FindName("PluginsTab")).Content);
                plugins.EditedSettings.NszCompressionLevel = 12;
                plugins.CancelSettingsCommand.Execute(null);
                Assert.Equal(9, actual.NszCompressionLevel);
                Assert.Equal(9, plugins.EditedSettings.NszCompressionLevel);
                Assert.False(closed);
                actual.NszCompressionLevel = originalLevel;
                actual.NandExecutablePath = originalNandExecutable;
                actual.NandBisKeysPath = originalBisKeys;
                actual.TitlePageUrl = originalTitleUrl;
                App.ServiceProvider.GetRequiredService<IShowRenameToolWindowCommand>().Execute(null);
                Assert.IsType<RenameToolWindow>(((TabItem)main.FindName("RenameTab")).Content);
                main.Navigate(WorkspaceSection.Updates);
                var updates = Assert.IsType<UpdateCenterView>(((TabItem)main.FindName("UpdatesTab")).Content);
                Assert.IsType<UpdateCenterViewModel>(updates.DataContext);
                main.Navigate(WorkspaceSection.Home);
                main.Navigate(WorkspaceSection.Updates);
                Assert.Same(updates, ((TabItem)main.FindName("UpdatesTab")).Content);
                // A completed NSZ preview stays available even without its source file.
                var showPreview = typeof(BatchIntegrityWindowViewModel).GetMethod("ShowPreview",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
                var showCompleted = typeof(BatchIntegrityWindowViewModel).GetMethod("ShowCompletedResult",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
                var nszOverview = new Emignatik.NxFileViewer.Models.Overview.FileOverview(new PreviewItem());
                var nszPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid() + ".nsz");
                var nszFile = new Emignatik.NxFileViewer.Models.NxFile(nszPath, nszOverview.RootItem, nszOverview);
                showPreview.Invoke(batch, new object[] { nszFile });
                nszOverview.NcasIntegrity = Emignatik.NxFileViewer.Models.Overview.NcasIntegrity.Original;
                var nszResult = new Emignatik.NxFileViewer.Services.Integrity.BatchIntegrityResult(nszPath,
                    "NSZ", "NSZ", "Cdn", "Blockless", nszOverview.NcasIntegrity, null);
                showCompleted.Invoke(batch, new object[] { nszResult });
                batch.SelectedResult = nszResult;
                var capturedNsz = Assert.IsType<Emignatik.NxFileViewer.Views.UserControls.FileOverviewViewModel>(batch.PreviewOverview);
                Assert.Equal(nszOverview.NcasIntegrity, capturedNsz.NcasIntegrity);
                var nspOverview = new Emignatik.NxFileViewer.Models.Overview.FileOverview(new PreviewItem());
                var nspPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid() + ".nsp");
                batch.SelectedResult = nszResult with { IsFirmware = true };
                Assert.Equal(1, batch.SelectedDetailsTabIndex);
                showPreview.Invoke(batch, new object[] {
                    new Emignatik.NxFileViewer.Models.NxFile(nspPath, nspOverview.RootItem, nspOverview) });
                Assert.Equal(0, batch.SelectedDetailsTabIndex);
                Assert.Null(batch.SelectedResult);
                Assert.False(batch.ShowStoredOverview);
                Assert.NotSame(capturedNsz, batch.PreviewOverview);
                Assert.Equal(nspOverview.NcasIntegrity, batch.PreviewOverview!.NcasIntegrity);
                var nspResult = nszResult with { FilePath = nspPath, FileType = "NSP", Compression = "None" };
                showCompleted.Invoke(batch, new object[] { nspResult });
                Assert.Equal(nspPath, batch.SelectedResult!.FilePath);
                batch.SelectedResult = nspResult;
                Assert.NotNull(batch.PreviewOverview);
                batch.SelectedResult = nszResult;
                Assert.Same(capturedNsz, batch.PreviewOverview);
                var uncachedResult = nszResult with { FilePath = nszPath + ".missing.nsz" };
                batch.Results.Add(uncachedResult);
                batch.SelectedResult = uncachedResult;
                // Per-file conversion must work directly after scanning, but reject missing keys and incompatible types.
                System.IO.File.WriteAllBytes(nspPath, new byte[] { 0 });
                try
                {
                    var uncheckedNsp = nspResult with { Integrity = Emignatik.NxFileViewer.Models.Overview.NcasIntegrity.Unchecked };
                    Assert.True(batch.CanActOnFile(uncheckedNsp, Emignatik.NxFileViewer.Services.Nsz.NszOperation.Compress));
                    Assert.False(batch.CanActOnFile(uncheckedNsp, Emignatik.NxFileViewer.Services.Nsz.NszOperation.Decompress));
                    Assert.False(batch.CanActOnFile(uncheckedNsp with { HasMissingKeys = true }, Emignatik.NxFileViewer.Services.Nsz.NszOperation.Compress));
                    Assert.False(batch.CanActOnFile(uncheckedNsp with { IsFirmware = true }, Emignatik.NxFileViewer.Services.Nsz.NszOperation.Compress));
                    Assert.True(batch.CanOpenInSingle(uncheckedNsp));
                }
                finally { System.IO.File.Delete(nspPath); }
                Assert.Null(batch.PreviewOverview);
                var detailsTabs = (TabControl)batchPage.FindName("DetailsTabs");
                var detailsBinding = detailsTabs.GetBindingExpression(TabControl.SelectedIndexProperty)!.ParentBinding;
                Assert.Equal(nameof(BatchIntegrityWindowViewModel.SelectedDetailsTabIndex), detailsBinding.Path.Path);
                Assert.Equal(System.Windows.Data.BindingMode.TwoWay, detailsBinding.Mode);
                var firmwareResult = nszResult with { FilePath = "firmware.zip", FileType = "ZIP", IsFirmware = true };
                batch.SelectedResult = firmwareResult;
                Assert.Equal(1, batch.SelectedDetailsTabIndex);
                foreach (var type in new[] { "NSP", "NSZ", "XCI", "XCZ" })
                {
                    batch.SelectedDetailsTabIndex = 1;
                    batch.SelectedResult = nszResult with { FileType = type };
                    Assert.Equal(0, batch.SelectedDetailsTabIndex);
                    batch.SelectedResult = firmwareResult;
                    Assert.Equal(1, batch.SelectedDetailsTabIndex);
                }
                batch.SelectedDetailsTabIndex = 0;
                Assert.Equal(0, batch.SelectedDetailsTabIndex);
                batch.SelectedResult = nszResult;
                batch.SelectedResult = firmwareResult;
                Assert.Equal(1, batch.SelectedDetailsTabIndex);
                Assert.Single(app.Windows);
                var firmwareRoot = new Emignatik.NxFileViewer.Models.TreeItems.Impl.FirmwareFileItem("firmware.7z");
                using (var firmwareFile = new Emignatik.NxFileViewer.Models.NxFile("firmware.7z", firmwareRoot,
                    new Emignatik.NxFileViewer.Models.Overview.FileOverview(firmwareRoot))
                    { FirmwareResult = firmwareResult with { Structure = "2.0.0", FirmwareDetails = "synthetic details" } })
                {
                    var firmwareViewModel = new Emignatik.NxFileViewer.Views.UserControls.OpenedFileViewModel(firmwareFile, App.ServiceProvider);
                    Assert.True(firmwareViewModel.HasFirmwareInfo);
                    Assert.True(firmwareViewModel.IsFirmwareOnly);
                    Assert.Equal(2, firmwareViewModel.InitialDetailsTabIndex);
                    Assert.Equal("2.0.0", firmwareViewModel.FirmwareVersion);
                    Assert.Equal("synthetic details", firmwareViewModel.FirmwareDetails);
                }
                var grid = (DataGrid)batchPage.FindName("ResultsGrid");
                Assert.True(grid.CanUserSortColumns);
                Assert.Equal(ScrollBarVisibility.Auto, ScrollViewer.GetHorizontalScrollBarVisibility(grid));
                Assert.All(grid.Columns, column => Assert.False(column.Width.IsStar));
                grid.Width = 500;
                grid.Height = 200;
                grid.ApplyTemplate();
                grid.Measure(new Size(500, 200));
                grid.Arrange(new Rect(0, 0, 500, 200));
                grid.UpdateLayout();
                var findScroll = typeof(BatchIntegrityWindow).GetMethod("FindScrollViewer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;
                var scroll = Assert.IsType<ScrollViewer>(findScroll.Invoke(null, new object[] { grid }));
                Assert.True(scroll.ScrollableWidth > 0);
                Assert.Contains(grid.Columns, column => column.SortMemberPath == "TitleId" && column.Visibility == Visibility.Collapsed);
                var titleColumn = System.Linq.Enumerable.First(grid.Columns, column => column.SortMemberPath == "Title");
                titleColumn.Visibility = Visibility.Visible;
                main.Navigate(WorkspaceSection.Home);
                main.Navigate(WorkspaceSection.Batch);
                Assert.Equal(Visibility.Visible, titleColumn.Visibility);
                var makeMenu = typeof(BatchIntegrityWindow).GetMethod("CreateColumnsMenu", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
                var menu = (ContextMenu)makeMenu.Invoke(batchPage, new object[] { new Button() })!;
                Assert.Equal(grid.Columns.Count, menu.Items.Count);
                var titleMenu = (MenuItem)menu.Items[grid.Columns.IndexOf(titleColumn)];
                titleMenu.IsChecked = false;
                titleMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                Assert.Equal(Visibility.Collapsed, titleColumn.Visibility);
                titleMenu.IsChecked = true;
                titleMenu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                Assert.Equal(Visibility.Visible, titleColumn.Visibility);
                batch.Results.Clear();
                batch.Results.Add(nszResult with { Title = "Zulu", FileSize = 20 });
                batch.Results.Add(nspResult with { Title = "Alpha", FileSize = 3 });
                batch.ResultsView.SortDescriptions.Add(new System.ComponentModel.SortDescription("FileSize", System.ComponentModel.ListSortDirection.Ascending));
                Assert.Equal("Alpha", System.Linq.Enumerable.First(System.Linq.Enumerable.Cast<Emignatik.NxFileViewer.Services.Integrity.BatchIntegrityResult>(batch.ResultsView)).Title);
                batch.SearchText = "zulu";
                Assert.Single(System.Linq.Enumerable.Cast<object>(batch.ResultsView));
                batch.FileTypeFilter = "NSP";
                Assert.Empty(System.Linq.Enumerable.Cast<object>(batch.ResultsView));
                batch.ResetFiltersCommand.Execute(null);
                Assert.Equal(2, System.Linq.Enumerable.Count(System.Linq.Enumerable.Cast<object>(batch.ResultsView)));
                var emptyOverview = new Emignatik.NxFileViewer.Views.UserControls.FileOverviewView { DataContext = null };
                var keyWarning = Assert.IsType<Border>(((Grid)emptyOverview.Content).Children[0]);
                Assert.Equal(Visibility.Collapsed, keyWarning.Visibility);
                var languages = Emignatik.NxFileViewer.Localization.LocalizationManager.Instance;
                var originalLanguage = languages.Current;
                try
                {
                    languages.Current = System.Linq.Enumerable.First(languages.RealLocalizations, l => l.CultureName.StartsWith("de"));
                    var runner = new Emignatik.NxFileViewer.Services.BackgroundTask.BackgroundTaskRunner();
                    var statusChanged = false;
                    runner.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(runner.ProgressText)) statusChanged = true; };
                    var germanReady = runner.ProgressText;
                    languages.Current = System.Linq.Enumerable.First(languages.RealLocalizations, l => l.CultureName.StartsWith("en"));
                    Assert.True(statusChanged);
                    Assert.Equal(languages.Current.Keys.Status_Ready, runner.ProgressText);
                    Assert.NotEqual(germanReady, runner.ProgressText);
                }
                finally { languages.Current = originalLanguage; }
                var historyEnabled = actual.EnableBatchHistory;
                try
                {
                    actual.EnableBatchHistory = false;
                    Assert.False(batch.IsHistoryEnabled);
                    Assert.Empty(batch.History);
                    Assert.False(batch.ResumeHistoryCommand.CanExecute(null));
                    Assert.False(batch.LoadHistoryCommand.CanExecute(null));
                    actual.EnableBatchHistory = true;
                    Assert.True(batch.IsHistoryEnabled);
                    // Restoring a history has no live preview cache. Display persisted
                    // game metadata even when the source/archive no longer exists.
                    foreach (var format in new[] { "NSP", "NSZ", "XCI", "XCZ", "NSP (ZIP)", "NSZ (7Z)" })
                    {
                        var savedGame = nszResult with { FilePath = nszPath + ".missing", FileType = format,
                            Title = "Saved game", TitleId = "0100123456789000", Publisher = "Saved publisher", FileSize = 1024 };
                        batch.SelectedHistory = new Emignatik.NxFileViewer.Services.Integrity.BatchHistoryEntry(Guid.NewGuid(), DateTime.UtcNow,
                            "missing history source", false, true, true, false, "Completed", new[] { savedGame }, new());
                        batch.LoadHistoryCommand.Execute(null);
                        Assert.Same(savedGame, batch.SelectedResult);
                        Assert.Null(batch.PreviewOverview);
                        Assert.True(batch.ShowStoredOverview);
                        Assert.Equal(0, batch.SelectedDetailsTabIndex);
                        Assert.Contains(batch.StoredOverviewFields, field => field.Value == "Saved game");
                        Assert.Contains(batch.StoredOverviewFields, field => field.Value == "0100123456789000");
                        batch.SelectedResult = savedGame with { IsNand = true };
                        Assert.False(batch.ShowStoredOverview);
                        batch.SelectedResult = savedGame with { IsFirmware = true };
                        Assert.False(batch.ShowStoredOverview);
                    }
                }
                finally { actual.EnableBatchHistory = historyEnabled; }
                main.Close();
            }
            catch (Exception ex) { error = ex; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(30)), "Workspace test timed out.");
        if (error != null) throw new InvalidOperationException("Workspace UI test failed.", error);
    }
    private sealed class PreviewItem : Emignatik.NxFileViewer.Models.TreeItems.ItemBase
    {
        public PreviewItem() : base(null) { }
        public override string Name => "preview";
        public override string DisplayName => Name;
        public override string Format => "NSZ";
        public override string LibHacTypeName => "preview";
    }
}
