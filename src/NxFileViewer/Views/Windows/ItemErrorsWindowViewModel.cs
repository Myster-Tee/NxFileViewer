﻿using System;
using Emignatik.NxFileViewer.Models.TreeItems;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Utils.MVVM;

namespace Emignatik.NxFileViewer.Views.Windows;

public class ItemErrorsWindowViewModel : WindowViewModelBase
{
    private readonly IItem _item;
    private string _errors = string.Empty;

    public ItemErrorsWindowViewModel(IItem item)
    {
        _item = item ?? throw new ArgumentNullException(nameof(item));

        _item.Errors.ErrorsChanged += OnItemErrorsChanged;
        UpdateErrors();
    }

    private void OnItemErrorsChanged(object sender, ErrorsChangedHandlerArgs args)
    {
        UpdateErrors();
    }

    private void UpdateErrors()
    {
        Errors = ErrorsFormatter.Format(_item.Errors);
    }

    public string DisplayName => _item.DisplayName;

    public string Errors
    {
        get => _errors;
        private set
        {
            _errors = value;
            NotifyPropertyChanged();
        }
    }
}