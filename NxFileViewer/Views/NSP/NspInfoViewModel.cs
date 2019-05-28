using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Emignatik.NxFileViewer.NSP.Models;
using Emignatik.NxFileViewer.Properties;
using Emignatik.NxFileViewer.Views.MVVM;
using Emignatik.NxFileViewer.Views.NCA;
using log4net;

namespace Emignatik.NxFileViewer.Views.NSP
{
    public class NspInfoViewModel : FileViewModelBase
    {
        private readonly NspInfo _nspInfo;

        private PfsFileViewModel _selectedPfsFile;
        private FileViewModelBase _selectedFileInfo;
        private readonly ILog _log;
        private CnmtViewModel _selectedCnmt;

        public NspInfoViewModel(NspInfo nspInfo)
        {
            _nspInfo = nspInfo ?? throw new ArgumentNullException(nameof(nspInfo));

            _log = LogManager.GetLogger(this.GetType());

            var saveSelectedFilesCommand = new RelayCommand(OnSaveSelectedFiles);
            var decryptSelectedFilesHeaderCommand = new RelayCommand(OnDecryptSelectedFilesHeader);
            PfsFiles = _nspInfo.Files?.Select(file => new PfsFileViewModel(file)
            {
                SaveSelectedFilesCommand = saveSelectedFilesCommand,
                DecryptSelectedFilesHeaderCommand = decryptSelectedFilesHeaderCommand,
            });

            var cnmts = _nspInfo.Cnmts?.OrderBy(cnmt => cnmt.Type).Select((cnmtInfo, i) => new CnmtViewModel(cnmtInfo)
            {
                TabTitle = string.Format(Resources.ContentNum, i + 1)
            }).ToArray();
            Cnmts = cnmts;
            SelectedCnmt = cnmts?.FirstOrDefault();
        }

        public CnmtViewModel SelectedCnmt
        {
            get => _selectedCnmt;
            set
            {
                _selectedCnmt = value;
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<CnmtViewModel> Cnmts { get; }

        public IEnumerable<PfsFileViewModel> PfsFiles { get; }

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

        private void UpdateOnSelectedPfsFileChanged()
        {
            var selectedFile = SelectedPfsFile;

            SelectedFileInfo = !(selectedFile?.File is PfsNcaFile pfsNcaFile) ? null : new NcaInfoViewModel(pfsNcaFile)
            {
                Source = $"{Source}!{pfsNcaFile.Name}"
            };
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
                _log.Error("Failed to save selected file(s).", ex);
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