using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Emignatik.NxFileViewer.Localization;

namespace Emignatik.NxFileViewer.Views.ObjectPropertyViewer;

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

        foreach (var (propertiesViewAttribute, propertyInfo) in GetCachedObjectTypeProperties(newDataContext.GetType()))
        {
            var name = propertiesViewAttribute.Name ?? propertyInfo.Name;
            var nameLocalizationKey = propertiesViewAttribute.NameLocalizationKey;
            if (nameLocalizationKey != null)
            {
                if (LocalizationManager.Instance.Current.Keys.TryGetValue(nameLocalizationKey, out var localizedName))
                    name = localizedName;
            }

            var description = propertiesViewAttribute.Description;
            var descriptionLocalizationKey = propertiesViewAttribute.DescriptionLocalizationKey;
            if (descriptionLocalizationKey != null)
            {
                if (LocalizationManager.Instance.Current.Keys.TryGetValue(descriptionLocalizationKey, out var localizedDescription))
                    description = localizedDescription;
            }

            var propertyValue = propertyInfo.GetValue(newDataContext);
            var visibility = propertiesViewAttribute.HideIfNull && propertyValue == null ? Visibility.Collapsed : Visibility.Visible;

            properties.Add(new PropertyData
            {
                Name = name,
                Description = description,
                Value = propertyValue,
                Visibility = visibility,
            });
        }

    }

    private static readonly Dictionary<Type, Property[]> CachedPropertiesByType = new();

    private static Property[] GetCachedObjectTypeProperties(Type type)
    {
        if (CachedPropertiesByType.TryGetValue(type, out var properties))
            return properties;
        properties = GetObjectTypeProperties(type).ToArray();

        CachedPropertiesByType.Add(type, properties);
        return properties;
    }

    /// <summary>
    /// Returns the list of properties marked with the <see cref="PropertyViewAttribute"/> attribute and
    /// ordered from the highest to the lowest type in the inheritance hierarchy
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static IEnumerable<Property> GetObjectTypeProperties(Type type)
    {
        var typesHierarchy = new List<Type>();
        var tTmp = type;
        while (tTmp != null)
        {
            typesHierarchy.Insert(0, tTmp);
            tTmp = tTmp.BaseType;
        }

        foreach (var typeTmp in typesHierarchy)
        {
            foreach (var propertyInfo in typeTmp.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.DeclaredOnly))
            {
                var propertiesViewAttribute = propertyInfo.GetCustomAttributes<PropertyViewAttribute>(false).FirstOrDefault();
                if (propertiesViewAttribute != null)
                    yield return new Property(propertiesViewAttribute, propertyInfo);
            }
        }
    }

    private class Property
    {
        public PropertyViewAttribute Attribute { get; }
        public PropertyInfo PropertyInfo { get; }

        public Property(PropertyViewAttribute attribute, PropertyInfo propertyInfo)
        {
            Attribute = attribute;
            PropertyInfo = propertyInfo;
        }

        // Return the first and last name.
        public void Deconstruct(out PropertyViewAttribute propertyViewAttribute, out PropertyInfo propertyInfo)
        {
            propertyViewAttribute = Attribute;
            propertyInfo = PropertyInfo;
        }
    }
}