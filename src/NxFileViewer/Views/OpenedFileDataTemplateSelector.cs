using System.Windows;
using System.Windows.Controls;

namespace Emignatik.NxFileViewer.Views
{
    public class OpenedFileDataTemplateSelector : DataTemplateSelector
    {

        public DataTemplate OpenedFileTemplate { get; set; }

        public DataTemplate NoOpenedFileTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item?.GetType() == typeof(OpenedFileViewModel))
                return OpenedFileTemplate;
            return NoOpenedFileTemplate;
        }
    }
}
