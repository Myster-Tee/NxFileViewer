using System;
using System.Windows.Input;
using Emignatik.NxFileViewer.Services.KeysManagement;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Commands;

public class LoadKeysCommand : CommandBase, ILoadKeysCommand
{

    private readonly IKeySetProviderService _keySetProviderService;
    private readonly ILogger _logger;

    public LoadKeysCommand(IKeySetProviderService keySetProviderService, ILoggerFactory loggerFactory)
    {
        _keySetProviderService = keySetProviderService ?? throw new ArgumentNullException(nameof(keySetProviderService));
        _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
    }

    public override void Execute(object? parameter)
    {
        try
        {
            _keySetProviderService.GetKeySet(forceReload: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

}

public interface ILoadKeysCommand : ICommand
{
}