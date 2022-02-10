using System.Collections.Generic;

namespace Emignatik.NxFileViewer.Services;

public interface IFileRenamerService
{
    void Rename(string inputDirectory, string namingPattern, IReadOnlyCollection<string> fileExtensions, bool includeSubDirectories);
}