using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.Overview;
using Emignatik.NxFileViewer.Styling.Theme;
using Emignatik.NxFileViewer.Utils.MVVM;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views
{
    public class FileOverviewViewModel : ViewModelBase
    {
        private readonly FileOverview _fileOverview;
        private readonly IBrushesProvider _brushesProvider;

        private CnmtContainerViewModel? _selectedCnmtContainer;

        public FileOverviewViewModel(FileOverview fileOverview, IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _fileOverview = fileOverview ?? throw new ArgumentNullException(nameof(fileOverview));

            _brushesProvider = serviceProvider.GetRequiredService<IBrushesProvider>();
            VerifyNcasHeaderSignatureCommand = serviceProvider.GetRequiredService<IVerifyNcasHeaderSignatureCommand>();
            VerifyNcasHashCommand = serviceProvider.GetRequiredService<IVerifyNcasHashCommand>();

            _fileOverview.PropertyChanged += OnFileOverviewPropertyChanged;
            CnmtContainers = _fileOverview.CnmtContainers.Select((contentOverview, i) => new CnmtContainerViewModel(contentOverview, i + 1, serviceProvider)).ToArray();
            SelectedCnmtContainer = CnmtContainers.FirstOrDefault();
        }

        private void OnFileOverviewPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_fileOverview.NcasHeadersSignatureValidity):
                    NotifyPropertyChanged(nameof(NcaHeadersSignatureValidity));
                    NotifyPropertyChanged(nameof(NcaHeadersSignatureValidityColor));
                    break;
                case nameof(_fileOverview.NcasHashValidity):
                    NotifyPropertyChanged(nameof(NcasHashValidity));
                    NotifyPropertyChanged(nameof(NcasHashValidityColor));
                    break;
            }
        }

        public NcasValidity NcasHashValidity => _fileOverview.NcasHashValidity;

        public NcasValidity NcaHeadersSignatureValidity => _fileOverview.NcasHeadersSignatureValidity;

        public IVerifyNcasHeaderSignatureCommand VerifyNcasHeaderSignatureCommand { get; }

        public IVerifyNcasHashCommand VerifyNcasHashCommand { get; }

        public bool IsMultiContentPackage => _fileOverview.CnmtContainers.Count > 1;

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

        public string MultiContentPackageToolTip => LocalizationManager.Instance.Current.Keys.MultiContentPackageToolTip.SafeFormat(PackageType.SuperXCI.ToString());

        public Brush NcaHeadersSignatureValidityColor
        {
            get
            {
                switch (NcaHeadersSignatureValidity)
                {
                    case NcasValidity.NoNca:
                    case NcasValidity.Unchecked:
                    case NcasValidity.InProgress:
                        return _brushesProvider.FontBrushDefault;
                    case NcasValidity.Valid:
                        return _brushesProvider.FontBrushSuccess;
                    case NcasValidity.Invalid:
                        return _brushesProvider.FontBrushWarning; // An invalid signature often happens and does not mean it is corrupted therefore it is a warning
                    case NcasValidity.Error:
                    default:
                        return _brushesProvider.FontBrushError;
                }
            }
        }

        public Brush NcasHashValidityColor
        {
            get
            {
                switch (NcasHashValidity)
                {
                    case NcasValidity.NoNca:
                    case NcasValidity.Unchecked:
                    case NcasValidity.InProgress:
                        return _brushesProvider.FontBrushDefault;
                    case NcasValidity.Valid:
                        return _brushesProvider.FontBrushSuccess;
                    case NcasValidity.Invalid:
                    case NcasValidity.Error:
                    default:
                        return _brushesProvider.FontBrushError;
                }
            }
        }
    }

}
