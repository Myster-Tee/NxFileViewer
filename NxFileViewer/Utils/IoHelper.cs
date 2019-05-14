using System.IO;
using System.Linq;

namespace Emignatik.NxFileViewer.Utils
{
    public static class IoHelper
    {
        public static string SanitizeFileName(string fileName)
        {
            const string DEFAULT_FILE_NAME = "UNKNOWN";

            var sanitizedName = fileName ?? DEFAULT_FILE_NAME;

            sanitizedName = Path.GetInvalidFileNameChars()
                .Aggregate(sanitizedName, (current, invalidChar) => current.Replace(invalidChar, '_'))
                .Trim();

            if (sanitizedName == "")
                sanitizedName = DEFAULT_FILE_NAME;

            return sanitizedName;
        }

        public static void DeleteDirectoryContent(string dirPath)
        {
            var dir = new DirectoryInfo(dirPath);

            foreach (var file in dir.GetFiles())
            {
                file.Delete();
            }
            foreach (var subDir in dir.GetDirectories())
            {
                subDir.Delete(true);
            }
        }
    }
}