using System.Collections.Generic;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;

namespace Emignatik.NxFileViewer.Services.FileRenaming;

public interface IFileRenamerService
{
    void RenameFromDirectory(string inputDirectory, INamingPatterns namingPatterns, IReadOnlyCollection<string> fileExtensions, bool includeSubDirectories);

    void RenameFile(string inputFile, INamingPatterns namingPatterns);
}