using System.Windows.Input;
using Emignatik.NxFileViewer.Utils.MVVM;

namespace Emignatik.NxFileViewer.Views
{
    public abstract class FileViewModelBase : ViewModelBase
    {

        private bool _isSelected;
        private string _location;


        public FileViewModelBase()
        {
            SaveSelectedFilesCommand = new RelayCommand(OnSaveSelectedFiles);
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

        public ICommand SaveSelectedFilesCommand { get; }

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                NotifyPropertyChanged();
            }
        }

        private void OnSaveSelectedFiles()
        {

            //TODO: finir la fonction de sauvegarde

        }

        public string FileName { get; set; }

    }

}