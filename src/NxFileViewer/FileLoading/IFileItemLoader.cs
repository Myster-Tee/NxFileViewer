using System;
using Emignatik.NxFileViewer.Model.TreeItems;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using LibHac;

namespace Emignatik.NxFileViewer.FileLoading
{
    /// <summary>
    /// <see cref="IItem"/> tree loader from a Nintendo Switch file
    /// </summary>
    public interface IFileItemLoader
    {
        public event MissingKeyExceptionHandler? MissingKey;

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

    public delegate void MissingKeyExceptionHandler(object sender, MissingKeyExceptionHandlerArgs args);

    public class MissingKeyExceptionHandlerArgs
    {

        public MissingKeyExceptionHandlerArgs(MissingKeyException ex, IItem? parentItem)
        {
            Exception = ex ?? throw new ArgumentNullException(nameof(ex));
            ParentItem = parentItem;
        }

        public MissingKeyException Exception { get; }


        public IItem? ParentItem { get; }
    }
}