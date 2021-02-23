using System;

namespace Emignatik.NxFileViewer.FileLoading
{
    internal class FileNotSupportedException : Exception
    {
        public FileNotSupportedException(string? filePath)
        {
            FilePath = filePath;
        }

        public string? FilePath { get; }
    }
}