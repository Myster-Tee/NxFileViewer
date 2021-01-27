using System;
using System.IO;
using System.Reflection;

namespace Emignatik.NxFileViewer.Utils
{
    public static class PathHelper
    {
        static PathHelper()
        {
            try
            {
                CurrentAppDir = AppDomain.CurrentDomain.BaseDirectory ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Directory.GetCurrentDirectory();
            }
            catch
            {
                CurrentAppDir = Directory.GetCurrentDirectory();
            }
                
            try
            {
                HomeUserDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
            catch
            {
                HomeUserDir = null;
            }
        }

        public static string? HomeUserDir { get; }


        public static string CurrentAppDir { get; }

        /// <summary>
        /// Compute the full path relative to the current application directory
        /// </summary>
        /// <param name="relOrAbsPath"></param>
        /// <returns></returns>
        public static string ToFullPath(this string? relOrAbsPath)
        {

            if (string.IsNullOrEmpty(relOrAbsPath))
                return CurrentAppDir;

            return Path.GetFullPath(Path.IsPathRooted(relOrAbsPath) ? relOrAbsPath : Path.Combine(CurrentAppDir, relOrAbsPath));
        }

    }
}
