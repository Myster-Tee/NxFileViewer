using System;
using System.IO;

namespace Emignatik.NxFileViewer.Utils
{
    public class TempDirMgr
    {
        private readonly string _tempDir;

        public TempDirMgr(string workingDir)
        {
            if (workingDir == null)
                throw new ArgumentNullException(nameof(workingDir));

            var rootedWorkingDir = workingDir.ToFullPath();

            _tempDir = Path.GetFullPath(Path.Combine(rootedWorkingDir, "Temp"));
        }

        public string TempDirPath => _tempDir;

        public void Initialize()
        {
            if (!Directory.Exists(_tempDir))
                Directory.CreateDirectory(_tempDir);
            else
                IoHelper.DeleteDirectoryContent(_tempDir);
        }

        public string CreateSubDir(string relDirPath, bool cleanupIfExists = true)
        {
            if (Path.IsPathRooted(relDirPath))
                throw new ArgumentException($"Specified path \"{relDirPath}\" should be relative.", nameof(relDirPath));

            var subDirFullPath = Path.GetFullPath(Path.Combine(_tempDir, relDirPath));
            if (!Directory.Exists(subDirFullPath))
                Directory.CreateDirectory(subDirFullPath);
            else if (cleanupIfExists)
                IoHelper.DeleteDirectoryContent(subDirFullPath);

            return subDirFullPath;
        }

    }
}
