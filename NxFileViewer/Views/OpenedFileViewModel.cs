using System;
using Emignatik.NxFileViewer.Model;
using Emignatik.NxFileViewer.Utils.MVVM;

namespace Emignatik.NxFileViewer.Views
{
    public class OpenedFileViewModel : ViewModelBase
    {
        private readonly OpenedFile _openedFile;

        public OpenedFileViewModel(OpenedFile openedFile, IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _openedFile = openedFile ?? throw new ArgumentNullException(nameof(openedFile));
            Content = new ContentViewModel(_openedFile.RootItem, serviceProvider);
            FileOverview = new FileOverviewViewModel(_openedFile.Overview, serviceProvider);
        }

        public IServiceProvider ServiceProvider { get; }

        public FileOverviewViewModel FileOverview { get; }

        public ContentViewModel Content { get; }

    }
}
