using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Emignatik.NxFileViewer.Logging;
using Emignatik.NxFileViewer.NSP.Models;
using Emignatik.NxFileViewer.NxFormats.NACP.Models;
using Emignatik.NxFileViewer.NxFormats.PFS0;
using Emignatik.NxFileViewer.Properties;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Views.MVVM;
using Emignatik.NxFileViewer.Views.NCA;

namespace Emignatik.NxFileViewer.Views.NSP
{
    public class NspInfoViewModel : FileViewModelBase
    {
        private readonly NspInfo _nspInfo;
        private NacpTitle _selectedTitle;
        private string _appName;
        private string _publisher;
        private BitmapImage _icon = null;
        private Pfs0FileViewModel _selectedPfs0File;
        private FileViewModelBase _selectedFileInfo;

        public NspInfoViewModel(NspInfo nspInfo)
        {
            _nspInfo = nspInfo ?? throw new ArgumentNullException(nameof(nspInfo));

            var nacpTitles = Titles;
            SelectedTitle = nacpTitles?.FirstOrDefault();

            var saveSelectedFilesCommand = new RelayCommand(OnSaveSelectedFiles);
            var decryptSelectedFilesHeaderCommand = new RelayCommand(OnDecryptSelectedFilesHeader);
            Pfs0Files = _nspInfo.Files?.Select(file => new Pfs0FileViewModel(file)
            {
                SaveSelectedFilesCommand = saveSelectedFilesCommand,
                DecryptSelectedFilesHeaderCommand = decryptSelectedFilesHeaderCommand,
            }).ToArray();
        }

        public string AppType => _nspInfo.CnmtHeader.Type.ToString();

        public string DisplayVersion => _nspInfo.NcaControlContent?.NacpContent.DisplayVersion;

        public string TitleId => _nspInfo.CnmtHeader.TitleId;

        public uint TitleVersion => _nspInfo.CnmtHeader.TitleVersion;

        public Pfs0FileViewModel[] Pfs0Files { get; }

        public Pfs0FileViewModel SelectedPfs0File
        {
            get => _selectedPfs0File;
            set
            {
                _selectedPfs0File = value;
                NotifyPropertyChanged();
                UpdateOnSelectedPfs0FileChanged();
            }
        }

        public FileViewModelBase SelectedFileInfo
        {
            get => _selectedFileInfo;
            set
            {
                _selectedFileInfo = value;
                NotifyPropertyChanged();
            }
        }

        public NacpTitle[] Titles
        {
            get
            {
                var ncaControlContent = _nspInfo.NcaControlContent;
                return ncaControlContent?.NacpContent.Titles;
            }
        }

        public NacpTitle SelectedTitle
        {
            get => _selectedTitle;
            set
            {
                _selectedTitle = value;
                NotifyPropertyChanged();
                UpdateOnSelectedTitleChanged();
            }
        }

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

        public BitmapImage Icon
        {
            get => _icon;
            private set
            {
                _icon = value;
                NotifyPropertyChanged();
            }
        }

        private void UpdateOnSelectedPfs0FileChanged()
        {
            var selectedFile = SelectedPfs0File;

            SelectedFileInfo = !(selectedFile?.File is Pfs0NcaFile pfs0NcaFile) ? null : new NcaInfoViewModel(pfs0NcaFile.Header)
            {
                Source = $"{TitleId}!{pfs0NcaFile.Definition.FileName}"
            };
        }

        private void UpdateOnSelectedTitleChanged()
        {
            var selectedTitle = SelectedTitle;
            AppName = selectedTitle != null ? selectedTitle.AppName : "";
            Publisher = selectedTitle != null ? selectedTitle.Publisher : "";
            Icon = GetIconForTitle(selectedTitle);
        }

        private BitmapImage GetIconForTitle(NacpTitle title)
        {
            var ncaControlContent = _nspInfo.NcaControlContent;
            if (title == null || ncaControlContent == null)
                return null;

            var iconOfSelectedLanguage = ncaControlContent.Icons.FirstOrDefault(iconTmp => iconTmp.Language == title.Language);

            var iconFilePath = iconOfSelectedLanguage?.IconFilePath;
            if (iconFilePath == null || !File.Exists(iconFilePath))
                return null;

            try
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache; // Important: to prevent WPF from locking the file
                bitmapImage.UriSource = new Uri(iconFilePath);
                bitmapImage.EndInit();
                return bitmapImage;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to load icon from file \"{iconFilePath}\".", ex);
                return null;
            }
        }

        private string[] GetSelectedFileNames()
        {
            return (Pfs0Files ?? new Pfs0FileViewModel[0]).Where(vm => vm.IsSelected).Select(vm => vm.File.Definition?.FileName).ToArray();
        }

        private void OnSaveSelectedFiles()
        {
            var selectedFileNames = GetSelectedFileNames();

            if (selectedFileNames.Length <= 0)
                return;

            if (!PromptSaveDir(out var saveDir))
                return;

            try
            {
                Pfs0Utils.SavePfs0Files(Source, selectedFileNames, saveDir);
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to save selected file(s).", ex);
            }
        }

        private static bool PromptSaveDir(out string saveDir)
        {
            var fbd = new FolderBrowserDialog
            {
                SelectedPath = Settings.Default.LastSaveDir
            };

            if (fbd.ShowDialog() != DialogResult.OK)
            {
                saveDir = null;
                return false;
            }

            var selectedDir = fbd.SelectedPath;
            try
            {
                Settings.Default.LastSaveDir = selectedDir;
                Settings.Default.Save();
            }
            catch
            {
            }

            saveDir = selectedDir;
            return true;
        }

        private void OnDecryptSelectedFilesHeader()
        {
            var selectedFileNames = GetSelectedFileNames();

            if (selectedFileNames.Length <= 0)
                return;

            if (!KeySetProviderService.TryGetKeySet(out var keySet, out var errorMessage))
            {
                Logger.LogError($"Can't decrypt header(s) of selected file(s), keys can't be obtained: {errorMessage}");
                return;
            }

            if (!PromptSaveDir(out var saveDir))
                return;

            try
            {
                Pfs0Utils.DecryptPfs0NcaFilesHeader(Source, selectedFileNames, saveDir, keySet);
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to decrypt header(s) of selected file(s).", ex);
            }

        }
    }
}