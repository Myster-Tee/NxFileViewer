using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Models.TreeItems;
using Emignatik.NxFileViewer.Styling.Theme;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Emignatik.NxFileViewer.Services.PackageAnalysis;
using Emignatik.NxFileViewer.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views.UserControls;

public class FileOverviewViewModel : ViewModelBase
{
    private readonly FileOverview _fileOverview;
    private readonly IBrushesProvider _brushesProvider;

    private CnmtContainerViewModel? _selectedCnmtContainer;
    private string _missingKeys = "";

    public FileOverviewViewModel(FileOverview fileOverview, IServiceProvider serviceProvider)
    {
        serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _fileOverview = fileOverview ?? throw new ArgumentNullException(nameof(fileOverview));

        _brushesProvider = serviceProvider.GetRequiredService<IBrushesProvider>();
        ThemeObserver.Observe(serviceProvider.GetService<IThemeService>(), this,
            static view => view.NotifyPropertyChanged(nameof(NcasIntegrityValidityColor)));
        VerifyNcasIntegrityCommand = serviceProvider.GetRequiredService<IVerifyNcasIntegrityCommand>();
        CopyMissingKeysCommand = new RelayCommand(CopyMissingKeys);

        _fileOverview.PropertyChanged += OnFileOverviewPropertyChanged;
        _fileOverview.MissingKeys.CollectionChanged += (_, _) =>
        {
            UpdateMissingKeys();
        };

        foreach (var cnmtContainerViewModel in _fileOverview.CnmtContainers.Select((contentOverview, i) => new CnmtContainerViewModel(contentOverview, i + 1, serviceProvider)))
        {
            CnmtContainers.Add(cnmtContainerViewModel);
        }

        SelectedCnmtContainer = CnmtContainers.FirstOrDefault();

        foreach (var nca in _fileOverview.RootItem.FindChildrenOfType<Emignatik.NxFileViewer.Models.TreeItems.Impl.NcaItem>(includeItem: true))
            System.Windows.WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(nca, nameof(INotifyPropertyChanged.PropertyChanged), OnNcaSignatureChanged);
        UpdateMissingKeys();
    }

    public bool HasMissingKeys => _fileOverview.MissingKeys.Count > 0;

    public string MissingKeys
    {
        get => _missingKeys;
        private set
        {
            _missingKeys = value;
            NotifyPropertyChanged();
        }
    }

    public NcasIntegrity NcasIntegrity => _fileOverview.NcasIntegrity;


    public IVerifyNcasIntegrityCommand VerifyNcasIntegrityCommand { get; }

    public RelayCommand CopyMissingKeysCommand { get; }


    public string FileType => _fileOverview.FileType.ToString();

    public string CompressionType => _fileOverview.NcaCompressionType.ToString();

    public string PackageStructure => _fileOverview.PackageStructure switch
    {
        Services.PackageAnalysis.PackageStructure.Scene => LocalizationManager.Instance.Current.Keys.PackageStructure_Scene,
        Services.PackageAnalysis.PackageStructure.Cdn => LocalizationManager.Instance.Current.Keys.PackageStructure_Cdn,
        Services.PackageAnalysis.PackageStructure.Converted => LocalizationManager.Instance.Current.Keys.PackageStructure_Converted,
        Services.PackageAnalysis.PackageStructure.Homebrew => LocalizationManager.Instance.Current.Keys.PackageStructure_Homebrew,
        Services.PackageAnalysis.PackageStructure.Incomplete => LocalizationManager.Instance.Current.Keys.PackageStructure_Incomplete,
        Services.PackageAnalysis.PackageStructure.Filesystem => LocalizationManager.Instance.Current.Keys.PackageStructure_Filesystem,
        _ => LocalizationManager.Instance.Current.Keys.PackageStructure_Unknown
    };

    private void OnNcaSignatureChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(Emignatik.NxFileViewer.Models.TreeItems.Impl.NcaItem.HeaderSignatureValidity)) NotifyPropertyChanged(nameof(NcaSignature));
    }

    public string NcaSignature
    {
        get
        {
            var signatures = _fileOverview.RootItem.FindChildrenOfType<Emignatik.NxFileViewer.Models.TreeItems.Impl.NcaItem>(includeItem: true).Select(nca => nca.HeaderSignatureValidity).ToArray();
            var keys = LocalizationManager.Instance.Current.Keys;
            if (signatures.Length == 0) return keys.Security_Unavailable;
            if (signatures.Any(validity => validity == LibHac.Common.Validity.Invalid)) return keys.Signature_NotPassed;
            return signatures.All(validity => validity == LibHac.Common.Validity.Valid) ? keys.Signature_Passed : keys.Signature_Unchecked;
        }
    }

    public string FileSize => _fileOverview.FileSize.ToFileSize();

    public string CompressionRatio => _fileOverview.CompressionRatio is { } ratio
        ? $"{ratio:P1} ({_fileOverview.EstimatedUncompressedSize.ToFileSize()} {LocalizationManager.Instance.Current.Keys.FileInfo_Uncompressed})"
        : "—";

    public string? SystemUpdateVersion => _fileOverview.SystemUpdateVersion;

    public Visibility SystemUpdateVisibility => string.IsNullOrWhiteSpace(SystemUpdateVersion)
        ? Visibility.Collapsed
        : Visibility.Visible;

    public bool IsSuperPackage => _fileOverview.IsSuperPackage;

    public ObservableCollection<CnmtContainerViewModel> CnmtContainers { get; } = new();

    public CnmtContainerViewModel? SelectedCnmtContainer
    {
        get => _selectedCnmtContainer;
        set
        {
            _selectedCnmtContainer = value;
            NotifyPropertyChanged();
        }
    }

    public string MultiContentPackageToolTip => LocalizationManager.Instance.Current.Keys.MultiContentPackageToolTip.SafeFormat(this._fileOverview.FileType.ToString());

    public Brush NcasIntegrityValidityColor
    {
        get
        {
            switch (NcasIntegrity)
            {
                case NcasIntegrity.NoNca:
                case NcasIntegrity.Unchecked:
                case NcasIntegrity.InProgress:
                    return _brushesProvider.FontBrushDefault;
                case NcasIntegrity.Original:
                    return _brushesProvider.FontBrushSuccess;
                case NcasIntegrity.Incomplete:
                case NcasIntegrity.Modified:
                    return _brushesProvider.FontBrushWarning;
                case NcasIntegrity.Corrupted:
                case NcasIntegrity.Error:
                default:
                    return _brushesProvider.FontBrushError;
            }
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
        NotifyPropertyChanged(nameof(HasMissingKeys));
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
            case nameof(_fileOverview.NcasIntegrity):
                NotifyPropertyChanged(nameof(NcasIntegrity));
                NotifyPropertyChanged(nameof(NcasIntegrityValidityColor));
                break;
        }
    }



}
