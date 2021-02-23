using System.Threading;
using LibHac.Fs.Fsa;

namespace Emignatik.NxFileViewer.Tools
{

    /// <summary>
    /// </summary>
    /// <param name="value">A value in range of [0-1]</param>
    public delegate void ProgressValueHandler(double value);

    public interface ILibHacFileSaver
    {

        int BufferSize { get; set; }

        /// <summary>
        /// Saves the LibHac file to the specified path
        /// </summary>
        /// <param name="srcFile"></param>
        /// <param name="dstFilePath"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progressValueHandler"></param>
        public void Save(IFile srcFile, string dstFilePath, CancellationToken cancellationToken, ProgressValueHandler? progressValueHandler = null);
    }
}