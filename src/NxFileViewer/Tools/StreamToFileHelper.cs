using System;
using System.IO;
using System.Threading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Settings;

namespace Emignatik.NxFileViewer.Tools
{
    public class StreamToFileHelper : IStreamToFileHelper
    {
        private readonly IAppSettings _appSettings;

        public StreamToFileHelper(IAppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public void Save(Stream srcStream, string dstFilePath, CancellationToken cancellationToken, ProgressValueHandler? progressValueHandler)
        {
            var bufferSize = _appSettings.ProgressBufferSize;
            if (bufferSize <= 0)
                throw new InvalidOperationException(LocalizationManager.Instance.Current.Keys.InvalidSetting_BufferSizeInvalid.SafeFormat(bufferSize));

            var targetDirectory = Path.GetDirectoryName(dstFilePath)!;
            if (!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);

            using var dstStream = File.Create(dstFilePath);
            try
            {

                decimal totalBytes = srcStream.Length;

                var buffer = new byte[bufferSize];

                long totalBytesWritten = 0;
                do
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var nbBytesRead = srcStream.Read(buffer);
                    dstStream.Write(buffer, 0, nbBytesRead);
                    totalBytesWritten += nbBytesRead;

                    var progressValue = totalBytes == 0 ? 1.0 : (double)(totalBytesWritten / totalBytes);
                    progressValueHandler?.Invoke(progressValue);

                } while (totalBytesWritten < totalBytes);
            }
            catch (OperationCanceledException)
            {
                dstStream.Dispose();
                File.Delete(dstFilePath);
                throw;
            }
        }
    }
}
