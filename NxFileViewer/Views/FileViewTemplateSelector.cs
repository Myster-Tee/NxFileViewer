using System.Windows;
using System.Windows.Controls;
using Emignatik.NxFileViewer.Views.NCA;
using Emignatik.NxFileViewer.Views.NSP;

namespace Emignatik.NxFileViewer.Views
{
    /// <summary>
    /// Selects the view corresponding to the view model
    /// </summary>
    public class FileViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NspInfoViewDataTemplate { get; set; }

        public DataTemplate NcaInfoViewDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is NspInfoViewModel)
                return NspInfoViewDataTemplate;

            if (item is NcaInfoViewModel)
                return NcaInfoViewDataTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
