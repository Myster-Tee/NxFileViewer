using System;
using System.Windows.Input;
using Emignatik.NxFileViewer.NSP.Models;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.Views.NSP
{
    public class Pfs0FileViewModel : ViewModelBase
    {
        private bool _isSelected;

        public Pfs0FileViewModel(Pfs0File file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
        }

        public ICommand SaveSelectedFilesCommand { get; internal set; }

        public ICommand DecryptSelectedFilesHeaderCommand { get; internal set; }

        public string FileName
        {
            get
            {
                var fileDefinition = File.Definition;
                return $"{fileDefinition?.FileName}  ({fileDefinition?.FileSize.ToFileSize()})";
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        public Pfs0File File { get; }
    }
}
