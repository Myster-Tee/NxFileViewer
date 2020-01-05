using System;
using System.Linq;
using Emignatik.NxFileViewer.NSP.Models;

namespace Emignatik.NxFileViewer.Views.NSP
{
    public class ControlPartitionViewModel : ViewModelBase
    {
        private readonly ControlPartitionInfo _controlPartitionInfo;
        private TitleInfo _selectedTitle;

        private string _appName;
        private string _publisher;
        private byte[] _icon;


        public ControlPartitionViewModel(ControlPartitionInfo controlPartitionInfo)
        {
            _controlPartitionInfo = controlPartitionInfo ?? throw new ArgumentNullException(nameof(controlPartitionInfo));


            var titles = Titles;
            SelectedTitle = titles?.FirstOrDefault();
        }

        public byte[] Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                NotifyPropertyChanged();
            }
        }

        public string DisplayVersion => _controlPartitionInfo.Nacp?.DisplayVersion;

        public TitleInfo[] Titles => _controlPartitionInfo?.Nacp?.Titles;

        public string AppName
        {
            get => _appName;
            private set
            {
                _appName = value;
                NotifyPropertyChanged();
            }
        }

        public string Publisher
        {
            get => _publisher;
            private set
            {
                _publisher = value;
                NotifyPropertyChanged();
            }
        }


        public TitleInfo SelectedTitle
        {
            get => _selectedTitle;
            set
            {
                _selectedTitle = value;
                NotifyPropertyChanged();
                UpdateOnSelectedTitleChanged();
            }
        }

        private void UpdateOnSelectedTitleChanged()
        {
            var selectedTitle = SelectedTitle;
            AppName = selectedTitle != null ? selectedTitle.AppName : "";
            Publisher = selectedTitle != null ? selectedTitle.Publisher : "";
            Icon = GetIconForTitle(selectedTitle);
        }

        private byte[] GetIconForTitle(TitleInfo title)
        {
            var iconsInfo = _controlPartitionInfo.Icons;
            if (title == null || iconsInfo == null)
                return null;

            var iconOfSelectedLanguage = iconsInfo.FirstOrDefault(iconTmp => iconTmp.Language == title.Language);

            return iconOfSelectedLanguage?.Image;
        }
    }
}
