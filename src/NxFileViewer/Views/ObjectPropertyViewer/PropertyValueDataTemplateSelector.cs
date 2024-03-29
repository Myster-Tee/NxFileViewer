﻿using System.Windows;
using System.Windows.Controls;

namespace Emignatik.NxFileViewer.Views.ObjectPropertyViewer;

public class PropertyValueDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate StringTemplate { get; set; } = new();

    public DataTemplate BoolTemplate { get; set; } = new();

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is bool)
            return BoolTemplate;

        return StringTemplate;

    }
}