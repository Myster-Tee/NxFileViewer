using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Emignatik.NxFileViewer.Logging;
using Emignatik.NxFileViewer.NSP.Models;
using Emignatik.NxFileViewer.Properties;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Views.MVVM;
using Emignatik.NxFileViewer.Views.NCA;

namespace Emignatik.NxFileViewer.Views.NSP
{
    public class NspInfoViewModel : FileViewModelBase
    {
        private readonly NspInfo _nspInfo;
        private TitleInfo _selectedTitle;
        private string _appName;
        private string _publisher;
        private BitmapImage _icon = null;
        private PfsFileViewModel _selectedPfsFile;
        private FileViewModelBase _selectedFileInfo;

        public NspInfoViewModel(NspInfo nspInfo)
        {
            _nspInfo = nspInfo ?? throw new ArgumentNullException(nameof(nspInfo));

            var nacpTitles = Titles;
            SelectedTitle = nacpTitles?.FirstOrDefault();

            var saveSelectedFilesCommand = new RelayCommand(OnSaveSelectedFiles);
            var decryptSelectedFilesHeaderCommand = new RelayCommand(OnDecryptSelectedFilesHeader);
            PfsFiles = _nspInfo.Files?.Select(file => new PfsFileViewModel(file)
            {
                SaveSelectedFilesCommand = saveSelectedFilesCommand,
                DecryptSelectedFilesHeaderCommand = decryptSelectedFilesHeaderCommand,
            }).ToArray();
        }

        public string AppType => _nspInfo?.CnmtInfo.Type.ToString();

        public string DisplayVersion => _nspInfo.NacpInfo?.DisplayVersion;

        public string TitleId => _nspInfo?.CnmtInfo.TitleId;

        public uint? TitleVersion => _nspInfo?.CnmtInfo.TitleVersion;

        public PfsFileViewModel[] PfsFiles { get; }

        public PfsFileViewModel SelectedPfsFile
        {
            get => _selectedPfsFile;
            set
            {
                _selectedPfsFile = value;
                NotifyPropertyChanged();
                UpdateOnSelectedPfsFileChanged();
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

        public TitleInfo[] Titles
        {
            get
            {
                var nacpInfo = _nspInfo.NacpInfo;
                return nacpInfo?.Titles?.ToArray();
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

        private void UpdateOnSelectedPfsFileChanged()
        {
            var selectedFile = SelectedPfsFile;

            SelectedFileInfo = !(selectedFile?.File is PfsNcaFile pfsNcaFile) ? null : new NcaInfoViewModel(pfsNcaFile)
            {
                Source = $"{TitleId}!{pfsNcaFile.Name}"
            };
        }

        private void UpdateOnSelectedTitleChanged()
        {
            var selectedTitle = SelectedTitle;
            AppName = selectedTitle != null ? selectedTitle.AppName : "";
            Publisher = selectedTitle != null ? selectedTitle.Publisher : "";
            Icon = GetIconForTitle(selectedTitle);
        }

        private BitmapImage GetIconForTitle(TitleInfo title)
        {
            var nacpInfo = _nspInfo.NacpInfo;
            if (title == null || nacpInfo == null)
                return null;

            var iconOfSelectedLanguage = _nspInfo.Icons.FirstOrDefault(iconTmp => iconTmp.Language == title.Language);

            return iconOfSelectedLanguage?.Image;
        }

        private string[] GetSelectedFileNames()
        {
            return (PfsFiles ?? new PfsFileViewModel[0]).Where(vm => vm.IsSelected).Select(vm => vm.FileName).ToArray();
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
                //TODO: finir la fonction de sauvegarde
                //Pfs0Utils.SavePfs0Files(Source, selectedFileNames, saveDir);
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
            //TODO: finir la fonction de sauvegarde

            //var selectedFileNames = GetSelectedFileNames();

            //if (selectedFileNames.Length <= 0)
            //    return;

            //if (!KeySetProviderService.TryGetKeySet(out var keySet, out var errorMessage))
            //{
            //    Logger.LogError($"Can't decrypt header(s) of selected file(s), keys can't be obtained: {errorMessage}");
            //    return;
            //}

            //if (!PromptSaveDir(out var saveDir))
            //    return;

            //try
            //{
            //    Pfs0Utils.DecryptPfs0NcaFilesHeader(Source, selectedFileNames, saveDir, keySet);
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogError("Failed to decrypt header(s) of selected file(s).", ex);
            //}

        }
    }
}