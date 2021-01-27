using System;
using System.Reflection;

namespace Emignatik.NxFileViewer.Views.ObjectPropertyViewer
{
    public class PropertiesViewAttribute : Attribute
    {

        public PropertiesViewAttribute(string? name = null)
        {
            Name = name;
        }

        /// <summary>
        /// When defined, replaces the object <see cref="PropertyInfo"/> name
        /// </summary>
        public string? Name { get; }

        public bool IsNameLocalized { get; set; } = false;
    }
}
