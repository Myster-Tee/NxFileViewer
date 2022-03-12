using System;
using System.Threading;
using Emignatik.NxFileViewer.Services.FileRenaming;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;

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
        _fileRenamerService.RenameFromDirectoryAsync(_renameSettings.InputDirectory, _renameSettings.NamingPatterns, _renameSettings.FileFilters, true, progressReporter, cancellationToken);
    }

    public void Setup(string inputDirectory, INamingPatterns namingPatterns, string? fileFilters, bool includeSubdirectories, bool simulation)
    {
        if (inputDirectory == null) throw new ArgumentNullException(nameof(inputDirectory));
        if (namingPatterns == null) throw new ArgumentNullException(nameof(namingPatterns));
        if (fileFilters == null) throw new ArgumentNullException(nameof(fileFilters));
        _renameSettings = new RenameSettings
        {
            IncludeSubdirectories = includeSubdirectories,
            FileFilters = fileFilters,
            NamingPatterns = namingPatterns,
            InputDirectory = inputDirectory,
            Simulation = simulation,
        };
    }

    private class RenameSettings
    {
        public bool IncludeSubdirectories { get; init; }
        public string? FileFilters { get; init; }
        public INamingPatterns NamingPatterns { get; init; } = null!;
        public string InputDirectory { get; init; } = null!;
        public bool Simulation { get; set; }
    }
}

public interface IFilesRenamerRunnable : IRunnable
{
    void Setup(string inputDirectory, INamingPatterns namingPatterns, string? fileFilters, bool includeSubdirectories, bool includeSubDirectories);
}

