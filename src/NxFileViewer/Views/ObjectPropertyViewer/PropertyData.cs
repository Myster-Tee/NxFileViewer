using System;
using System.Reflection;
using System.Windows;
using Emignatik.NxFileViewer.Utils.MVVM;

namespace Emignatik.NxFileViewer.Views.ObjectPropertyViewer;

/// <summary>
/// Represents a line of the <see cref="PropertiesView"/>
/// </summary>
public class PropertyData : NotifyPropertyChangedBase
{

    private readonly object _obj;
    private readonly PropertyInfo _propertyInfo;

    public PropertyData(PropertyInfo propertyInfo, object obj)
    {
        _propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
        _obj = obj ?? throw new ArgumentNullException(nameof(obj));
    }

    /// <summary>
    /// Get the property info corresponding to this property
    /// </summary>
    public PropertyInfo PropertyInfo => _propertyInfo;

    /// <summary>
    /// Object property name (first column)
    /// </summary>
    public string Name { get; init; } = "";

    /// <summary>
    /// Object description (Use for tooltip)
    /// </summary>
    public string? Description { get; init; }

    public bool HideWhenValueNull { get; init; }

    public Visibility Visibility => HideWhenValueNull && Value == null ? Visibility.Collapsed : Visibility.Visible;

    /// <summary>
    /// Object property value (second column)
    /// </summary>
    public object? Value
    {
        get
        {
            try
            {
                return _propertyInfo.GetValue(_obj);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }

    public new void NotifyPropertyChanged(string propertyName)
    {
        base.NotifyPropertyChanged(propertyName);
    }
}