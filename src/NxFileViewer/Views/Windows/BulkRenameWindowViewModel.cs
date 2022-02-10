using System;
using System.Windows.Input;
using Emignatik.NxFileViewer.Services;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Views.Windows;

public class BulkRenameWindowViewModel : WindowViewModelBase
{
    private readonly IFileRenamerService _fileRenamerService;

    public BulkRenameWindowViewModel(IFileRenamerService fileRenamerService)
    {
        _fileRenamerService = fileRenamerService ?? throw new ArgumentNullException(nameof(fileRenamerService));
        RenameCommand = new RelayCommand(OnRename);
        BrowseInputDirectoryCommand = new RelayCommand(OnBrowseInputDirectory);
    }

    public ICommand RenameCommand { get; }
    public ICommand BrowseInputDirectoryCommand { get; }

    private string _inputDirectory;
    private string _namingPattern;

    public string InputDirectory
    {
        get => _inputDirectory;
        set
        {
            _inputDirectory = value;
            NotifyPropertyChanged();
        }
    }

    public string NamingPattern
    {
        get => _namingPattern;
        set
        {
            _namingPattern = value;
            NotifyPropertyChanged();
        }
    }

    private void OnBrowseInputDirectory()
    {

    }


    private void OnRename()
    {
        //TODO: exposer tous les paramètres
        try
        {
            _fileRenamerService.Rename(InputDirectory, NamingPattern, new[] { ".nsp", ".nsz", ".xci", ".xcz" }, true);
        }
        catch (Exception ex)
        {
            //TODO: gérer l'exception
        }
    }


}