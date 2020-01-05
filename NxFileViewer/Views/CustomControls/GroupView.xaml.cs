using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Emignatik.NxFileViewer.Views.CustomControls
{
    /// <summary>
    /// Interaction logic for GroupView.xaml
    /// </summary>
    [ContentProperty(nameof(GroupView.Content))]
    public partial class GroupView : Border
    {

        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(GroupView), new PropertyMetadata(default(string), (o, args) =>
                {
                    ((GroupView)o).HeaderTextBlock.Text = args.NewValue as string;
                }));

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(object), typeof(GroupView), new PropertyMetadata(default(object), (o, args) =>
                {
                    ((GroupView)o).ContentControlContent.Content = args.NewValue;
                }));

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public GroupView()
        {
            InitializeComponent();
        }
    }
}
