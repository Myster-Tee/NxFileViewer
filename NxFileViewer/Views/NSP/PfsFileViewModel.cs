using System;
using System.Windows.Input;
using Emignatik.NxFileViewer.NSP.Models;
using Emignatik.NxFileViewer.Utils;

namespace Emignatik.NxFileViewer.Views.NSP
{
    public class PfsFileViewModel : ViewModelBase
    {
        private bool _isSelected;

        public PfsFileViewModel(PfsFile file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
        }

        public ICommand SaveSelectedFilesCommand { get; internal set; }

        public ICommand DecryptSelectedFilesHeaderCommand { get; internal set; }

        public string FileName => $"{File.Name}  ({File.Size.ToFileSize()})";

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        public PfsFile File { get; }
    }
}
