using System.ComponentModel;
using System.Windows;

namespace Emignatik.NxFileViewer.Views
{
    /// <summary>
    /// Logique d'interaction pour ItemsErrorsWindow.xaml
    /// </summary>
    public partial class ItemErrorsWindow : Window
    {


        public ItemErrorsWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}
