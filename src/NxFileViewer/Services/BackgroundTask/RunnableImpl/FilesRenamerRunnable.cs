using System;
using System.Threading;
using Emignatik.NxFileViewer.Services.FileRenaming;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;


public class FileRenamerRunnable : IFileRenamerRunnable
{

    private readonly IFileRenamerService _fileRenamerService;
    private RenameSettings? _renameSettings;

    public bool SupportsCancellation => true;

    public bool SupportProgress => false;

    public FileRenamerRunnable(IFileRenamerService fileRenamerService)
    {
        _fileRenamerService = fileRenamerService ?? throw new ArgumentNullException(nameof(fileRenamerService));
    }

    public void Run(IProgressReporter progressReporter, CancellationToken cancellationToken)
    {
        if (_renameSettings == null)
            throw new InvalidOperationException($"{nameof(Setup)} method should be invoked first.");

        _fileRenamerService.RenameFileAsync(
            _renameSettings.InputPath,
            _renameSettings.NamingSettings,
            _renameSettings.Simulation,
            _renameSettings.Logger,
            cancellationToken
        ).Wait(cancellationToken);
    }

    public IFileRenamerRunnable Setup(string inputDirectory, INamingSettings namingSettings, bool simulation, ILogger? logger)
    {
        if (inputDirectory == null) throw new ArgumentNullException(nameof(inputDirectory));
        if (namingSettings == null) throw new ArgumentNullException(nameof(namingSettings));

        _renameSettings = new RenameSettings
        {
            NamingSettings = namingSettings,
            InputPath = inputDirectory,
            Simulation = simulation,
            Logger = logger,
        };

        return this;
    }

    private class RenameSettings
    {
        public INamingSettings NamingSettings { get; init; } = null!;
        public string InputPath { get; init; } = null!;
        public bool Simulation { get; init; }
        public ILogger? Logger { get; init; }
    }
}

public interface IFileRenamerRunnable : IRunnable
{
    IFileRenamerRunnable Setup(string inputFile, INamingSettings namingSettings, bool simulation, ILogger? logger);
}

