using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Model.Overview;
using Emignatik.NxFileViewer.Styling.Theme;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views
{
    public class FileOverviewViewModel : ViewModelBase
    {
        private readonly FileOverview _fileOverview;
        private readonly IBrushesProvider _brushesProvider;

        private CnmtContainerViewModel? _selectedCnmtContainer;
        private string _missingKeys = "";
        private string? _ncasHeadersSignatureExceptions;
        private string? _ncasHashExceptions;

        public FileOverviewViewModel(FileOverview fileOverview, IServiceProvider serviceProvider)
        {
            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _fileOverview = fileOverview ?? throw new ArgumentNullException(nameof(fileOverview));

            _brushesProvider = serviceProvider.GetRequiredService<IBrushesProvider>();
            VerifyNcasHeaderSignatureCommand = serviceProvider.GetRequiredService<IVerifyNcasHeaderSignatureCommand>();
            VerifyNcasHashCommand = serviceProvider.GetRequiredService<IVerifyNcasHashCommand>();
            CopyMissingKeysCommand = new RelayCommand(CopyMissingKeys);

            _fileOverview.PropertyChanged += OnFileOverviewPropertyChanged;
            _fileOverview.MissingKeys.CollectionChanged += (_, _) =>
            {
                UpdateMissingKeys();
            };

            CnmtContainers = _fileOverview.CnmtContainers.Select((contentOverview, i) => new CnmtContainerViewModel(contentOverview, i + 1, serviceProvider)).ToArray();
            SelectedCnmtContainer = CnmtContainers.FirstOrDefault();

            UpdateMissingKeys();
            UpdateNcasHashExceptions();
            UpdateNcasHeadersSignatureExceptions();
        }

        public string MissingKeys
        {
            get => _missingKeys;
            private set
            {
                _missingKeys = value;
                NotifyPropertyChanged();
            }
        }

        public NcasValidity NcasHashValidity => _fileOverview.NcasHashValidity;

        public NcasValidity NcaHeadersSignatureValidity => _fileOverview.NcasHeadersSignatureValidity;

        public IVerifyNcasHeaderSignatureCommand VerifyNcasHeaderSignatureCommand { get; }

        public IVerifyNcasHashCommand VerifyNcasHashCommand { get; }

        public RelayCommand CopyMissingKeysCommand { get; }

        public bool IsMultiContentPackage => _fileOverview.CnmtContainers.Count > 1;

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

        public string? NcasHeadersSignatureExceptions
        {
            get => _ncasHeadersSignatureExceptions;
            private set
            {
                _ncasHeadersSignatureExceptions = value;
                NotifyPropertyChanged();
            }
        }

        public string? NcasHashExceptions
        {
            get => _ncasHashExceptions;
            private set
            {
                _ncasHashExceptions = value;
                NotifyPropertyChanged();
            }
        }

        private void UpdateMissingKeys()
        {
            var missingKeys = new List<string>();
            foreach (var missingKey in _fileOverview.MissingKeys)
            {
                missingKeys.Add($"• {LocalizationManager.Instance.Current.Keys.ToolTip_KeyMissing.SafeFormat(missingKey.KeyName, missingKey.KeyType)}");
            }

            MissingKeys = string.Join(Environment.NewLine, missingKeys);
        }

        private void CopyMissingKeys()
        {
            try
            {
                var missingKeys = MissingKeys;
                Clipboard.SetText(missingKeys);
            }
            catch
            {
                // ignored
            }
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
                case nameof(_fileOverview.NcasHashExceptions):
                    UpdateNcasHashExceptions();
                    break;
                case nameof(_fileOverview.NcasHeadersSignatureExceptions):
                    UpdateNcasHeadersSignatureExceptions();
                    break;
            }
        }

        private void UpdateNcasHashExceptions()
        {
            var exceptions = _fileOverview.NcasHashExceptions;
            NcasHashExceptions = ExceptionsToString(exceptions);
        }


        private void UpdateNcasHeadersSignatureExceptions()
        {
            var exceptions = _fileOverview.NcasHeadersSignatureExceptions;
            NcasHeadersSignatureExceptions = ExceptionsToString(exceptions);
        }

        private string? ExceptionsToString(IReadOnlyList<Exception>? exceptions)
        {
            if (exceptions == null)
                return null;

            return string.Join(Environment.NewLine, exceptions.Select(ex => $"• {ex.Message}"));
        }
    }

}
