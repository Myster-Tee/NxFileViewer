using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Emignatik.NxFileViewer.Localization;

namespace Emignatik.NxFileViewer.Views.ObjectPropertyViewer
{
    /// <summary>
    /// Logique d'interaction pour PropertiesView.xaml
    /// </summary>
    public partial class PropertiesView : UserControl
    {
        public PropertiesView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BuildFromDataContext(e.NewValue);
        }

        private void BuildFromDataContext(object? newDataContext)
        {

            var properties = new ObservableCollection<PropertyData>();
            PropertiesDataGrid.DataContext = properties;

            if (newDataContext == null)
                return;


            var propertyInfos = newDataContext.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            foreach (var propertyInfo in propertyInfos)
            {
                var propertiesViewAttribute = propertyInfo.GetCustomAttributes<PropertiesViewAttribute>(true).FirstOrDefault();
                if (propertiesViewAttribute != null)
                {

                    var name = propertiesViewAttribute.Name ?? propertyInfo.Name;
                    if (propertiesViewAttribute.IsNameLocalized)
                    {
                        if (LocalizationManager.Instance.Current.Keys.TryGetValue(name, out var localizedName))
                            name = localizedName;
                    }

                    properties.Add(new PropertyData
                    {
                        Name = name,
                        Value = propertyInfo.GetValue(newDataContext)
                    });
                }
            }

        }

    }
}
