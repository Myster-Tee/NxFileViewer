using System;
using System.IO;
using System.Threading;
using Emignatik.NxFileViewer.FileLoading;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Services.BackgroundTask;
using Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl;
using Emignatik.NxFileViewer.Services.Integrity;
using Emignatik.NxFileViewer.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Services.Nsz;

public sealed class ConversionVerifier(IFileLoader loader, IServiceProvider services, IAppSettings settings) : IConversionVerifier
{
    public BatchIntegrityResult Verify(string path, IProgressReporter progress, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var file = loader.Load(path);
        var runnable = services.GetRequiredService<IVerifyNcasIntegrityRunnable>();
        runnable.Setup(file.Overview, settings.IgnoreMissingDeltaFragments);
        runnable.Run(progress, cancellationToken);
        var overview = file.Overview;
        return new(path, Path.GetExtension(path).TrimStart('.').ToUpperInvariant(), overview.FileType.ToString(),
            overview.PackageStructure.ToString(), overview.NcaCompressionType.ToString(), overview.NcasIntegrity,
            overview.NcasIntegrity == NcasIntegrity.Original ? null : LocalizationManager.Instance.Current.Keys.BatchIntegrity_IntegrityFailed);
    }
}
