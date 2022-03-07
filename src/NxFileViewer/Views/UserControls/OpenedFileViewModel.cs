using System;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Models;
using Emignatik.NxFileViewer.Utils.MVVM;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views.UserControls
{
    public class OpenedFileViewModel : ViewModelBase
    {
        private readonly NxFile _nxFile;

        public OpenedFileViewModel(NxFile nxFile, IServiceProvider serviceProvider)
        {
            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _nxFile = nxFile ?? throw new ArgumentNullException(nameof(nxFile));
            Content = new ContentViewModel(_nxFile.RootItem, serviceProvider);
            FileOverview = new FileOverviewViewModel(_nxFile.Overview, serviceProvider);

            OpenFileLocationCommand = serviceProvider.GetRequiredService<IOpenFileLocationCommand>();
            OpenFileLocationCommand.FilePath = _nxFile.Path;
        }

        public FileOverviewViewModel FileOverview { get; }

        public ContentViewModel Content { get; }

        public string FilePath => _nxFile.Path;

        public IOpenFileLocationCommand OpenFileLocationCommand { get; }
    }
}
