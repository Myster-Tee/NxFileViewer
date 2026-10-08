using System.IO;

namespace Emignatik.NxFileViewer.Models.TreeItems.Impl;

public sealed class NandFileItem(string path) : ItemBase(null)
{
    public override string Name => Path.GetFileName(path);
    public override string DisplayName => Name;
    public override string LibHacTypeName => "NAND";
    public override string Format => "NAND";
}
