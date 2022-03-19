using System.Windows;
using System.Windows.Controls;
using Emignatik.NxFileViewer.Views.UserControls;

namespace Emignatik.NxFileViewer.Views;

public class OpenedFileDataTemplateSelector : DataTemplateSelector
{

    public DataTemplate OpenedFileTemplate { get; set; } = new();

    public DataTemplate NoOpenedFileTemplate { get; set; } = new();

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item?.GetType() == typeof(OpenedFileViewModel))
            return OpenedFileTemplate;
        return NoOpenedFileTemplate;
    }
}