using System.IO;
using System.Text.RegularExpressions;

namespace Emignatik.NxFileViewer.Tools
{
    public class FsSanitizer : IFsSanitizer
    {
        private readonly Regex _fileNameRegex;
        private readonly Regex _pathRegex;

        public FsSanitizer()
        {
            _fileNameRegex = new Regex($"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]");
            _pathRegex = new Regex($"[{Regex.Escape(new string(Path.GetInvalidPathChars()) + "*?")}]");
        }

        public string SanitizeFileName(string fileName)
        {
            return _fileNameRegex.Replace(fileName, "_");
        }

        public string SanitizePath(string path)
        {
            return _pathRegex.Replace(path, "_");
        }
    }
}