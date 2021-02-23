using System;
using System.IO;
using System.Threading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Tools;
using LibHac.Fs.Fsa;

namespace Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl
{
    public class SaveFileRunnable : ISaveFileRunnable
    {
        private readonly ILibHacFileSaver _libHacFileSaver;

        private IFile? _srcFile;
        private string? _dstFilePath;

        public SaveFileRunnable(ILibHacFileSaver libHacFileSaver)
        {
            _libHacFileSaver = libHacFileSaver ?? throw new ArgumentNullException(nameof(libHacFileSaver));
        }

        public bool SupportsCancellation => true;

        public bool SupportProgress => true;

        public void Setup(IFile srcFile, string dstFilePath)
        {
            _srcFile = srcFile ?? throw new ArgumentNullException(nameof(srcFile));
            _dstFilePath = dstFilePath ?? throw new ArgumentNullException(nameof(dstFilePath));
        }

        public void Run(IProgressReporter progressReporter, CancellationToken cancellationToken)
        {
            if (_srcFile == null || _dstFilePath == null)
                throw new InvalidOperationException($"{nameof(Setup)} should be called first.");

            var progressText = LocalizationManager.Instance.Current.Keys.Status_SavingFile.SafeFormat(Path.GetFileName(_dstFilePath));
            progressReporter.SetText(progressText);


            _libHacFileSaver.Save(_srcFile, _dstFilePath, cancellationToken, progressReporter.SetPercentage);
        }
    }

    public interface ISaveFileRunnable : IRunnable
    {
        void Setup(IFile srcFile, string dstFilePath);
    }
}
