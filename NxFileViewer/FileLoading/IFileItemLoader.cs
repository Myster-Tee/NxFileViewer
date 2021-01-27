using Emignatik.NxFileViewer.Model.TreeItems;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;

namespace Emignatik.NxFileViewer.FileLoading
{
    /// <summary>
    /// <see cref="IItem"/> tree loader from a Nintendo Switch file
    /// </summary>
    public interface IFileItemLoader
    {
        /// <summary>
        /// Loads an XCI file
        /// </summary>
        /// <param name="xciFilePath"></param>
        /// <returns></returns>
        XciItem LoadXci(string xciFilePath);

        /// <summary>
        /// Loads an NSP file
        /// </summary>
        /// <param name="nspFilePath"></param>
        /// <returns></returns>
        NspItem LoadNsp(string nspFilePath);
    }
}