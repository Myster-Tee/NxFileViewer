using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Services.Nand;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Win32;
using System.ComponentModel;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Models;
using Emignatik.NxFileViewer.Services.FileOpening;

namespace Emignatik.NxFileViewer.Views.Windows;

public sealed class NandViewModel : NotifyPropertyChangedBase
{
    private readonly NandCliPlugin _plugin;
    private readonly IFileOpeningService? _opening;
    private string _physicalSource = "";
    private string _source = "";
    private string _information = "";
    private string? _partition;
    private IReadOnlyList<string> _partitions = Array.Empty<string>();
    public NandViewModel(NandCliPlugin plugin, IMainBackgroundTaskRunnerService runner, IFileOpeningService? opening = null)
    {
        _plugin = plugin;
        _opening = opening;
        Runner = runner;
        OpenCommand = new RelayCommand(Open, () => !Runner.IsRunning);
        ExportCommand = new RelayCommand(Export, () => !Runner.IsRunning && !string.IsNullOrEmpty(SelectedPartition) && File.Exists(_physicalSource));
        ReadInformationCommand = new RelayCommand(ReadInformation, () => !Runner.IsRunning && File.Exists(_physicalSource));
        PropertyChangedEventManager.AddHandler(Runner, OnTaskChanged, "");
    }
    private void OnTaskChanged(object? sender, PropertyChangedEventArgs e)
    {
        OpenCommand.TriggerCanExecuteChanged(true); ExportCommand.TriggerCanExecuteChanged(true); ReadInformationCommand.TriggerCanExecuteChanged(true);
    }
    public bool IsStandalone { get; private set; } = true;
    public RelayCommand ReadInformationCommand { get; }
    public void SetFile(NxFile file)
    {
        IsStandalone = false;
        NotifyPropertyChanged(nameof(IsStandalone));
        Source = file.FilePath;
        _physicalSource = file.NandPhysicalPath ?? "";
        Information = file.NandResult?.Information ?? "";
        Partitions = file.NandResult?.NameOnly == false ? file.NandResult.Partitions : Array.Empty<string>();
        SelectedPartition = Partitions.Count > 0 ? Partitions[0] : null;
        ReadInformationCommand.TriggerCanExecuteChanged();
    }
    public IMainBackgroundTaskRunnerService Runner { get; }
    public RelayCommand OpenCommand { get; }
    public RelayCommand ExportCommand { get; }
    public string Source { get => _source; private set { _source = value; NotifyPropertyChanged(); } }
    public string Information { get => _information; private set { _information = value; NotifyPropertyChanged(); } }
    public IReadOnlyList<string> Partitions { get => _partitions; private set { _partitions = value; NotifyPropertyChanged(); } }
    public string? SelectedPartition { get => _partition; set { _partition = value; NotifyPropertyChanged(); ExportCommand.TriggerCanExecuteChanged(); } }

    private async void Open()
    {
        var keys = LocalizationManager.Instance.Current.Keys;
        var dialog = new OpenFileDialog { Title = keys.Nand_Open, Filter = "NAND / ZIP / 7z (*.*)|*.*", CheckFileExists = true };
        if (dialog.ShowDialog() != true) return;
        if (PackageZip.IsArchive(dialog.FileName) && _opening != null)
        {
            await _opening.SafeOpenFile(dialog.FileName);
            return;
        }
        Source = "";
        _physicalSource = "";
        Information = "";
        Partitions = Array.Empty<string>();
        SelectedPartition = null;
        try
        {
            var information = await Runner.RunAsync(new RunnableRelay<string>((progress, token) => _plugin.ReadInformation(dialog.FileName, progress, token)) { SupportsCancellation = true });
            Source = dialog.FileName;
            _physicalSource = dialog.FileName;
            Information = information;
            Partitions = NandCliPlugin.FindPartitions(information);
            SelectedPartition = Partitions.Count > 0 ? Partitions[0] : null;
            ReadInformationCommand.TriggerCanExecuteChanged();
        }
        catch (OperationCanceledException) { Information = keys.Nand_Cancelled; }
        catch (Exception ex) { ThemedDialog.Notice(ex.Message, "NxNandManager", MessageBoxImage.Error); }
    }

    private async void ReadInformation()
    {
        try
        {
            Information = await Runner.RunAsync(new RunnableRelay<string>((progress, token) => _plugin.ReadInformation(_physicalSource, progress, token)) { SupportsCancellation = true });
            Partitions = NandCliPlugin.FindPartitions(Information);
            SelectedPartition = Partitions.Count > 0 ? Partitions[0] : null;
        }
        catch (OperationCanceledException) { ThemedDialog.Notice(LocalizationManager.Instance.Current.Keys.Nand_Cancelled, "NxNandManager", MessageBoxImage.Information); }
        catch (Exception ex) { ThemedDialog.Notice(ex.Message, "NxNandManager", MessageBoxImage.Error); }
    }

    private async void Export()
    {
        if (SelectedPartition == null) return;
        var keys = LocalizationManager.Instance.Current.Keys;
        var source = _physicalSource;
        var partition = SelectedPartition;
        var dialog = new SaveFileDialog { Title = keys.Nand_Export, FileName = partition + ".bin", Filter = "NAND (*.bin)|*.bin", OverwritePrompt = true };
        if (dialog.ShowDialog() != true) return;
        try
        {
            await Runner.RunAsync(new RunnableRelay((progress, token) => _plugin.Export(source, dialog.FileName, partition, progress, token)) { SupportsCancellation = true });
            ThemedDialog.Notice(keys.Nand_ExportDone + Environment.NewLine + dialog.FileName, "NxNandManager", MessageBoxImage.Information);
        }
        catch (OperationCanceledException) { ThemedDialog.Notice(keys.Nand_Cancelled, "NxNandManager", MessageBoxImage.Information); }
        catch (Exception ex) { ThemedDialog.Notice(ex.Message, "NxNandManager", MessageBoxImage.Error); }
    }
}
