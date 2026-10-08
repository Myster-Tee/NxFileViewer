using System;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Models;
using Emignatik.NxFileViewer.Utils.MVVM;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views.UserControls;

public class OpenedFileViewModel : ViewModelBase
{
    private readonly NxFile _nxFile;

    public OpenedFileViewModel(NxFile nxFile, IServiceProvider serviceProvider)
    {
        serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _nxFile = nxFile ?? throw new ArgumentNullException(nameof(nxFile));
        if (nxFile.NandResult != null)
        {
            Nand = new Windows.NandViewModel(serviceProvider.GetRequiredService<Services.Nand.NandCliPlugin>(),
                serviceProvider.GetRequiredService<Services.BackgroundTask.IMainBackgroundTaskRunnerService>());
            Nand.SetFile(nxFile);
        }
        Content = new ContentViewModel(_nxFile.RootItem, serviceProvider);
        FileOverview = new FileOverviewViewModel(_nxFile.Overview, serviceProvider);

        OpenFileLocationCommand = serviceProvider.GetRequiredService<IOpenFileLocationCommand>();
        OpenFileLocationCommand.FilePath = _nxFile.ArchivePath ?? _nxFile.FilePath;
    }

    public FileOverviewViewModel FileOverview { get; }

    public ContentViewModel Content { get; }

    public string FilePath => _nxFile.FilePath;
    public Windows.NandViewModel? Nand { get; }
    public bool IsNand => _nxFile.NandResult != null;
    public bool IsSpecialFile => IsFirmwareOnly || IsNand;
    public bool HasFirmwareInfo => _nxFile.FirmwareResult != null;
    public bool IsFirmwareOnly => _nxFile.RootItem is Models.TreeItems.Impl.FirmwareFileItem;
    public int InitialDetailsTabIndex => IsNand ? 3 : _nxFile.FirmwareResult?.IsFirmware == true ? 2 : 0;
    public string FirmwareVersion => _nxFile.FirmwareResult?.Structure ?? "";
    public string FirmwareDetails => _nxFile.FirmwareResult?.FirmwareDetails ?? "";

    public IOpenFileLocationCommand OpenFileLocationCommand { get; }
}
