using System;
using Emignatik.NxFileViewer.Model;
using Emignatik.NxFileViewer.Utils.MVVM;

namespace Emignatik.NxFileViewer.Views
{
    public class OpenedFileViewModel : ViewModelBase
    {
        private readonly NxFile _nxFile;

        public OpenedFileViewModel(NxFile nxFile, IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _nxFile = nxFile ?? throw new ArgumentNullException(nameof(nxFile));
            Content = new ContentViewModel(_nxFile.RootItem, serviceProvider);
            FileOverview = new FileOverviewViewModel(_nxFile.Overview, serviceProvider);
        }

        public IServiceProvider ServiceProvider { get; }

        public FileOverviewViewModel FileOverview { get; }

        public ContentViewModel Content { get; }

    }
}
