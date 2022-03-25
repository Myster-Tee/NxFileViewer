using System;
using System.Reflection;

namespace Emignatik.NxFileViewer.Views.ObjectPropertyViewer;

[AttributeUsage(AttributeTargets.Property)]
public class PropertyViewAttribute : Attribute
{

    public PropertyViewAttribute(string? name = null)
    {
        Name = name;
    }

    /// <summary>
    /// When defined, overrides the object <see cref="MemberInfo.Name"/>
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// When specified, overrides the object <see cref="MemberInfo.Name"/> with the corresponding localized value
    /// </summary>
    public string? NameLocalizationKey { get; set; }

    /// <summary>
    /// The property description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The key to the localized description
    /// </summary>
    public string? DescriptionLocalizationKey { get; set; }
}