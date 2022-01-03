using System;
using LibHac.Fs;

namespace Emignatik.NxFileViewer.Model.TreeItems.Impl
{
    public class NacpItem : DirectoryEntryItem
    {
        public const string NacpFileName = "control.nacp";

        public NacpItem(Nacp nacp, SectionItem parentItem, DirectoryEntry directoryEntry, string name, string path)
            : base(parentItem, directoryEntry, name, path)
        {
            Nacp = nacp ?? throw new ArgumentNullException(nameof(nacp));
            ParentItem = parentItem ?? throw new ArgumentNullException(nameof(parentItem));
            AddOnContentBaseId = Nacp.AddOnContentBaseId.ToStrId();
        }

        public override SectionItem ParentItem { get; }

        public Nacp Nacp { get; }

        public override string LibHacUnderlyingTypeName => nameof(Nacp);

        public string DisplayVersion => Nacp.DisplayVersion;

        public string AddOnContentBaseId { get; }

        public string Isbn => Nacp.Isbn;

        public string ApplicationErrorCodeCategory => Nacp.ApplicationErrorCodeCategory;

        public string BcatPassphrase => Nacp.BcatPassphrase;

        public uint ParentalControlFlag => Nacp.ParentalControlFlag;
    }
}