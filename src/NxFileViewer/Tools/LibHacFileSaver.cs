using System;
using System.IO;
using System.Threading;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;

namespace Emignatik.NxFileViewer.Tools
{
    public class LibHacFileSaver : ILibHacFileSaver
    {
        private int _bufferSize = 1024 * 1024 * 4;

        public void Save(IFile srcFile, string dstFilePath, CancellationToken cancellationToken, ProgressValueHandler? progressValueHandler)
        {
            var targetDirectory = Path.GetDirectoryName(dstFilePath)!;
            if (!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);

            using var srcStream = srcFile.AsStream();
            using var dstStream = File.Create(dstFilePath);

            decimal totalBytes = srcStream.Length;

            var buffer = new byte[BufferSize];

            long totalBytesWritten = 0;
            int nbBytesRead;
            do
            {
                nbBytesRead = srcStream.Read(buffer);
                dstStream.Write(buffer, 0, nbBytesRead);
                totalBytesWritten += nbBytesRead;

                var progressValue = totalBytes == 0 ? 1.0 : (double)(totalBytesWritten / totalBytes);
                progressValueHandler?.Invoke(progressValue);

            } while (nbBytesRead > 0 && !cancellationToken.IsCancellationRequested);

            if (cancellationToken.IsCancellationRequested)
            {
                dstStream.Dispose();
                File.Delete(dstFilePath);
            }
        }

        public int BufferSize
        {
            get => _bufferSize;
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(BufferSize));

                _bufferSize = value;
            }
        }
    }
}
