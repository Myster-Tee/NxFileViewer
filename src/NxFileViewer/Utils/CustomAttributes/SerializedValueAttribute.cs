using System;

namespace Emignatik.NxFileViewer.Utils.CustomAttributes;

public class SerializedValueAttribute : Attribute
{
    public string Value { get; }

    public SerializedValueAttribute(string value)
    {
        Value = value;
    }
}