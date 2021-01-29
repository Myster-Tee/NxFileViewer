namespace Emignatik.NxFileViewer.Views.ObjectPropertyViewer
{
    /// <summary>
    /// Represents a line of the <see cref="PropertiesView"/>
    /// </summary>
    public class PropertyData
    {
        /// <summary>
        /// Object property name (first column)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Object description (Use for tooltip)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Object property value (second column)
        /// </summary>
        public object? Value { get; set; }

    }
}