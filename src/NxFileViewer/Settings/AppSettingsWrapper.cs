using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Emignatik.NxFileViewer.Settings.Models;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings
{
    public class AppSettingsWrapper : IAppSettings
    {
        public event PropertyChangedEventHandler? PropertyChanged;


        public AppSettingsModel Model { get; private set; } = new();


        public string? AppLanguage
        {
            get => Model.AppLanguage ?? "";
            set
            {
                Model.AppLanguage = value;
                NotifyPropertyChanged();
            }
        }

        public string LastSaveDir
        {
            get => Model.LastSaveDir ?? "";
            set
            {
                Model.LastSaveDir = value;
                NotifyPropertyChanged();
            }
        }

        public string LastOpenedFile
        {
            get => Model.LastOpenedFile ?? "";
            set
            {
                Model.LastOpenedFile = value;
                NotifyPropertyChanged();
            }
        }

        public string ProdKeysFilePath
        {
            get => Model.KeysFilePath ?? "";
            set
            {
                Model.KeysFilePath = value;
                NotifyPropertyChanged();
            }
        }

        public string ConsoleKeysFilePath
        {
            get => Model.ConsoleKeysFilePath ?? "";
            set
            {
                Model.ConsoleKeysFilePath = value;
                NotifyPropertyChanged();
            }
        }

        public string TitleKeysFilePath
        {
            get => Model.TitleKeysFilePath ?? "";
            set
            {
                Model.TitleKeysFilePath = value;
                NotifyPropertyChanged();
            }
        }

        public LogLevel LogLevel
        {
            get => Model.LogLevel;
            set
            {
                Model.LogLevel = value;
                NotifyPropertyChanged();
            }
        }

        public string ProdKeysDownloadUrl
        {
            get => Model.ProdKeysDownloadUrl ?? "";
            set
            {
                Model.ProdKeysDownloadUrl = value;
                NotifyPropertyChanged();
            }
        }

        public string TitleKeysDownloadUrl
        {
            get => Model.TitleKeysDownloadUrl ?? "";
            set
            {
                Model.TitleKeysDownloadUrl = value;
                NotifyPropertyChanged();
            }
        }

        public bool AlwaysReloadKeysBeforeOpen
        {
            get => Model.AlwaysReloadKeysBeforeOpen;
            set
            {
                Model.AlwaysReloadKeysBeforeOpen = value;
                NotifyPropertyChanged();
            }
        }

        public string TitlePageUrl
        {
            get => Model.TitlePageUrl;
            set
            {
                Model.TitlePageUrl = value;
                NotifyPropertyChanged();
            }
        }

        public int ProgressBufferSize { get; set; } = 4 * 1024 * 1024;


        public void Update(AppSettingsModel newModel)
        {
            Model = newModel ?? throw new ArgumentNullException(nameof(newModel));

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
