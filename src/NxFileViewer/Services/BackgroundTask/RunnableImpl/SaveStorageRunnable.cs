using System;
using System.Threading;
using Emignatik.NxFileViewer.Tools;
using LibHac.Fs;
using LibHac.Tools.FsSystem;

namespace Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl
{
    public class SaveStorageRunnable : ISaveStorageRunnable
    {
        private readonly IStreamToFileHelper _streamToFileHelper;

        private IStorage? _srcStorage;
        private string? _dstFilePath;


        public SaveStorageRunnable(IStreamToFileHelper streamToFileHelper)
        {
            _streamToFileHelper = streamToFileHelper ?? throw new ArgumentNullException(nameof(streamToFileHelper));
        }

        public bool SupportsCancellation => true;

        public bool SupportProgress => true;


        public void Setup(IStorage srcStorage, string dstFilePath)
        {
            _dstFilePath = dstFilePath ?? throw new ArgumentNullException(nameof(dstFilePath));
            _srcStorage = srcStorage ?? throw new ArgumentNullException(nameof(srcStorage));
        }

        public void Run(IProgressReporter progressReporter, CancellationToken cancellationToken)
        {
            if (_srcStorage == null || _dstFilePath == null)
                throw new InvalidOperationException($"{nameof(Setup)} should be called first.");

            _streamToFileHelper.Save(_srcStorage.AsStream(), _dstFilePath, cancellationToken, progressReporter.SetPercentage);
        }


    }

    public interface ISaveStorageRunnable : IRunnable
    {
        void Setup(IStorage srcStorage, string dstFilePath);
    }
}
