using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using LibHac.Ncm;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Commands;

public class OpenTitleWebPageCommand : CommandBase, IOpenTitleWebPageCommand
{
    private readonly IOpenedFileService _openedFileService;
    private readonly IAppSettings _appSettings;
    private readonly ILogger _logger;

    private CnmtItem? _cnmtItem;

    public OpenTitleWebPageCommand(IOpenedFileService openedFileService, IAppSettings appSettings, ILoggerFactory loggerFactory)
    {
        _openedFileService = openedFileService ?? throw new ArgumentNullException(nameof(openedFileService));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());

        _openedFileService.OpenedFileChanged += (_, _) => // (_, _) == An ass ?
        {
            UpdateCnmtItem();
        };
        UpdateCnmtItem();
    }

    private CnmtItem? CnmtItem
    {
        get => _cnmtItem;
        set
        {
            _cnmtItem = value;
            TriggerCanExecuteChanged();
        }
    }

    public override void Execute(object? parameter)
    {
        var cnmtItem = CnmtItem;
        if (cnmtItem == null)
            return;

        try
        {
            var url = _appSettings.TitlePageUrl.SafeFormat(Uri.EscapeDataString(cnmtItem.ApplicationTitleId));

            var processStartInfo = new ProcessStartInfo(url)
            {
                CreateNoWindow = true,
                UseShellExecute = true
            };
            Process.Start(processStartInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(LocalizationManager.Instance.Current.Keys.OpenTitleWebPage_Failed.SafeFormat(ex.Message));
        }
    }

    private void UpdateCnmtItem()
    {
        CnmtItem = GetCnmtItem(_openedFileService.OpenedFile);
    }

    private static CnmtItem? GetCnmtItem(NxFile? nxFile)
    {
        if (nxFile == null)
            return null;

        var cnmtItems = nxFile.Overview.CnmtContainers.Select(container => container.CnmtItem).ToArray();
        var appCnmtItem = cnmtItems.FirstOrDefault(cnmtItem => cnmtItem.ContentType == ContentMetaType.Application);
        if (appCnmtItem != null)
            return appCnmtItem;

        return cnmtItems.FirstOrDefault();
    }

    public override bool CanExecute(object? parameter)
    {
        return _cnmtItem != null;
    }
}

public interface IOpenTitleWebPageCommand : ICommand
{
}