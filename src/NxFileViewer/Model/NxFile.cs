using System;
using Emignatik.NxFileViewer.Model.Overview;
using Emignatik.NxFileViewer.Model.TreeItems;

namespace Emignatik.NxFileViewer.Model
{
    /// <summary>
    /// Represents an actually opened file
    /// </summary>
    public class NxFile : IDisposable
    {
        public NxFile(string path, IItem rootItem, FileOverview overview)
        {
            Path = path;
            RootItem = rootItem;
            Overview = overview;
            FileName = System.IO.Path.GetFileName(path);
        }

        /// <summary>
        /// Get the path of the opened file
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Get the root item of the opened file
        /// </summary>
        public IItem RootItem { get; }

        /// <summary>
        /// Get the opened file overview information
        /// </summary>
        public FileOverview Overview { get; }

        public string FileName { get; }

        public void Dispose()
        {
            RootItem.Dispose();
        }
    }
}