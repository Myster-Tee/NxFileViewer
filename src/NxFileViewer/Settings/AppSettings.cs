using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Emignatik.NxFileViewer.Settings.Model;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings
{
    public class AppSettingsWrapper : IAppSettingsWrapper<AppSettingsModel>
    {
        private AppSettingsModel _appSettingsModel = new AppSettingsModel();

        public event PropertyChangedEventHandler? PropertyChanged;

        public string LastSaveDir
        {
            get => _appSettingsModel.LastSaveDir ?? "";
            set
            {
                _appSettingsModel.LastSaveDir = value;
                NotifyPropertyChanged();
            }
        }

        public string LastOpenedFile
        {
            get => _appSettingsModel.LastOpenedFile ?? "";
            set
            {
                _appSettingsModel.LastOpenedFile = value;
                NotifyPropertyChanged();
            }
        }

        public string ProdKeysFilePath
        {
            get => _appSettingsModel.KeysFilePath ?? "";
            set
            {
                _appSettingsModel.KeysFilePath = value;
                NotifyPropertyChanged();
            }
        }

        public string ConsoleKeysFilePath
        {
            get => _appSettingsModel.ConsoleKeysFilePath ?? "";
            set
            {
                _appSettingsModel.ConsoleKeysFilePath = value;
                NotifyPropertyChanged();
            }
        }

        public string TitleKeysFilePath
        {
            get => _appSettingsModel.TitleKeysFilePath ?? "";
            set
            {
                _appSettingsModel.TitleKeysFilePath = value;
                NotifyPropertyChanged();
            }
        }

        public LogLevel LogLevel
        {
            get => _appSettingsModel.LogLevel;
            set
            {
                _appSettingsModel.LogLevel = value;
                NotifyPropertyChanged();
            }
        }

        public string ProdKeysDownloadUrl
        {
            get => _appSettingsModel.ProdKeysDownloadUrl ?? "";
            set
            {
                _appSettingsModel.ProdKeysDownloadUrl = value;
                NotifyPropertyChanged();
            }
        }

        public string TitleKeysDownloadUrl
        {
            get => _appSettingsModel.TitleKeysDownloadUrl ?? "";
            set
            {
                _appSettingsModel.TitleKeysDownloadUrl = value;
                NotifyPropertyChanged();
            }
        }

        public AppSettingsModel WrappedModel => _appSettingsModel;

        public void Update(AppSettingsModel newModel)
        {
            _appSettingsModel = newModel ?? throw new ArgumentNullException(nameof(newModel));

            NotifyAllPropertiesChanged();
        }

        private void NotifyAllPropertiesChanged()
        {
            var properties = typeof(IAppSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                NotifyPropertyChanged(property.Name);
            }
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
