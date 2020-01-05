using System;
using System.IO;

namespace Emignatik.NxFileViewer.Utils
{
    public static class PathHelper
    {

        /// <summary>
        /// Compute the full path relative to the current application directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ToFullPath(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return AppDomain.CurrentDomain.BaseDirectory;

            return Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path));
        }

    }
}
