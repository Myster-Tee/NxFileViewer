using System;
using System.Threading;
using Emignatik.NxFileViewer.Services.FileRenaming;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;


public class FilesRenamerRunnable : IFilesRenamerRunnable
{

    private readonly IFileRenamerService _fileRenamerService;
    private RenameSettings? _renameSettings;

    public bool SupportsCancellation => true;
    public bool SupportProgress => true;

    public FilesRenamerRunnable(IFileRenamerService fileRenamerService)
    {
        _fileRenamerService = fileRenamerService ?? throw new ArgumentNullException(nameof(fileRenamerService));
    }

    public void Run(IProgressReporter progressReporter, CancellationToken cancellationToken)
    {
        if (_renameSettings == null)
            throw new InvalidOperationException($"{nameof(Setup)} method should be invoked first.");

        _fileRenamerService.RenameFromDirectoryAsync(
            _renameSettings.InputDirectory,
            _renameSettings.FileFilters,
            _renameSettings.IncludeSubdirectories,
            _renameSettings.NamingSettings,
            _renameSettings.Simulation,
            _renameSettings.Logger,
            progressReporter,
            cancellationToken
        ).Wait(cancellationToken);
    }

    public IFilesRenamerRunnable Setup(string inputDirectory, INamingSettings namingSettings, string? fileFilters, bool includeSubdirectories, bool simulation, ILogger? logger)
    {
        if (inputDirectory == null) throw new ArgumentNullException(nameof(inputDirectory));
        if (namingSettings == null) throw new ArgumentNullException(nameof(namingSettings));
        if (fileFilters == null) throw new ArgumentNullException(nameof(fileFilters));
        _renameSettings = new RenameSettings
        {
            IncludeSubdirectories = includeSubdirectories,
            FileFilters = fileFilters,
            NamingSettings = namingSettings,
            InputDirectory = inputDirectory,
            Simulation = simulation,
            Logger = logger,
        };

        return this;
    }

    private class RenameSettings
    {
        public bool IncludeSubdirectories { get; init; }
        public string? FileFilters { get; init; }
        public INamingSettings NamingSettings { get; init; } = null!;
        public string InputDirectory { get; init; } = null!;
        public bool Simulation { get; init; }
        public ILogger? Logger { get; init; }
    }
}

public interface IFilesRenamerRunnable : IRunnable
{
    IFilesRenamerRunnable Setup(string inputDirectory, INamingSettings namingSettings, string? fileFilters, bool includeSubdirectories, bool simulation, ILogger? logger);
}

