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
    private string _basePattern;
    private string _patchPattern;
    private string _addonPattern;

    public string InputDirectory
    {
        get => _inputDirectory;
        set
        {
            _inputDirectory = value;
            NotifyPropertyChanged();
        }
    }

    public string BasePattern
    {
        get => _basePattern;
        set
        {
            _basePattern = value;
            NotifyPropertyChanged();
        }
    }

    public string PatchPattern
    {
        get => _patchPattern;
        set
        {
            _patchPattern = value;
            NotifyPropertyChanged();
        }
    }

    public string AddonPattern
    {
        get => _addonPattern;
        set
        {
            _addonPattern = value;
            NotifyPropertyChanged();
        }
    }

    private void OnBrowseInputDirectory()
    {
        //TODO: à implémenter
    }


    private void OnRename()
    {
        //TODO: exposer tous les paramètres
        try
        {
            _fileRenamerService.Rename(InputDirectory, BasePattern, new[] { ".nsp", ".nsz", ".xci", ".xcz" }, true);
        }
        catch (Exception ex)
        {
            //TODO: gérer l'exception
        }
    }


}