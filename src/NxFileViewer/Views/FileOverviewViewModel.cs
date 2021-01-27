using System;
using System.Linq;
using System.Windows;
using Emignatik.NxFileViewer.Model.Overview;
using Emignatik.NxFileViewer.Utils.MVVM;

namespace Emignatik.NxFileViewer.Views
{
    public class FileOverviewViewModel : ViewModelBase
    {
        private readonly FileOverview _fileOverview;
        private CnmtContainerViewModel? _selectedCnmtContainer;

        public FileOverviewViewModel(FileOverview fileOverview, IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _fileOverview = fileOverview;

            CnmtContainers = _fileOverview.CnmtContainers.Select((contentOverview, i) => new CnmtContainerViewModel(contentOverview, i + 1, serviceProvider)).ToArray();
            SelectedCnmtContainer = CnmtContainers.FirstOrDefault();
            ContentSelectorVisibility = CnmtContainers.Length > 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        public IServiceProvider ServiceProvider { get; }

        public CnmtContainerViewModel[] CnmtContainers { get; }

        public CnmtContainerViewModel? SelectedCnmtContainer
        {
            get => _selectedCnmtContainer;
            set
            {
                _selectedCnmtContainer = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility ContentSelectorVisibility { get; }

        public string SpecialFileInfo
        {
            get
            {
                switch (_fileOverview.PackageType)
                {
                    case PackageType.SuperXCI:
                        return "Super XCI";
                    case PackageType.SuperNSP:
                        return "Super NSP";
                    default:
                        return "";
                }
            }
        }
    }
}
