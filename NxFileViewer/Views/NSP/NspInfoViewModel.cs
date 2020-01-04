using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Emignatik.NxFileViewer.NSP.Models;
using Emignatik.NxFileViewer.Properties;
using log4net;

namespace Emignatik.NxFileViewer.Views.NSP
{
    public class NspInfoViewModel : FileViewModelBase
    {
        private readonly NspInfo _nspInfo;

        private FileViewModelBase _selectedFileInfo;
        private readonly ILog _log;
        private CnmtViewModel _selectedCnmt;

        public NspInfoViewModel(NspInfo nspInfo, FileViewModelFactory fileViewModelFactory)
        {
            _nspInfo = nspInfo ?? throw new ArgumentNullException(nameof(nspInfo));
            FileName = $"{Path.GetFileName(_nspInfo.Location)}";

            _log = LogManager.GetLogger(this.GetType());

            Files = _nspInfo.Files?.Select(file =>
            {
                var fileViewModel = fileViewModelFactory.Create(file);
                fileViewModel.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(IsSelected) && fileViewModel.IsSelected)
                        SelectedFile = fileViewModel;

                };
                return fileViewModel;
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

        public IEnumerable<FileViewModelBase> Files { get; }


        public FileViewModelBase SelectedFile
        {
            get => _selectedFileInfo;
            set
            {
                _selectedFileInfo = value;
                NotifyPropertyChanged();
            }
        }

    }
}