using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Model.Overview;
using Emignatik.NxFileViewer.Utils.MVVM;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views
{
    public class CnmtContainerViewModel : ViewModelBase
    {
        private readonly CnmtContainer _cnmtContainer;
        private readonly int _containerNumber;
        private TitleInfo? _selectedTitle;

        public CnmtContainerViewModel(CnmtContainer cnmtContainer, int containerNumber, IServiceProvider serviceProvider)
        {
            _containerNumber = containerNumber;
            _cnmtContainer = cnmtContainer ?? throw new ArgumentNullException(nameof(cnmtContainer));
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            SaveSelectedImageCommand = serviceProvider.GetRequiredService<ISaveTitleImageCommand>();
            CopySelectedImageCommand = serviceProvider.GetRequiredService<ICopyTitleImageCommand>();
            Titles = _cnmtContainer.NacpContainer?.Titles;
            SelectedTitle = Titles?.FirstOrDefault();
        }

        public IServiceProvider ServiceProvider { get; }

        public ICopyTitleImageCommand CopySelectedImageCommand { get; }

        public ISaveTitleImageCommand SaveSelectedImageCommand { get; }

        public string TitleId => _cnmtContainer.CnmtItem.TitleId;

        public List<TitleInfo>? Titles { get; }

        public TitleInfo? SelectedTitle
        {
            get => _selectedTitle;
            set
            {
                _selectedTitle = value;
                SaveSelectedImageCommand.Title = value;
                CopySelectedImageCommand.Title = value;
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
        public string Type => _cnmtContainer.CnmtItem.ContentType;

        /// <summary>
        /// Corresponds to the technical patch level
        /// </summary>
        public string? TitleVersion => _cnmtContainer.CnmtItem.TitleVersion;

        /// <summary>
        /// The minimum system version
        /// </summary>
        public string? MinimumSystemVersion => _cnmtContainer.CnmtItem.MinimumSystemVersion;

        /// <summary>
        /// End user displayed version
        /// </summary>
        public string? DisplayVersion => _cnmtContainer.NacpContainer?.NacpItem.DisplayVersion;


        public Visibility PresentationGroupBoxVisibility => _cnmtContainer.NacpContainer == null ? Visibility.Collapsed : Visibility.Visible;
    }
}