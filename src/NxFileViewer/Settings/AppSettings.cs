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
        public event PropertyChangedEventHandler? PropertyChanged;

        public string? AppLanguage
        {
            get => WrappedModel.AppLanguage ?? "";
            set
            {
                WrappedModel.AppLanguage = value;
                NotifyPropertyChanged();
            }
        }

        public string LastSaveDir
        {
            get => WrappedModel.LastSaveDir ?? "";
            set
            {
                WrappedModel.LastSaveDir = value;
                NotifyPropertyChanged();
            }
        }

        public string LastOpenedFile
        {
            get => WrappedModel.LastOpenedFile ?? "";
            set
            {
                WrappedModel.LastOpenedFile = value;
                NotifyPropertyChanged();
            }
        }

        public string ProdKeysFilePath
        {
            get => WrappedModel.KeysFilePath ?? "";
            set
            {
                WrappedModel.KeysFilePath = value;
                NotifyPropertyChanged();
            }
        }

        public string ConsoleKeysFilePath
        {
            get => WrappedModel.ConsoleKeysFilePath ?? "";
            set
            {
                WrappedModel.ConsoleKeysFilePath = value;
                NotifyPropertyChanged();
            }
        }

        public string TitleKeysFilePath
        {
            get => WrappedModel.TitleKeysFilePath ?? "";
            set
            {
                WrappedModel.TitleKeysFilePath = value;
                NotifyPropertyChanged();
            }
        }

        public LogLevel LogLevel
        {
            get => WrappedModel.LogLevel;
            set
            {
                WrappedModel.LogLevel = value;
                NotifyPropertyChanged();
            }
        }

        public string ProdKeysDownloadUrl
        {
            get => WrappedModel.ProdKeysDownloadUrl ?? "";
            set
            {
                WrappedModel.ProdKeysDownloadUrl = value;
                NotifyPropertyChanged();
            }
        }

        public string TitleKeysDownloadUrl
        {
            get => WrappedModel.TitleKeysDownloadUrl ?? "";
            set
            {
                WrappedModel.TitleKeysDownloadUrl = value;
                NotifyPropertyChanged();
            }
        }

        public bool AlwaysReloadKeysBeforeOpen
        {
            get => WrappedModel.AlwaysReloadKeysBeforeOpen;
            set
            {
                WrappedModel.AlwaysReloadKeysBeforeOpen = value;
                NotifyPropertyChanged();
            }
        }

        public string TitlePageUrl
        {
            get => WrappedModel.TitlePageUrl;
            set
            {
                WrappedModel.TitlePageUrl = value;
                NotifyPropertyChanged();
            }
        }

        public int ProgressBufferSize { get; set; } = 4 * 1024 * 1024;

        public AppSettingsModel WrappedModel { get; private set; } = new();

        public void Update(AppSettingsModel newModel)
        {
            WrappedModel = newModel ?? throw new ArgumentNullException(nameof(newModel));

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
