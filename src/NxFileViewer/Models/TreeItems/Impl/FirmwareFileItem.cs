using System.IO;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

// Firmware hash verification requires no decryption keys or LibHac storage handles.
public sealed class FirmwareFileItem(string path) : ItemBase(null)
{
    public override string Name => Path.GetFileName(path.TrimEnd('/'));
    public override string DisplayName => Name;
    public override string LibHacTypeName => "Firmware";
    public override string Format => "SHA-256";
}
