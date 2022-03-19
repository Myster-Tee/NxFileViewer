using System;
using System.IO;
using System.Threading;

namespace Emignatik.NxFileViewer.Tools;

/// <summary>
/// </summary>
/// <param name="value">A value in range of [0-1]</param>
public delegate void ProgressValueHandler(double value);

public interface IStreamToFileHelper
{

    /// <summary>
    /// Saves the LibHac file to the specified path
    /// </summary>
    /// <param name="srcStream"></param>
    /// <param name="dstFilePath"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="progressValueHandler"></param>
    /// <exception cref="OperationCanceledException"></exception>
    public void Save(Stream srcStream, string dstFilePath, CancellationToken cancellationToken, ProgressValueHandler? progressValueHandler = null);
}