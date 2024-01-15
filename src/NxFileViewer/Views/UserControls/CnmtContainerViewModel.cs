using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Models.Overview;
using Emignatik.NxFileViewer.Utils.MVVM;
using LibHac.Ns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Views.UserControls;

public class CnmtContainerViewModel : ViewModelBase
{
    private readonly CnmtContainer _cnmtContainer;
    private readonly int _containerNumber;
    private TitleInfoViewModel? _selectedTitle;

    public CnmtContainerViewModel(CnmtContainer cnmtContainer, int containerNumber, IServiceProvider serviceProvider)
    {
        _containerNumber = containerNumber;
        _cnmtContainer = cnmtContainer ?? throw new ArgumentNullException(nameof(cnmtContainer));
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        SaveSelectedImageCommand = serviceProvider.GetRequiredService<ISaveTitleImageCommand>();
        CopySelectedImageCommand = serviceProvider.GetRequiredService<ICopyImageCommand>();

        var nacpContainer = _cnmtContainer.NacpContainer;
        if (nacpContainer != null)
        {
            var titleInfoViewModels = nacpContainer.Titles.Select(titleInfo => new TitleInfoViewModel(titleInfo, serviceProvider));

            foreach (var titleInfoViewModel in titleInfoViewModels)
            {
                Titles.Add(titleInfoViewModel);
            }
        }

        SelectedTitle = Titles.FirstOrDefault();
    }

    public IServiceProvider ServiceProvider { get; }

    public ICopyImageCommand CopySelectedImageCommand { get; }

    public ISaveTitleImageCommand SaveSelectedImageCommand { get; }

    public string TitleId => _cnmtContainer.CnmtItem.TitleId;

    public ObservableCollection<TitleInfoViewModel> Titles { get; } = new();

    public TitleInfoViewModel? SelectedTitle
    {
        get => _selectedTitle;
        set
        {
            _selectedTitle = value;
            SaveSelectedImageCommand.Title = value?.Title;
            CopySelectedImageCommand.Image = value?.Icon;
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Name of the container (displayed when Super NSP or XCI)
    /// </summary>
    public string DisplayName => $"{_containerNumber} {_cnmtContainer.CnmtItem.ContentType}";

    /// <summary>
    /// The type of container (Addon, Patch, Application, etc.)
    /// </summary>
    public string Type => _cnmtContainer.CnmtItem.ContentType.ToString();

    /// <summary>
    /// Corresponds to the technical patch level
    /// </summary>
    public string? TitleVersion => _cnmtContainer.CnmtItem.TitleVersion;

    /// <summary>
    /// Get the patch level
    /// </summary>
    public string PatchLevel => LocalizationManager.Instance.Current.Keys.ToolTip_PatchNumber.SafeFormat(_cnmtContainer.CnmtItem.PatchNumber);

    /// <summary>
    /// The minimum system version
    /// </summary>
    public string? MinimumSystemVersion => _cnmtContainer.CnmtItem.MinimumSystemVersion?.ToString();

    /// <summary>
    /// End user displayed version
    /// </summary>
    public string? DisplayVersion => _cnmtContainer.NacpContainer?.NacpItem.DisplayVersion;

    /// <summary>
    /// Indicates if title is a demo
    /// </summary>
    public bool IsDemo => _cnmtContainer.NacpContainer?.NacpItem.Attribute == ApplicationControlProperty.AttributeFlagValue.Demo;

    /// <summary>
    /// The build ID
    /// </summary>
    public string BuildID
    {
        get
        {
            if (_cnmtContainer.MainItem != null)
                return _cnmtContainer.MainItem?.ModuleId ?? "";
            if (_cnmtContainer.MainItemSectionIsSparse)
                return LocalizationManager.Instance.Current.Keys.CnmtOverview_BuildID_NotAvailableBecauseSectionIsSparse;
            else
                return "";
        }
    }

    public Visibility PresentationGroupBoxVisibility => _cnmtContainer.NacpContainer == null ? Visibility.Collapsed : Visibility.Visible;

}

/// <summary>
/// View model for a title and an icon for a specific region/language
/// </summary>
public class TitleInfoViewModel
{
    private readonly ILogger _logger;

    public TitleInfoViewModel(TitleInfo titleInfo, IServiceProvider serviceProvider)
    {
        Title = titleInfo ?? throw new ArgumentNullException(nameof(titleInfo));
        serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(this.GetType());

        Icon = BuildBitmapImage(Title.Icon);
    }

    public BitmapSource? Icon { get; }

    public TitleInfo Title { get; }

    public string AppName => Title.AppName;

    public string Publisher => Title.Publisher;

    public NacpLanguage Language => Title.Language;

    private BitmapImage? BuildBitmapImage(byte[]? bytes)
    {
        if (bytes == null)
            return null;

        try
        {
            using var ms = new MemoryStream(bytes);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
        catch (Exception ex)
        {
            var message = LocalizationManager.Instance.Current.Keys.LoadingError_FailedToLoadIcon.SafeFormat(ex.Message);
            _logger.LogError(ex, message);
            return null;
        }
    }

}